using Bottle.Infrastructure.HeaderProtocol;
using Coa.Bottle.Api.DependencyInjectors;
using Coa.Bottle.Api.Dependencys;

var builder = WebApplication.CreateBuilder(args);

var assemblies = new[]
{
    typeof(BaseModuleInstaller).Assembly,
    typeof(ImageHelperInstaller).Assembly
};

builder.Services.InstallServices(builder.Configuration, assemblies);

var app = builder.Build();

app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseHeaderProtocol();
app.MapControllers();
app.UseHsts();

await app.RunAsync();