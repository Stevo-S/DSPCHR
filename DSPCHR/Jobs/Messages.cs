using DSPCHR.Data;
using DSPCHR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DSPCHR.Jobs
{
    public class Messages
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Gateway.Client _gatewayClient;
        public Messages(ApplicationDbContext applicationDbContext, Gateway.Client gatewayClient)
        {
            _dbContext = applicationDbContext;
            _gatewayClient = gatewayClient;
        }
        public void SendToSubscriber(long outboundMessageId)
        {
            var mtMessage = _dbContext.OutboundMessages.Find(outboundMessageId);

            if(mtMessage != null)
            {
                _gatewayClient.Send("api/outboundsms", MtMessageJsonBody(mtMessage));
            }
        }

        public void SendToSubscribers(long subscriptionOfferWideMessageId)
        {
            var batchMt = _dbContext.SubscriptionOfferWideMessages.Find(subscriptionOfferWideMessageId);
            var subscribers = _dbContext.Subscribers.Where(s => s.OfferCode.Equals(batchMt.OfferCode)).ToList();

            if (batchMt != null)
            {
                // TODO: Implement sending messages to all subscribers of an offer
                //foreach(var subscriber in subscribers)
                //{
                //    var mtMessage = new OutboundMessage 
                //    {
                //        Destination = subscriber.Msisdn,
                //        Content = batchMt.Content,
                //        OfferCode = subscriber.OfferCode,
                //        ShortCode = batchMt.ShortCode,
                //        LinkId = ""
                //    };
                //}

                ParallelOptions parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 8
                };

                Parallel.ForEach(subscribers, parallelOptions, subscriber => 
                {
                    Console.WriteLine("Subscriber: " + subscriber.Msisdn);
                    var mtMessage = new OutboundMessage
                    {
                        Destination = subscriber.Msisdn,
                        Content = batchMt.Content,
                        OfferCode = subscriber.OfferCode,
                        ShortCode = batchMt.ShortCode,
                        LinkId = ""
                    };

                    _gatewayClient.Send("api/outboundsms", MtMessageJsonBody(mtMessage));
                });

            }
        }

        private string MtMessageJsonBody(OutboundMessage outboundMessage)
        {
            var requestBody = new
            {
                Msisdn = outboundMessage.Destination,
                OfferCode = outboundMessage.OfferCode,
                MessageContent = outboundMessage.Content
            };

            return JsonSerializer.Serialize(requestBody);

        }
    }
}
