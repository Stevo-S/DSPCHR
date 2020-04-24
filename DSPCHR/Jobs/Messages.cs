using DSPCHR.Data;
using DSPCHR.Models;
using Microsoft.Extensions.Configuration;
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
        private readonly short _maxDegreeOfParallelism = 8;
        public Messages(ApplicationDbContext applicationDbContext, Gateway.Client gatewayClient, IConfiguration configuration)
        {
            _dbContext = applicationDbContext;
            _gatewayClient = gatewayClient;
            short.TryParse(configuration["DegreeOfParallelism"], out _maxDegreeOfParallelism);
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
            var subscribers = _dbContext.Subscribers.Where(s => s.OfferCode.Equals(batchMt.OfferCode)
                && s.IsActive).ToList();

            if (batchMt != null)
            {
                ParallelOptions parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = _maxDegreeOfParallelism
                };

                //Parallel.ForEach(subscribers, parallelOptions, subscriber => 
                //{
                //    var mtMessage = new OutboundMessage
                //    {
                //        Destination = subscriber.Msisdn,
                //        Content = batchMt.Content,
                //        OfferCode = subscriber.OfferCode,
                //        ShortCode = batchMt.ShortCode,
                //        LinkId = ""
                //    };

                //    _gatewayClient.Send("api/outboundsms", MtMessageJsonBody(mtMessage));
                //});


                var batchSize = (int)(Math.Ceiling((float)(subscribers.Count / _maxDegreeOfParallelism)));

                Parallel.For(0, _maxDegreeOfParallelism, batchNumber => 
                {
                    var subscriberBatch = subscribers.Skip(batchNumber * batchSize).Take(batchSize);

                    foreach(var subscriber in subscriberBatch)
                    {
                        var mtMessage = new OutboundMessage
                        {
                            Destination = subscriber.Msisdn,
                            Content = batchMt.Content,
                            OfferCode = subscriber.OfferCode,
                            ShortCode = batchMt.ShortCode,
                            LinkId = ""
                        };

                        _gatewayClient.Send("api/outboundsms", MtMessageJsonBody(mtMessage));
                    }
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
