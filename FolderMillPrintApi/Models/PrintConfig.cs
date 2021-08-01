using System.Collections.Generic;

namespace FolderMillPrintApi.Models
{
    public class PrintConfig
    {
        public string HotFolder { get; set; }
        public List<string> Printers { get; set; } = new List<string>();
    }
}
