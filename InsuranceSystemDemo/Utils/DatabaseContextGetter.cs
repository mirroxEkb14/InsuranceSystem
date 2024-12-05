using InsuranceSystemDemo.Database;
using Microsoft.EntityFrameworkCore;

namespace InsuranceSystemDemo.Utils;

//
// Summary:
//     Contains the only method to get the database context options (is used to connect to the DB).
public static class DatabaseContextGetter
{
    public static DbContextOptions<DatabaseContext> GetDatabaseContextOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        if (App.Configuration != null)
            optionsBuilder.UseOracle(App.Configuration[MessageContainer.ConnectionStringKey]);
        return optionsBuilder.Options;
    }
}
