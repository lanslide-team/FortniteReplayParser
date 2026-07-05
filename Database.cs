using FortniteReplayParser.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FortniteReplayParser;

public static class Database
{
    public static ServiceProvider ConnectToDatabase()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();
        var cs = config.GetConnectionString("MySql");

        // Register EF Core with MySQL
        services.AddDbContext<AppDbContext>(options => options.UseMySql(cs, ServerVersion.AutoDetect(cs)));
        return services.BuildServiceProvider();
    }

    public static DateTime? ConvertToLocalTimezone(DateTime dateTime)
    {
        TimeZoneInfo tz;
        try
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById("Australia/Victoria");
        }
        catch (TimeZoneNotFoundException)
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
        }

        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, tz);
    }
}
