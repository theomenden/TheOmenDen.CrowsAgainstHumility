namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Settings;
public sealed class CosmosDbSettings
{
    public string ConnectionString { get; set; }
    public string EndpointUrl { get; set; }
    public string PrimaryKey { get; set; }
    public string DatabaseName { get; set; }
    public string ContainerName { get; set; }
    public List<ContainerInfo> Containers { get; set; }
}
