
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
class Program {
    static void Main(string[] args) {
        getTables();
    }
    static void getTables() {

        var cs = getConnectionString();
        Console.WriteLine(cs);
        
    }
    static string getConnectionString()
    {
        var sc = new ServiceCollection();
        IConfiguration configuration;
        configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json")
            .Build();
        return configuration.GetConnectionString("sqliteCS");
    }
}