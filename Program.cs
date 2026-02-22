using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Serve images from the project-level "Imagenes" folder (allows using /Imagenes/... URLs)
var imagesPath = Path.Combine(builder.Environment.ContentRootPath, "Imagenes");
if (Directory.Exists(imagesPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(imagesPath),
        RequestPath = "/Imagenes"
    });
}

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
