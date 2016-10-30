using System;

namespace ProjectMarkdown.Services
{
    public class SaveResult
    {
        public string FileName { get; set; }
        public Uri Source { get; set; }
        public string TempFile { get; set; }
    }
}
