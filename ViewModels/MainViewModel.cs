using ClipboardHistoryManager.Models;
using ClipboardHistoryManager.Services;
using System.Collections.ObjectModel;

namespace ClipboardHistoryManager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private const int MaxHistorySize = 10;
        private readonly IClipboardService _clipboardService;
        private readonly INotificationService _notificationService;
        public event Action OnCopySuccess;
        #endregion

        #region Properties
        public ObservableCollection<ClipboardItem> ClipboardHistory { get; } = new ObservableCollection<ClipboardItem>();
        public RelayCommand CopyItemCommand { get; }
        public RelayCommand RemoveItemCommand { get; }
        #endregion

        #region Constructor
        public MainViewModel(IClipboardService clipboardService, INotificationService notificationService)
        {
            _clipboardService = clipboardService;
            _notificationService = notificationService;
            CopyItemCommand = new RelayCommand(CopyItem);
            RemoveItemCommand = new RelayCommand(RemoveItem);
        }
        #endregion

        #region MemberFunction
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
                    _clipboardService.SetText(item.Text);
                    OnCopySuccess?.Invoke();
                    _notificationService.ShowNotification("複製成功", "Notify");
                }
                catch
                {
                }
            }
        }

        private void RemoveItem(object? parameter)
        {
            if (parameter is ClipboardItem item)
                ClipboardHistory.Remove(item);
        }
        #endregion
    }
}
