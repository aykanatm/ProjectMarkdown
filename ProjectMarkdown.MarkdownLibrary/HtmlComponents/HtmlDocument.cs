using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class HtmlDocument
    {
        private readonly List<HtmlComponent> _components;
        private readonly string _style;

        public HtmlDocument(List<HtmlComponent> components, string style)
        {
            _components = components;
            _style = style;
        }

        public override string ToString()
        {
            string output = string.Empty;
            
            output += "<style>" + _style + "</style>\r\n" +
                      "<article class=\"markdown-body\">\r\n";
            
            for (int i = 0; i < _components.Count; i++)
            {
                output += _components[i].ToString();
                output += "\r\n";
            }

            output += "</article>";

            return output;
        }
    }
}
