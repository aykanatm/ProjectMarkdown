using System.Linq;
using System.Text.RegularExpressions;
using ProjectMarkdown.MarkdownLibrary.ExtensionMethods;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Table : HtmlComponent
    {
        private readonly string _headers;
        private readonly string _body;

        public int RowCount { get;}

        public Table(string[] headers, string[] lines, int currentIndex) : base(null, TagTypes.Table)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                _headers += "<th>" + headers[i] + "</th>";
            }
            RowCount += 1;
            currentIndex += 1;
            while (true)
            {
                if (currentIndex != lines.Length)
                {
                    if (Regex.IsMatch(lines[currentIndex], ".\\|.") && lines[currentIndex].Contains("---"))
                    {
                        currentIndex += 1;
                    }
                    else if (Regex.IsMatch(lines[currentIndex], ".\\|."))
                    {
                        var row = "<tr>";

                        var items = lines[currentIndex].Split('|').Where(item => item != "").ToArray();
                        for (int i = 0; i < items.Length; i++)
                        {
                            row += "<th>" + items[i].ConvertMarkdownToHtml() + "</th>";
                        }
                        row += "</tr>";
                        _body += row;

                        RowCount += 1;
                        currentIndex += 1;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public override string ToString()
        {
            return "<table><thead>" + _headers + "</thead><tbody>" + _body +"</tbody></table>";
        }
    }
}
