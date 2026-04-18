var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DotnetKumaUptimeDemo>("dotnetkumauptimedemo");

builder.Build().Run();
