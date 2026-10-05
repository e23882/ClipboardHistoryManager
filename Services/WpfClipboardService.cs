using System.Windows;

namespace ClipboardHistoryManager.Services
{
    public class WpfClipboardService : IClipboardService
    {
        public void SetText(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
