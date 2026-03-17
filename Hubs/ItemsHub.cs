using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace BakeryApp.Hubs
{
    /// <summary>
    /// Hub ל-SignalR עבור עדכונים בזמן אמת של פריטים (מאפים)
    /// שולח עדכונים רק למשתמש שביצע את הפעולה, לכל החיבורים שלו (טאבים/מכשירים)
    /// </summary>
    [Authorize]
    public class ItemsHub : Hub
    {
        // Dictionary<userId, HashSet<connectionIds>>
        private static readonly Dictionary<string, HashSet<string>> UserConnections = new();

        public ItemsHub()
        {
        }

        /// <summary>
        /// חיבור חדש ל-Hub
        /// </summary>
        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier; // מזהה המשתמש מ-SignalR Claims
            if (!string.IsNullOrEmpty(userId))
            {
                lock (UserConnections)
                {
                    if (!UserConnections.ContainsKey(userId))
                        UserConnections[userId] = new HashSet<string>();

                    UserConnections[userId].Add(Context.ConnectionId);
                }
            }

            return base.OnConnectedAsync();
        }

        /// <summary>
        /// ניתוק חיבור מה-Hub
        /// </summary>
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                lock (UserConnections)
                {
                    if (UserConnections.ContainsKey(userId))
                    {
                        UserConnections[userId].Remove(Context.ConnectionId);
                        if (UserConnections[userId].Count == 0)
                            UserConnections.Remove(userId);
                    }
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// שליחת עדכון פריט ספציפי למשתמש
        /// </summary>
        /// <param name="userId">ID של המשתמש שמבצע את הפעולה</param>
        /// <param name="action">"add", "update", "delete"</param>
        /// <param name="itemName">שם המאפה</param>
        public async Task NotifyItemUpdate(string userId, string action, string itemName, string username)
        {
            if (string.IsNullOrEmpty(userId) || !UserConnections.ContainsKey(userId))
                return;

            var connectionIds = UserConnections[userId].ToList();
            if (connectionIds.Count == 0)
                return;

            await Clients.Clients(connectionIds).SendAsync("ReceiveItemUpdate", new
            {
                UserId = userId,
                Username = username,
                Action = action,
                ItemName = itemName,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}