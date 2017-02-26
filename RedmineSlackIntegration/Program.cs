using RedmineSlackIntegration.Bootstrap;

namespace RedmineSlackIntegration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConfigureService.Configure();
        }
    }
}
