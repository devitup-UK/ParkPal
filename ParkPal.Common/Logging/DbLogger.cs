using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ParkPal.Common.Database.Contexts;
using ParkPal.Common.Logging.Providers;
using ParkPal.Common.Models.Database.Entities.Log;

namespace ParkPal.Common.Logging;

public class DbLogger: ILogger
{
    private readonly DbLoggerProvider _dbLoggerProvider;
    private readonly IConfiguration _configuration;

    public DbLogger([NotNull] DbLoggerProvider dbLoggerProvider, IConfiguration configuration)
    {
        _dbLoggerProvider = dbLoggerProvider;
        _configuration = configuration;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        using DatabaseContext databaseContext = new(_configuration);
        int threadId = Thread.CurrentThread.ManagedThreadId; 

        Item logItem = new()
        {
            ThreadId = threadId,
            LogLevel = logLevel.ToString(),
            EventId = eventId.Id,
            EventName = eventId.Name,
            Message = formatter(state, exception),
            ExceptionMessage = exception?.Message,
            ExceptionStackTrace = exception?.StackTrace,
            ExceptionSource = exception?.Source,
            HostName = System.Environment.MachineName,
            ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
            Created = DateTime.Now,
            Modified = DateTime.Now
        };
        
        databaseContext.LogItems?.Add(logItem);
        databaseContext.SaveChanges();
    }
}