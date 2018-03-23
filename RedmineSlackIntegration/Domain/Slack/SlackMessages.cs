using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Redmine.Net.Api.Types;
using RedmineSlackIntegration.Domain.Configuration;

namespace RedmineSlackIntegration.Domain.Slack
{
    public static class SlackMessages
    {
        public static string StormIntegrationNumberOfFailedImports()
        {
            var files = new DirectoryInfo(ConfigurationProvider.StormIntegrationFolder).GetFiles().ToList();

            var orderedEnumerable = files.Where(x => x.CreationTime > DateTime.Now.AddHours(-24))
                                         .OrderByDescending(x => x.CreationTime)
                                         .ToList();

            var list = new List<string>
            {
                $"Antal felande filer som försökt läsas in i StormIntegration det senaste dygnet: {orderedEnumerable.Count}"
            };

            return PickOutRandomStringFromList(list);
        }

        public static string StormIntegrationFailedFullFile()
        {
            var files = new DirectoryInfo(ConfigurationProvider.StormIntegrationFolder).GetFiles().ToList();

            var fullfile = files.Where(x => x.CreationTime > DateTime.Now.AddHours(-24) &&
                                            x.FullName.Contains("CreateAndUpdateSkuFull"))
                                .OrderByDescending(x => x.CreationTime)
                                .FirstOrDefault();

            if (fullfile != null)
            {
                var list = new List<string>
                {
                    $"Det finns en full-fil som misslyckats läsa in via StormIntegration senaste dygnet! Fil: {Path.GetFileNameWithoutExtension(fullfile.FullName)}"
                };

                return PickOutRandomStringFromList(list);
            }
            return null;
        }

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

        public static string GetMultipleIssuesProdsattMessage(List<Issue> issues)
        {
            var message = "Följande ärenden blev nyss prodsatta:";

            foreach (var issue in issues)
            {
                var adlisLink = $"<http://adlis/issues/{issue.Id}|#{issue.Id}>";
                message += $"\n{adlisLink}: {issue.Subject}\n";
            }

            return message;
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
                "Ärende prodsatt!",
                "Ärende prodsatt!",
                "Ärende prodsatt!",
                "Ärende prodsatt!",
                "Ärende prodsatt!",
                "Ärende prodsatt!",
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