using System.Collections.Generic;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class HtmlDocument
    {
        private readonly List<HtmlComponent> _components;

        public HtmlDocument(List<HtmlComponent> components)
        {
            _components = components;
        }

        public override string ToString()
        {
            string output = string.Empty;
            output += "<link rel=\"stylesheet\" href=\"Styles/github-markdown.css\">\r\n" +
                      "<style>.markdown - body {box - sizing: border - box;min - width: 200px;max - width: 980px;margin: 0 auto;padding: 45px;}</style>\r\n" +
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
