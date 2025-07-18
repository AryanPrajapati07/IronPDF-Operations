using IronSofts.Services;

IronPdf.License.LicenseKey = "IRONSUITE.ARYAN22.EXCELSIOR.GMAIL.COM.6945-287E510A5A-B5GXCA6OBESTZA-LNO4ODK76XTM-7C6OHA466XIJ-M3QPGDAMR66S-SGQ5HVGNZG2N-TB3NWOQBUGUB-EK4CC7-TAFDRUCFSRWPUA-DEPLOYMENT.TRIAL-BS27OC.TRIAL.EXPIRES.12.JUL.2025";
IronOcr.License.LicenseKey = "IRONSUITE.ARYAN22.EXCELSIOR.GMAIL.COM.6945-287E510A5A-B5GXCA6OBESTZA-LNO4ODK76XTM-7C6OHA466XIJ-M3QPGDAMR66S-SGQ5HVGNZG2N-TB3NWOQBUGUB-EK4CC7-TAFDRUCFSRWPUA-DEPLOYMENT.TRIAL-BS27OC.TRIAL.EXPIRES.12.JUL.2025";
IronXL.License.LicenseKey = "IRONSUITE.ARYAN22.EXCELSIOR.GMAIL.COM.6945-287E510A5A-B5GXCA6OBESTZA-LNO4ODK76XTM-7C6OHA466XIJ-M3QPGDAMR66S-SGQ5HVGNZG2N-TB3NWOQBUGUB-EK4CC7-TAFDRUCFSRWPUA-DEPLOYMENT.TRIAL-BS27OC.TRIAL.EXPIRES.12.JUL.2025";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseCors("AllowAll");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
