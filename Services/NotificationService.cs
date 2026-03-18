using BakeryApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using UserNamespace.Models;

namespace BakeryNamespace.Services;

/// <summary>
/// Centralized service for handling SignalR notifications
/// Eliminates code duplication in controllers
/// </summary>
public interface INotificationService
{
    Task NotifyItemUpdate(int userId, string username, string userType, string action, string itemName);
}

public class NotificationService : INotificationService
{
    private readonly IHubContext<ItemsHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IHubContext<ItemsHub> hubContext, ILogger<NotificationService> logger)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Send item update notification to user via SignalR
    /// </summary>
    public async Task NotifyItemUpdate(int userId, string username, string userType, string action, string itemName)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userType) || string.IsNullOrEmpty(action) || string.IsNullOrEmpty(itemName))
                throw new ArgumentException("Missing required notification parameters");

            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveItemUpdate", new
                {
                    Username = username,
                    UserId = userId,
                    UserType = userType,
                    Action = action,
                    ItemName = itemName,
                    Timestamp = DateTime.UtcNow
                });

            _logger.LogInformation($"Sent {action} notification to user {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending notification for action {action} to user {userId}");
            throw;
        }
    }
}

public static partial class ServiceExtensions
{
    public static IServiceCollection AddNotificationService(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }
}
