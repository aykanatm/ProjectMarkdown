using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.Services
{
    public class SaveResult
    {
        public string FileName { get; set; }
        public Uri Source { get; set; }
        public string TempFile { get; set; }
    }
}
