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
                var currentLine = lines[i];
                
                if (currentLine == string.Empty)
                {
                    // Do nothing
                }
                else if (currentLine.StartsWith("###### "))
                {
                    currentLine = currentLine.Replace("#", "").Trim();
                    htmlComponents.Add(new Header(currentLine, Header.HeaderType.H6));
                }
                else if (currentLine.StartsWith("##### "))
                {
                    currentLine = currentLine.Replace("#", "").Trim();
                    htmlComponents.Add(new Header(currentLine, Header.HeaderType.H5));
                }
                else if (currentLine.StartsWith("#### "))
                {
                    currentLine = currentLine.Replace("#", "").Trim();
                    htmlComponents.Add(new Header(currentLine, Header.HeaderType.H4));
                }
                else if (currentLine.StartsWith("### "))
                {
                    currentLine = currentLine.Replace("#", "").Trim();
                    htmlComponents.Add(new Header(currentLine, Header.HeaderType.H3));
                }
                else if (currentLine.StartsWith("## "))
                {
                    currentLine = currentLine.Replace("#", "").Trim();
                    htmlComponents.Add(new Header(currentLine, Header.HeaderType.H2));
                }
                else if (currentLine.StartsWith("# "))
                {
                    currentLine = currentLine.Replace("#", "").Trim();
                    htmlComponents.Add(new Header(currentLine, Header.HeaderType.H1));
                }
                else if (currentLine.StartsWith("> "))
                {
                    var bqo = GenerateBlockquote(i, lines);
                    htmlComponents.Add(new Blockquote(bqo.Text));
                    i += bqo.NbLines;
                }
                else if (currentLine.StartsWith("***") || currentLine.StartsWith("---") ||
                         currentLine.StartsWith("***") || currentLine.StartsWith("___"))
                {
                    htmlComponents.Add(new HorizontalRule());
                }
                else if (currentLine.StartsWith("- ") || currentLine.StartsWith("* ") || currentLine.StartsWith("+ "))
                {
                    var unorderedList = new List(GetListItems(i, lines, isCallerSublist:false), List.ListTypes.Unordered);
                    htmlComponents.Add(unorderedList);
                    i += unorderedList.Count;
                }
                else if (Regex.IsMatch(currentLine,@"^\d+\. "))
                {
                    var orderedList = new List(GetListItems(i,lines,isCallerSublist:false), List.ListTypes.Ordered);
                    htmlComponents.Add(orderedList);
                    i += orderedList.Count;
                }
                else if (currentLine.StartsWith("```"))
                {
                    var codeBlock = GetCodeBlock(i, lines);
                    var language = currentLine.Substring(3);
                    
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
                else if (currentLine.StartsWith("!["))
                {
                    var imageAltText = new string(currentLine.SkipWhile(s => s != '[').Skip(1).TakeWhile(s => s != ']').ToArray()).Trim();
                    var imageUrl = new string(currentLine.SkipWhile(s => s != '(').Skip(1).TakeWhile(s => s != ')').ToArray()).Trim();
                    htmlComponents.Add(new Image(imageUrl, imageAltText));
                }
                else if (currentLine.StartsWith("[!["))
                {
                    htmlComponents.Add(new RawHtml(currentLine.GenerateInlineImages()
                                                              .GenerateHtmlLinks()));
                }
                else
                {
                    // Code should be the last because it strips all html tags from its content
                    
                    currentLine = currentLine.ConvertPairedMarkdownToHtml("**", PairedMarkdownTags.Bold)
                        .ConvertPairedMarkdownToHtml("__", PairedMarkdownTags.Bold)
                        .ConvertPairedMarkdownToHtml("*",PairedMarkdownTags.Italic)
                        .ConvertPairedMarkdownToHtml("_", PairedMarkdownTags.Italic)
                        .ConvertPairedMarkdownToHtml("~~",PairedMarkdownTags.StrikeThrough)
                        .ConvertPairedMarkdownToHtml("`", PairedMarkdownTags.InlineCode)
                        .GenerateInlineImages()
                        .GenerateHtmlLinks();
                    
                    htmlComponents.Add(new Paragraph(currentLine));
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
                output += lines[currentIndex].Substring(1).Trim();
                if (string.IsNullOrEmpty(lines[currentIndex]))
                {
                    break;
                }
                currentIndex += 1;
                nbLines += 1;
            }
            return new BlockquoteOutput(output,nbLines);
        }
        private List<HtmlComponent> GetListItems(int currentIndex, string[] lines, bool isCallerSublist)
        {
            var itemList = new List<HtmlComponent>();
            
            if (!isCallerSublist)
            {
                while (true)
                {
                    if (lines[currentIndex].StartsWith("- ") || lines[currentIndex].StartsWith("* ") ||
                        lines[currentIndex].StartsWith("+ ") || Regex.IsMatch(lines[currentIndex], @"^\d+\. "))
                    {
                        itemList.Add(new ListItem(lines[currentIndex].Substring(2).Trim()));
                        currentIndex += 1;
                    }
                    else if (lines[currentIndex].StartsWith("  - ") || lines[currentIndex].StartsWith("  * ") || lines[currentIndex].StartsWith("  + "))
                    {
                        var list = new List(GetListItems(currentIndex, lines, isCallerSublist:true), List.ListTypes.Unordered);
                        itemList.Add(list);
                        currentIndex += list.Count;
                    }
                    else if (Regex.IsMatch(lines[currentIndex], @"^\s{2}\d+\. "))
                    {
                        var list = new List(GetListItems(currentIndex, lines, isCallerSublist:true), List.ListTypes.Ordered);
                        itemList.Add(list);
                        currentIndex += list.Count;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                while (true)
                {
                    if (lines[currentIndex].StartsWith("  - ") || lines[currentIndex].StartsWith("  * ") ||
                        lines[currentIndex].StartsWith("  + ") || Regex.IsMatch(lines[currentIndex], @"^\s{2}\d+\. "))
                    {
                        itemList.Add(new ListItem(lines[currentIndex].Substring(4).Trim()));
                        currentIndex += 1;
                    }
                    else
                    {
                        break;
                    }
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
