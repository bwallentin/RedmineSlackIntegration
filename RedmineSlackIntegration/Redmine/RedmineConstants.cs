using System.Diagnostics.CodeAnalysis;

namespace RedmineSlackIntegration.Redmine
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum RedmineProjects
    {
        InkopProdukt = 82,
        AIS = 84,
        DailyBusiness = 95
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum RedmineStatus
    {
        KlarForDev = 2,
        Utveckling = 3,
        Acctest = 7,
        Demo = 14,
        Verifiering = 15,
        Prodsatt = 18
    }
}
