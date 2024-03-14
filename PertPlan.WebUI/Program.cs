using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Localization;
using PertPlan.WebUI.Models.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "pl" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var localizer = services.GetRequiredService<IStringLocalizerFactory>()
                        .Create(typeof(Validator));
    Validator.SetLocalizer(localizer);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
