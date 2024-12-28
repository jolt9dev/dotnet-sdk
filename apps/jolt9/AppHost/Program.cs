var builder = DistributedApplication.CreateBuilder(args);

_ = builder.AddProject<Projects.Jolt9_Api_Service>("api.service");

await builder.Build().RunAsync();