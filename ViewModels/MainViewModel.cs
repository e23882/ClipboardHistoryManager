
using ClipboardHistoryManager.Models;
using Notifications.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ClipboardHistoryManager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const int MaxHistorySize = 10;
        public ObservableCollection<ClipboardItem> ClipboardHistory { get; } = new ObservableCollection<ClipboardItem>();

        public RelayCommand CopyItemCommand { get; }
        public RelayCommand RemoveItemCommand { get; }

        public MainViewModel()
        {
            CopyItemCommand = new RelayCommand(CopyItem);
            RemoveItemCommand = new RelayCommand(RemoveItem);
        }

        public void AddHistoryItem(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var existing = ClipboardHistory.FirstOrDefault(i => i.Text == text);
            if (existing != null)
            {
                ClipboardHistory.Remove(existing);
                ClipboardHistory.Insert(0, existing);
                return;
            }

            if (ClipboardHistory.Count >= MaxHistorySize)
            {
                ClipboardHistory.RemoveAt(ClipboardHistory.Count - 1);
            }

            ClipboardHistory.Insert(0, new ClipboardItem(text));
        }

        public void LoadHistory(IEnumerable<string> history)
        {
            ClipboardHistory.Clear();
            foreach (var text in history)
            {
                ClipboardHistory.Add(new ClipboardItem(text));
            }
        }

        private void CopyItem(object? parameter)
        {
            if (parameter is ClipboardItem item)
            {
                try
                {
                    Notify("複製成功", "Notify", NotificationType.Information);
                    Clipboard.SetText(item.Text);
                }
                catch
                {
                    // Could show an error to the user
                }
            }
        }

        private void RemoveItem(object? parameter)
        {
            if (parameter is ClipboardItem item)
            {
                ClipboardHistory.Remove(item);
            }
        }

        public static void Notify(string message, string title = "Notify", NotificationType type = NotificationType.Information)
        {
            var notificationManager = new NotificationManager();
            notificationManager.Show(new NotificationContent { Title = title, Message = message, Type = type, }, "");
        }
    }
}
