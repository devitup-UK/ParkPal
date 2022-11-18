using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.Extensions.Logging;
using ParkPal.Common.Database.Contexts;
using ParkPal.Common.Logging.Providers;
using ParkPal.Common.Models.Database.Entities.Log;

namespace ParkPal.Common.Logging;

public class DbLogger: ILogger
{
    private readonly DbLoggerProvider _dbLoggerProvider;
    private DatabaseContext _dbContext;

    public DbLogger([NotNull] DbLoggerProvider dbLoggerProvider, DatabaseContext dbContext)
    {
        _dbLoggerProvider = dbLoggerProvider;
        _dbContext = dbContext;
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
        
        _dbContext.LogItems?.Add(logItem);
    }
}