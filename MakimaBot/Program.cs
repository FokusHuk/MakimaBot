using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

var builder = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseKestrel(options =>
            {
                var port = Environment.GetEnvironmentVariable("PORT");
                ArgumentNullException.ThrowIfNull(port);

                options.ListenAnyIP(int.Parse(port), listentOptions =>
                {
                    listentOptions.UseHttps();
                });
            });
        })
        .Build();

await builder.RunAsync();
