using ClipboardHistoryManager.Models;
using Notifications.Wpf;
using System.Collections.ObjectModel;
using System.Windows;

namespace ClipboardHistoryManager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private const int MaxHistorySize = 10;
        public event Action OnCopySuccess;
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ClipboardItem> ClipboardHistory { get; } = new ObservableCollection<ClipboardItem>();

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand CopyItemCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand RemoveItemCommand { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public MainViewModel()
        {
            CopyItemCommand = new RelayCommand(CopyItem);
            RemoveItemCommand = new RelayCommand(RemoveItem);
        }
        #endregion

        #region MemberFunction
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void AddHistoryItem(string text)
        {
            OnCopySuccess?.Invoke();
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
                ClipboardHistory.RemoveAt(ClipboardHistory.Count - 1);

            ClipboardHistory.Insert(0, new ClipboardItem(text));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="history"></param>
        public void LoadHistory(IEnumerable<string> history)
        {
            ClipboardHistory.Clear();
            foreach (var text in history)
            {
                ClipboardHistory.Add(new ClipboardItem(text));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void CopyItem(object? parameter)
        {
            if (parameter is ClipboardItem item)
            {
                try
                {
                    Clipboard.SetText(item.Text);
                    OnCopySuccess?.Invoke();
                    Notify("複製成功", "Notify", NotificationType.Information);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void RemoveItem(object? parameter)
        {
            if (parameter is ClipboardItem item)
                ClipboardHistory.Remove(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        public static void Notify(string message, string title = "Notify", NotificationType type = NotificationType.Information)
        {
            var notificationManager = new NotificationManager();
            notificationManager.Show(new NotificationContent { Title = title, Message = message, Type = type, }, "");
        }
        #endregion
    }
}
