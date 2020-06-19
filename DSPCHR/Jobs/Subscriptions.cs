using DSPCHR.Data;
using DSPCHR.Models;
using Hangfire;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DSPCHR.Jobs
{
    public class Subscriptions
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Jobs.Messages _messagesJob;
        private readonly Gateway.Client _gatewayClient;
        private readonly IHttpClientFactory _httpClientFactory;
        public Subscriptions(ApplicationDbContext applicationDbContext, Jobs.Messages messagesJob, 
            Gateway.Client gatewayClient, IHttpClientFactory clientFactory)
        {
            _dbContext = applicationDbContext;
            _gatewayClient = gatewayClient;
            _messagesJob = messagesJob;
            _httpClientFactory = clientFactory;
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

            // If it was an activation
            if (subscriber.IsActive)
            {
                // Check if there was a registered click for this MSISDN-OfferCode pair
                // and record successful conversion
                var click = _dbContext.WebActivationClicks.Where(wac => wac.Msisdn.Equals(subscriber.Msisdn)
                    && wac.OfferCode.Equals(subscriber.OfferCode) && !wac.Converted).
                    OrderByDescending(wac => wac.CreatedAt).Include(wac => wac.WebActivator).FirstOrDefault();

                if (click != null)
                {
                    click.Converted = true;
                    click.LastUpdated = DateTime.Now;

                    _dbContext.SaveChanges();

                    // Post back conversion success
                    if (click.WebActivator?.PostBackUrl != null)
                    {
                        // include original click ID in Postback URL
                        var postBackUrl = click.WebActivator.PostBackUrl;
                        postBackUrl = Regex.Replace(postBackUrl, "clickidref", click.ClickId, RegexOptions.IgnoreCase);
                        postBackUrl = Regex.Replace(postBackUrl, "msisdnref", click.Msisdn, RegexOptions.IgnoreCase);

                        var request = new HttpRequestMessage(HttpMethod.Get, postBackUrl);

                        var client = _httpClientFactory.CreateClient();
                        var response = client.SendAsync(request).Result;
                    }
                }


                var mt = new OutboundMessage
                {
                    Destination = subscriber.Msisdn,
                    CreatedAt = DateTime.Now,
                    OfferCode = subscriber.OfferCode,
                    SendAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // TODO: Make auto response user-controlled.
                mt.Content = subscriber.FirstSubscribedAt == subscriber.LastSubscribedAt ?
                                "Welcome to the service" :
                                "Welcome back to the service";

                //_dbContext.Add(mt);
                _dbContext.SaveChanges();

                //BackgroundJob.Enqueue<Jobs.Messages>((_messagesJob) => _messagesJob.SendToSubscriber(mt.Id));

            }

        }

        public void SendActivationRequest(string msisdn, string offerCode, string shortCode = "")
        {
            _gatewayClient.Send("api/Subscriptions/Activate", 
                ActivationJsonBody(msisdn, offerCode, shortCode)).Wait();
        }

        public void RegisterClick(string clickId, string campaignId)
        {

        }

        private string ActivationJsonBody(string msisdn, string offerCode, string shortCode)
        {
            var requestBody = new
            {
                Msisdn = msisdn,
                OfferCode = offerCode,
                ShortCode = shortCode
            };

            return JsonSerializer.Serialize(requestBody);
        }

    }
}