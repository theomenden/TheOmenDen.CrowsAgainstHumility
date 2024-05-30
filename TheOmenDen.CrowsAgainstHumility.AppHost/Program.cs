var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TheOmenDen_CrowsAgainstHumility_United>("theomenden-crowsagainsthumility-united");

builder.AddProject<Projects.TheOmenDen_CrowsAgainstHumility_Api>("theomenden-crowsagainsthumility-api");

builder.Build().Run();
