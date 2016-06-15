using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProjectMarkdown.MarkdownLibrary.ExtensionMethods;
using ProjectMarkdown.MarkdownLibrary.HtmlComponents;

namespace ProjectMarkdown.MarkdownLibrary
{
    public class MarkdownParser
    {
        private string _currentLine;
        public enum PairedMarkdownTags
        {
            Bold, Italic, InlineCode, StrikeThrough
        }
        public string Parse(string markdownString)
        {
            var htmlComponents = new List<HtmlComponent>();

            markdownString = markdownString.Replace("\r", "");
            var lines = markdownString.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                _currentLine = lines[i];
                
                if (_currentLine == string.Empty)
                {
                    // Do nothing
                }
                else if (_currentLine.StartsWith("###### "))
                {
                    _currentLine = _currentLine.Replace("#", "").Trim();
                    htmlComponents.Add(new Header(_currentLine, Header.HeaderType.H6));
                }
                else if (_currentLine.StartsWith("##### "))
                {
                    _currentLine = _currentLine.Replace("#", "").Trim();
                    htmlComponents.Add(new Header(_currentLine, Header.HeaderType.H5));
                }
                else if (_currentLine.StartsWith("#### "))
                {
                    _currentLine = _currentLine.Replace("#", "").Trim();
                    htmlComponents.Add(new Header(_currentLine, Header.HeaderType.H4));
                }
                else if (_currentLine.StartsWith("### "))
                {
                    _currentLine = _currentLine.Replace("#", "").Trim();
                    htmlComponents.Add(new Header(_currentLine, Header.HeaderType.H3));
                }
                else if (_currentLine.StartsWith("## "))
                {
                    _currentLine = _currentLine.Replace("#", "").Trim();
                    htmlComponents.Add(new Header(_currentLine, Header.HeaderType.H2));
                }
                else if (_currentLine.StartsWith("# "))
                {
                    _currentLine = _currentLine.Replace("#", "").Trim();
                    htmlComponents.Add(new Header(_currentLine, Header.HeaderType.H1));
                }
                else if (_currentLine.StartsWith("> "))
                {
                    var bqo = GenerateBlockquote(i, lines);
                    htmlComponents.Add(new Blockquote(bqo.Text));
                    i += bqo.NbLines;
                }
                else if (_currentLine.StartsWith("***") || _currentLine.StartsWith("---") ||
                         _currentLine.StartsWith("***") || _currentLine.StartsWith("___"))
                {
                    htmlComponents.Add(new HorizontalRule());
                }
                else if (_currentLine.StartsWith("- ") || _currentLine.StartsWith("* ") || _currentLine.StartsWith("+ "))
                {
                    var unorderedList = new List(GetListItems(i, lines), List.ListTypes.Unordered);
                    htmlComponents.Add(unorderedList);
                    i += unorderedList.Count;
                }
                else if (Regex.IsMatch(_currentLine,@"^\d+\. "))
                {
                    var orderedList = new List(GetListItems(i,lines), List.ListTypes.Ordered);
                    htmlComponents.Add(orderedList);
                    i += orderedList.Count;
                }
                else if (_currentLine.StartsWith("```"))
                {
                    var codeBlock = GetCodeBlock(i, lines);
                    var language = _currentLine.Substring(3);
                    
                    if (language == "javascript")
                    {
                        htmlComponents.Add(new BlockCode(codeBlock.Code, BlockCode.Highlight.Javascript));
                    }
                    else if (language == "python")
                    {
                        htmlComponents.Add(new BlockCode(codeBlock.Code, BlockCode.Highlight.Python));
                    }
                    else
                    {
                        htmlComponents.Add(new BlockCode(codeBlock.Code, BlockCode.Highlight.NoHighlight));
                    }
                    
                    i = codeBlock.LastIndex;
                }
                else if (_currentLine.StartsWith("!["))
                {
                    var imageAltText = new string(_currentLine.SkipWhile(s => s != '[').Skip(1).TakeWhile(s => s != ']').ToArray()).Trim();
                    var imageUrl = new string(_currentLine.SkipWhile(s => s != '(').Skip(1).TakeWhile(s => s != ')').ToArray()).Trim();
                    htmlComponents.Add(new Image(imageUrl, imageAltText));
                }
                else
                {
                    // Code should be the last because it strips all html tags from its content
                    
                    _currentLine = _currentLine.ConvertPairedMarkdownToHtml("**", PairedMarkdownTags.Bold)
                        .ConvertPairedMarkdownToHtml("__", PairedMarkdownTags.Bold)
                        .ConvertPairedMarkdownToHtml("*",PairedMarkdownTags.Italic)
                        .ConvertPairedMarkdownToHtml("_", PairedMarkdownTags.Italic)
                        .ConvertPairedMarkdownToHtml("~~",PairedMarkdownTags.StrikeThrough)
                        .ConvertPairedMarkdownToHtml("`", PairedMarkdownTags.InlineCode)
                        .GenerateHtmlLinks();
                    
                    htmlComponents.Add(new Paragraph(_currentLine));
                }
            }
            

            var htmlDocument = new HtmlDocument(htmlComponents);

            return htmlDocument.ToString();
        }

        struct BlockquoteOutput
        {
            public readonly string Text;
            public readonly int NbLines;

            public BlockquoteOutput(string text, int nbLines)
            {
                Text = text;
                NbLines = nbLines;
            }
        }
        private BlockquoteOutput GenerateBlockquote(int currentIndex, string[] lines)
        {
            var output = string.Empty;
            int nbLines = 0;
            while (lines[currentIndex].StartsWith("> "))
            {
                output += _currentLine.Substring(1).Trim();
                if (string.IsNullOrEmpty(lines[currentIndex]))
                {
                    break;
                }
                currentIndex += 1;
                nbLines += 1;
            }
            return new BlockquoteOutput(output,nbLines);
        }
        private List<HtmlComponent> GetListItems(int currentIndex, string[] lines)
        {
            var itemList = new List<HtmlComponent>();
            while (lines[currentIndex].StartsWith("- ") || lines[currentIndex].StartsWith("* ") || lines[currentIndex].StartsWith("+ ") || Regex.IsMatch(_currentLine, @"^\d+\. "))
            {
                itemList.Add(new ListItem(lines[currentIndex].Substring(2).Trim()));
                currentIndex += 1;
                if (string.IsNullOrEmpty(lines[currentIndex]))
                {
                    break;
                }
            }

            return itemList;
        }

        private struct CodeBlock
        {
            public readonly string Code;
            public readonly int LastIndex;

            public CodeBlock(string code, int lastIndex)
            {
                Code = code;
                LastIndex = lastIndex;
            }
        }
        private CodeBlock GetCodeBlock(int currentIndex, string[] lines)
        {
            string code = string.Empty;
            currentIndex += 1;

            while (lines[currentIndex] != "```")
            {
                if (lines[currentIndex + 1] != "```")
                {
                    code += lines[currentIndex] + "\r\n";
                }
                else
                {
                    code += lines[currentIndex];
                }
                
                currentIndex += 1;
            }

            return new CodeBlock(code, currentIndex);
        }
    }
}
