using Microsoft.Azure.Cosmos;
using Translator2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<TranslatorService>();

builder.Services.AddSingleton<CosmosService>();

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var connectionString = config["Cosmos:ConnectionString"];

    return new CosmosClient(connectionString);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Translator}/{action=Index}/{id?}");

app.Run();