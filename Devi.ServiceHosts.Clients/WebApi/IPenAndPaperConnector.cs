using Devi.ServiceHosts.DTOs.PenAndPaper;

namespace Devi.ServiceHosts.Clients.WebApi;

/// <summary>
/// Pen and paper connector
/// </summary>
public interface IPenAndPaperConnector
{
    /// <summary>
    /// Create campaign
    /// </summary>
    /// <param name="dto">Campaign data</param>
    void CreateCampaign(CreateCampaignDTO dto);
}