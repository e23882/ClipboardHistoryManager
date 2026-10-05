using Notifications.Wpf;

namespace ClipboardHistoryManager.Services
{
    public class WpfNotificationService : INotificationService
    {
        private readonly NotificationManager _notificationManager = new NotificationManager();

        public void ShowNotification(string message, string title)
        {
            _notificationManager.Show(new NotificationContent { Title = title, Message = message, Type = NotificationType.Information }, "");
        }
    }
}
