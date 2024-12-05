using Microsoft.Extensions.Configuration;
using System.Windows;

namespace InsuranceSystemDemo;

public partial class App : Application
{
    public static IConfiguration? Configuration { get; private set; }

    //
    // Summary:
    //     Loads the configuration file.
    public App()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Configuration = builder.Build();
    }
}
