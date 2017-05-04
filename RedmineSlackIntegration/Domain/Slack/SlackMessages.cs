using System;
using System.Collections.Generic;
using Redmine.Net.Api.Types;

namespace RedmineSlackIntegration.Domain.Slack
{
    public static class SlackMessages
    {
        public static string WipLimitBrokenMessage()
        {
            var list = new List<string>
            {
                "För många ärenden igång. WIP-limiten är bruten!!"
            };

            return PickOutRandomStringFromList(list);

        }
        
        public static string GetNewIssuesRandomMessage(Issue issue)
        {
            var adlisLink = $"<http://adlis/issues/{issue.Id}|#{issue.Id}>";

            var list = new List<string>
            {
                "Nytt ärende redo för utveckling!"
            };

            var randomString = PickOutRandomStringFromList(list);

            return $"{randomString} {adlisLink}: {issue.Subject}";
        }

        public static string GetProdsattRandomMessage(Issue issue)
        {
            var adlisLink = $"<http://adlis/issues/{issue.Id}|#{issue.Id}>";

            var list = new List<string>
            {
                "Nytt ärende prodsatt. Woho!",
                "Ett nytt ärende har blivit prodsatt, trevligt!",
                "Som ni jobbar! Nytt ärende nu ute i prod!",
                "Va grymma ni är, ännu ett nytt ärende ute i prod!",
                "Jösses Amalia! Ännu ett ärende blev precis prodsatt. Väl utfört arbete!",
                "Hoppla, ärende prodsatt!",
                "Wow, ännu ett ärende prodsatt!",
                "Kors i taket! Ett till ärende prodsatt!",
                "Ärende prodsatt!",
                "Ännu ett ärende är nu ute och seglar i produktionsmiljöns vilda vatten!"
            };

            var randomString = PickOutRandomStringFromList(list);

            return $"{randomString} {adlisLink}: {issue.Subject}";
        }

        private static string PickOutRandomStringFromList(IReadOnlyList<string> list)
        {
            var r = new Random();
            var index = r.Next(list.Count);
            var randomString = list[index];

            return randomString;
        }
    }
}
