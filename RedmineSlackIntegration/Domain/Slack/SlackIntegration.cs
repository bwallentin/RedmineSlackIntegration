using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Redmine.Net.Api.Types;
using RedmineSlackIntegration.Domain.Redmine;

namespace RedmineSlackIntegration.Domain.Slack
{
    public interface ISlackClient
    {
        void PostIssuesToSlack(IEnumerable<Issue> issues);
        void PostDailyBusinessWarningToSlack();
    }

    public class SlackClient : ISlackClient
    {
        private readonly Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();

        public SlackClient()
        {
            var uriString = @"https://hooks.slack.com/services/" + ConfigurationProvider.SlackHook;
            _uri = new Uri(uriString);
        }

        public void PostIssuesToSlack(IEnumerable<Issue> issues)
        {
            foreach (var issue in issues)
            {
                switch (issue.Status.Id)
                {
                    case (int)RedmineStatus.Prodsatt:
                        PostMessage(SlackMessages.GetProdsattRandomMessage(issue));
                        continue;
                    case (int)RedmineStatus.KlarForDev:
                        PostMessage(SlackMessages.GetReadyForDevRandomMessage(issue));
                        continue;
                }
            }
        }

        public void PostDailyBusinessWarningToSlack()
        {
            PostMessage(SlackMessages.GetTooMuchDailyBusinessMessage());
        }

        private void PostMessage(string text, string username = null, string channel = null)
        {
            var payload = new Payload
            {
                Channel = channel,
                Username = username,
                Text = text
            };

            PostMessage(payload);
        }

        private void PostMessage(Payload payload)
        {
            var payloadJson = JsonConvert.SerializeObject(payload);

            using (var client = new WebClient())
            {
                var data = new NameValueCollection
                {
                    ["payload"] = payloadJson
                };

                var response = client.UploadValues(_uri, "POST", data);
                var responseText = _encoding.GetString(response);
            }
        }
    }

    internal class Payload
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
