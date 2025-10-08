
using ClipboardHistoryManager.Models;
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

            // Optional: Prevent adding duplicates
            var existing = ClipboardHistory.FirstOrDefault(i => i.Text == text);
            if (existing != null)
            {
                // Move existing to top
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

        private void CopyItem(object? parameter)
        {
            if (parameter is ClipboardItem item)
            {
                try
                {
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
    }
}
