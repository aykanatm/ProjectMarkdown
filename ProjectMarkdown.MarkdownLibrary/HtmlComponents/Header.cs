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
        private readonly string _text;

        public Header(string text, HeaderType headerType) : base(TagTypes.Header)
        {
            _text = text;
            _headerType = headerType;
        }

        public override string ToString()
        {
            switch (_headerType)
            {
                case HeaderType.H1:return "<h1>" + _text + "</h1>";
                case HeaderType.H2: return "<h2>" + _text + "</h2>";
                case HeaderType.H3: return "<h3>" + _text + "</h3>";
                case HeaderType.H4: return "<h4>" + _text + "</h4>";
                case HeaderType.H5: return "<h5>" + _text + "</h5>";
                case HeaderType.H6: return "<h6>" + _text + "</h6>";
                default:
                    return string.Empty;
            }
        }
    }
}
