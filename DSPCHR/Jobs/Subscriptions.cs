using DSPCHR.Data;
using DSPCHR.Models;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Jobs
{
    public class Subscriptions
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Jobs.Messages _messagesJob;
        public Subscriptions(ApplicationDbContext applicationDbContext, Jobs.Messages messagesJob)
        {
            _dbContext = applicationDbContext;
        }

        // Process subscription notifications
        public void ProcessNotification(Models.Subscription subscription)
        {
            var subscriber = _dbContext.Subscribers.Where(s => s.OfferCode.Equals(subscription.OfferCode)
                                                                && s.Msisdn.Equals(subscription.Msisdn))
                                                                .FirstOrDefault();
            if (subscriber != null) // Subscriber record exists
            {
                if (subscription.Type.ToLower().StartsWith("activ"))
                {
                    subscriber.IsActive = true;
                    subscriber.LastSubscribedAt = subscription.Timestamp;
                }
                else // unsubscription 
                {
                    subscriber.IsActive = false;
                    subscriber.LastUnsubscribedAt = subscription.Timestamp;
                }

                _dbContext.Update(subscriber);
            }
            else // Subscriber record does not exist
            {
                subscriber = new Models.Subscriber
                {
                    Msisdn = subscription.Msisdn,
                    OfferCode = subscription.OfferCode,
                    ShortCode = subscription.ShortCode
                };

                if (subscription.Type.ToLower().StartsWith("activ"))
                {
                    subscriber.IsActive = true;
                    subscriber.FirstSubscribedAt = subscription.Timestamp;
                    subscriber.LastSubscribedAt = subscription.Timestamp;
                }
                else // unsubscription 
                {
                    subscriber.IsActive = false;
                    subscriber.LastUnsubscribedAt = subscription.Timestamp;
                }

                _dbContext.Add(subscriber);
            }
            
            _dbContext.SaveChanges();

            // Send welcome message to user who has just subscribed
            if (subscriber.IsActive)
            {
                var mt = new OutboundMessage
                {
                    Destination = subscriber.Msisdn,
                    CreatedAt = DateTime.Now,
                    OfferCode = subscriber.OfferCode,
                    SendAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                mt.Content = subscriber.FirstSubscribedAt == subscriber.LastSubscribedAt ?
                                "Welcome to the service" :
                                "Welcome back to the service";

                _dbContext.Add(mt);
                _dbContext.SaveChanges();

                BackgroundJob.Enqueue<Jobs.Messages>((_messagesJob) => _messagesJob.SendToSubscriber(mt.Id));

            }

        }
    }
}