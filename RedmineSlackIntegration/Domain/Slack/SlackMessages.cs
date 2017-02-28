using System;
using System.Collections.Generic;
using Redmine.Net.Api.Types;

namespace RedmineSlackIntegration.Domain.Slack
{
    public static class SlackMessages
    {
        public static string GetTooMuchDailyBusinessMessage()
        {
            var list = new List<string>
            {
                "Nu har ni för många daily business ärenden igång!!",
                "Jösses!! Nu har ni för mycket daily business igång igen",
                "Åh nej! Så mycket daily bussiness ni arbetar med",
                "För många daily business ärenden igång!!",
                "Hej vänner. Jag vet att ni älskar daily business. Men inte så mycket igång samtidigt tack!",
                "Tjena <@slackbot>, ska vi ta en fika medans teamet minskar antal daily business de arbetar med?"
            };

            return PickOutRandomStringFromList(list);
        }

        public static string GetReadyForDevRandomMessage(Issue issue)
        {
            var adlisLink = $"<http://adlis/issues/{issue.Id}|#{issue.Id}>";

            var list = new List<string>
            {
                "Nytt ärende redo för utveckling.",
                "Hoppas ni är redo att arbeta, för nu finns ett nytt ärende redo för utveckling!",
                "Vässa hjärnorna och fingrarna, nu har vi ett nytt ärende redo att hugga tag i!"
            };

            var randomString = PickOutRandomStringFromList(list);

            return $"{randomString} {adlisLink}: {issue.Subject}";
        }

        public static string GetProdsattRandomMessage(Issue issue)
        {
            var adlisLink = $"<http://adlis/issues/{issue.Id}|#{issue.Id}>";

            var list = new List<string>
            {
                "Nytt ärende prodsatt",
                "Sedärja, ett nytt ärende har precis blivit prodsatt.",
                "Ett nytt ärende har blivit prodsatt, trevligt!",
                "Som ni jobbar! Nytt ärende nu ute i prod!",
                "Va grymma ni är, ännu ett nytt ärende ute i prod!"
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
