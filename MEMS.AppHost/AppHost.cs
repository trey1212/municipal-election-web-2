using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddAzureSqlServer("sql")
                 .RunAsContainer(); // Runs in Docker locally, connects to Azure SQL in Azure
var memsDb = sql.AddDatabase("memsdb");

var api = builder.AddProject<Projects.MEMS_Api>("mems-api")
                 .WithReference(memsDb)
                 .WaitFor(memsDb)
                 .WithExternalHttpEndpoints();

// builder.AddProject<Projects.MEMS_Admin>("mems-admin")
//        .WithReference(memsDb)
//        .WithReference(api)
//        .WaitFor(memsDb);

// builder.AddProject<Projects.MEMS_Public>("mems-public")
//        .WithReference(api)
//        .WaitFor(api);

builder.Build().Run();