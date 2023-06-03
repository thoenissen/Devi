namespace Devi.ServiceHosts.WebApi.Data.Entity.Collections.PenAndPaper;

/// <summary>
/// Session with campaign data
/// </summary>
public class SessionWithCampaignEntity : SessionEntity
{
    /// <summary>
    /// Campaign
    /// </summary>
    public CampaignEntity Campaign { get; set; }
}