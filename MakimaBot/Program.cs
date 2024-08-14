using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

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
        .Build();

await builder.RunAsync();
