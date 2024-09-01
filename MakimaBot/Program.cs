using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseKestrel(options =>
            {
                var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
                ArgumentNullException.ThrowIfNull(port);

                options.ListenAnyIP(int.Parse(port));
            });
        })
        .ConfigureAppConfiguration(builder =>
        {
            builder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"));
            builder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "application.config.json"));
            builder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config/config.json"));
            builder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "changelog/changelog.json"));
        })
        .Build();


await builder.RunAsync();
