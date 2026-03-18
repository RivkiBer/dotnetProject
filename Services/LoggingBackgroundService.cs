using System.Threading.Channels;
using BakeryNamespace.Models;
using Microsoft.Extensions.Hosting;

namespace BakeryNamespace.Services
{
    public class LoggingBackgroundService : BackgroundService
    {
        private readonly Channel<LogMessage> _channel;
        private readonly ILogger<LoggingBackgroundService> _logger;

        public LoggingBackgroundService(Channel<LogMessage> logChannel, ILogger<LoggingBackgroundService> logger)
        {
            _channel = logChannel;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await foreach (var message in _channel.Reader.ReadAllAsync(stoppingToken))
                {
                    try
                    {
                        var logsDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                        Directory.CreateDirectory(logsDir);
                        var logFile = Path.Combine(logsDir, $"logs_{DateTime.Now:yyyy-MM-dd}.txt");
                        var logLine = $"{message.StartTime:yyyy-MM-dd HH:mm:ss} | {message.Controller}.{message.Action} | User: {message.User} | Duration: {message.DurationMs}ms";
                        await File.AppendAllTextAsync(logFile, logLine + Environment.NewLine, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error writing log message");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Logging background service was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in logging background service");
            }
        }
    }
}