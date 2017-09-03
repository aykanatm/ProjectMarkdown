using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public string Parse(string markdownString, string style)
        {
            try
            {
                if (!string.IsNullOrEmpty(markdownString))
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
                        else if (currentLine.StartsWith("- ") || currentLine.StartsWith("* ") || currentLine.StartsWith("+ "))
                        {
                            var unorderedList = new List(GetListItems(i, lines, isCallerSublist: false), List.ListTypes.Unordered);
                            htmlComponents.Add(unorderedList);
                            i += unorderedList.Count;
                        }
                        else if (Regex.IsMatch(currentLine, @"^\d+\. "))
                        {
                            var orderedList = new List(GetListItems(i, lines, isCallerSublist: false), List.ListTypes.Ordered);
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
                        else if (Regex.IsMatch(currentLine, ".\\|."))
                        {
                            if (i + 1 < lines.Length)
                            {
                                var nextLine = lines[i + 1];
                                if (Regex.IsMatch(nextLine, "[---]*\\|[---]*"))
                                {
                                    var headers = currentLine.Split('|').Where(h => h != "").ToArray();
                                    for (int j = 0; j < headers.Length; j++)
                                    {
                                        headers[j] = headers[j].Trim();
                                    }
                                    var table = new Table(headers, lines, i);
                                    htmlComponents.Add(table);
                                    i += table.RowCount;
                                }
                                else
                                {
                                    currentLine = currentLine.ConvertMarkdownToHtml();
                                    htmlComponents.Add(new Paragraph(currentLine));
                                }
                            }
                            else
                            {
                                currentLine = currentLine.ConvertMarkdownToHtml();
                                htmlComponents.Add(new Paragraph(currentLine));
                            }
                        }
                        else if (currentLine.StartsWith("***") || currentLine.StartsWith("---") ||
                                 currentLine.StartsWith("***") || currentLine.StartsWith("___"))
                        {
                            htmlComponents.Add(new HorizontalRule());
                        }
                        else
                        {
                            currentLine = currentLine.ConvertMarkdownToHtml();
                            htmlComponents.Add(new Paragraph(currentLine));
                        }
                    }


                    var htmlDocument = new HtmlDocument(htmlComponents, style);

                    return htmlDocument.ToString();
                }
                return String.Empty;
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while parsing the markdown string. " + e.Message);
            }
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

        private static BlockquoteOutput GenerateBlockquote(int currentIndex, string[] lines)
        {
            var output = string.Empty;
            int nbLines = 0;
            while (currentIndex < lines.Length && lines[currentIndex].StartsWith("> "))
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
            try
            {
                var itemList = new List<HtmlComponent>();

                if (!isCallerSublist)
                {
                    while (true)
                    {
                        if (currentIndex >= lines.Length)
                        {
                            break;
                        }

                        if (lines[currentIndex].StartsWith("- ") || lines[currentIndex].StartsWith("* ") ||
                            lines[currentIndex].StartsWith("+ ") || Regex.IsMatch(lines[currentIndex], @"^\d+\. "))
                        {
                            itemList.Add(new ListItem(lines[currentIndex].Substring(2).Trim()));
                            currentIndex += 1;
                        }
                        else if (lines[currentIndex].StartsWith("  - ") || lines[currentIndex].StartsWith("  * ") || lines[currentIndex].StartsWith("  + "))
                        {
                            var list = new List(GetListItems(currentIndex, lines, isCallerSublist: true), List.ListTypes.Unordered);
                            itemList.Add(list);
                            currentIndex += list.Count;
                        }
                        else if (Regex.IsMatch(lines[currentIndex], @"^\s{2}\d+\. "))
                        {
                            var list = new List(GetListItems(currentIndex, lines, isCallerSublist: true), List.ListTypes.Ordered);
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
            catch (Exception e)
            {
                throw new Exception("An error occured while retrieving HTML components. " + e.Message);
            }
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

        private static CodeBlock GetCodeBlock(int currentIndex, string[] lines)
        {
            try
            {
                string code = string.Empty;
                currentIndex += 1;

                if (currentIndex < lines.Length)
                {
                    while (currentIndex < lines.Length && lines[currentIndex] != "```")
                    {
                        if ((currentIndex + 1) < lines.Length && lines[currentIndex + 1] != "```")
                        {
                            code += lines[currentIndex] + "\r\n";
                        }
                        else
                        {
                            code += lines[currentIndex];
                        }

                        currentIndex += 1;
                    }
                }

                return new CodeBlock(code, currentIndex);
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while retrieving code block. " + e.Message);
            }
        }
    }
}
