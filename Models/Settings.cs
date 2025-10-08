using System.Collections.Generic;

namespace ClipboardHistoryManager.Models
{
    public class Settings
    {
        public double WindowTop { get; set; } = 100;
        public double WindowLeft { get; set; } = 100;
        public List<string> History { get; set; } = new List<string>();
    }
}
