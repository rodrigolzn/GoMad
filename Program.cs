using GoMad.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrWhiteSpace(connectionString))
{
	builder.Services.AddDbContext<AppDbContext>(options =>
		options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
}

builder.Services.AddRazorPages();

var app = builder.Build();

try
{
	if (!string.IsNullOrWhiteSpace(connectionString))
	{
		using var scope = app.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		await DatabaseInitializer.EnsureDatabaseAsync(dbContext);
	}

	if (!app.Environment.IsDevelopment())
	{
		app.UseExceptionHandler("/Error");
		app.UseHsts();
	}

	app.UseHttpsRedirection();
	app.UseStaticFiles();

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

	Console.WriteLine("Starting web host...");
	await app.RunAsync();
}
catch (Exception ex)
{
	Console.Error.WriteLine("Unhandled exception during app startup:");
	Console.Error.WriteLine(ex.ToString());
	throw;
}
