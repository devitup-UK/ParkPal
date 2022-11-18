using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ParkPal.Common.Database.Contexts;

namespace ParkPal.Common.Logging.Providers;

[ProviderAlias("Database")]
public class DbLoggerProvider: ILoggerProvider
{
    private DatabaseContext _dbContext;
    private IConfiguration _configuration;
    
    public DbLoggerProvider(IConfiguration configuration)
    {
        _configuration = configuration;
        _dbContext = new DatabaseContext(_configuration);

    }

    public ILogger CreateLogger(string categoryName)
    {
        return new DbLogger(this, _dbContext);
    }

    public void Dispose()
    {
        
    }
}