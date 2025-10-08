
namespace ClipboardHistoryManager.Models
{
    public class ClipboardItem
    {
        public string Text { get; set; }

        public ClipboardItem(string text)
        {
            Text = text;
        }
    }
}
