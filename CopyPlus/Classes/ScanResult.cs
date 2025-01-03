using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyPlus.Classes
{
    public class ScanResult
    {
        //public List<FileInfo> Results { get; set; }
        public int Success { get; set; } = 0;
        public int Failure { get; set; } = 0;
        public int InProgress { get; set; } = 0;
        public int NotStarted { get; set; } = 0;
        public int Exists { get; set; } = 0;
    }
}
