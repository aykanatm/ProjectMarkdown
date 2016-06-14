using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Header : HtmlComponent
    {
        public enum HeaderType
        {
            H1,H2,H3,H4,H5,H6
        }

        private readonly HeaderType _headerType;

        public Header(string text, HeaderType headerType) : base(text, TagTypes.Header)
        {
            _headerType = headerType;
        }

        public override string ToString()
        {
            switch (_headerType)
            {
                case HeaderType.H1:return "<h1>" + Text + "</h1>";
                case HeaderType.H2: return "<h2>" + Text + "</h2>";
                case HeaderType.H3: return "<h3>" + Text + "</h3>";
                case HeaderType.H4: return "<h4>" + Text + "</h4>";
                case HeaderType.H5: return "<h5>" + Text + "</h5>";
                case HeaderType.H6: return "<h6>" + Text + "</h6>";
                default:
                    return string.Empty;
            }
        }
    }
}
