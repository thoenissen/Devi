namespace Devi.ServiceHosts.DTOs.Docker;

/// <summary>
/// Docker container
/// </summary>
public class CreateDockerContainerDTO
{
    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }
}