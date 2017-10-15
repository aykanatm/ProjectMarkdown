using System;
using System.Text.RegularExpressions;
using LogUtils;

namespace ProjectMarkdown.Services
{
    public static class TextFormatter
    {
        public enum TextFormats
        {
            BlockCode,
            BlockQuote,
            Bold,
            Heading1,
            Heading2,
            Heading3,
            Heading4,
            Heading5,
            Heading6,
            InlineCode,
            Italic,
            OrderedList,
            UnorderedList,
            StrikeThrough
        }

        public static string Format(string input, TextFormats textFormat)
        {
            Logger.GetInstance().Debug("Format() >>");
            var formattedText = "";

            try
            {
                switch (textFormat)
                {
                    case TextFormats.BlockCode:
                    {
                        input = input.Replace("\r", "");
                        var lines = input.Split('\n');
                        if (lines[0].StartsWith("```") && lines[lines.Length - 1].EndsWith("```"))
                        {
                            for (var i = 1; i < lines.Length - 1; i++)
                            {
                                if (i != lines.Length - 2)
                                {
                                    formattedText += lines[i] + "\r\n";
                                }
                                else
                                {
                                    formattedText += lines[i];
                                }
                            }
                        }
                        else
                        {
                            formattedText += "```\r\n";
                            for (var i = 0; i < lines.Length; i++)
                            {
                                formattedText += lines[i] + "\r\n";
                            }
                            formattedText += "```";
                        }
                        break;
                    }
                    case TextFormats.BlockQuote:
                    {
                        input = input.Replace("\r", "");
                        var lines = input.Split('\n');

                        for (var i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].StartsWith(">"))
                            {
                                if (i != lines.Length - 1)
                                {
                                    formattedText += lines[i].Substring(1, lines[i].Length - 1).Trim() + "\r\n";
                                }
                                else
                                {
                                    formattedText += lines[i].Substring(1, lines[i].Length - 1).Trim();
                                }
                            }
                            else
                            {
                                if (i != lines.Length - 1)
                                {
                                    formattedText += "> " + lines[i].Trim() + "\r\n";
                                }
                                else
                                {
                                    formattedText += "> " + lines[i].Trim();
                                }
                            }
                        }
                        
                        break;
                    }
                    case TextFormats.Bold:
                    {
                        if (input.StartsWith("**") && input.EndsWith("**"))
                        {
                            formattedText = input.Substring(2, input.Length - 4);
                        }
                        else
                        {
                            formattedText = "**" + input + "**";
                        }

                        break;
                    }
                    case TextFormats.Heading1:
                    {
                        if (input.StartsWith("# "))
                        {
                            formattedText = input.Substring(1, input.Length - 1).Trim();
                        }
                        else
                        {
                            formattedText = "# " + input;
                        }

                        break;
                    }
                    case TextFormats.Heading2:
                    {
                        if (input.StartsWith("## "))
                        {
                            formattedText = input.Substring(2, input.Length - 2).Trim();
                        }
                        else
                        {
                            formattedText = "## " + input;
                        }

                        break;
                    }
                    case TextFormats.Heading3:
                    {
                        if (input.StartsWith("### "))
                        {
                            formattedText = input.Substring(3, input.Length - 3).Trim();
                        }
                        else
                        {
                            formattedText = "### " + input;
                        }

                        break;
                    }
                    case TextFormats.Heading4:
                    {
                        if (input.StartsWith("#### "))
                        {
                            formattedText = input.Substring(4, input.Length - 4).Trim();
                        }
                        else
                        {
                            formattedText = "#### " + input;
                        }

                        break;
                    }
                    case TextFormats.Heading5:
                    {
                        if (input.StartsWith("##### "))
                        {
                            formattedText = input.Substring(5, input.Length - 5).Trim();
                        }
                        else
                        {
                            formattedText = "##### " + input;
                        }

                        break;
                    }
                    case TextFormats.Heading6:
                    {
                        if (input.StartsWith("###### "))
                        {
                            formattedText = input.Substring(6, input.Length - 6).Trim();
                        }
                        else
                        {
                            formattedText = "###### " + input;
                        }

                        break;
                    }
                    case TextFormats.InlineCode:
                    {
                        if (input.StartsWith("`") && input.EndsWith("`"))
                        {
                            formattedText = input.Substring(1, input.Length - 2).Trim();
                        }
                        else
                        {
                            formattedText = "`" + input + "`";
                        }
                        break;
                    }
                    case TextFormats.Italic:
                    {
                        if (input.StartsWith("*") && input.EndsWith("*"))
                        {
                            formattedText = input.Substring(1, input.Length - 2).Trim();
                        }
                        else
                        {
                            formattedText = "*" + input + "*";
                        }
                        break;
                    }
                    case TextFormats.OrderedList:
                    {
                        input = input.Replace("\r", "");
                        var lines = input.Split('\n');
                        for (var i = 0; i < lines.Length; i++)
                        {
                            var line = lines[i];
                            if (!string.IsNullOrEmpty(line))
                            {
                                // If the line starts with a number, followed by a dot and a space
                                if (Regex.IsMatch(line, @"^\d+\. "))
                                {
                                    if (i != lines.Length - 1)
                                    {
                                        formattedText += line.Substring(2, line.Length - 2).Trim() + "\r\n";
                                    }
                                    else
                                    {
                                        formattedText += line.Substring(2, line.Length - 2).Trim();
                                    }
                                }
                                else
                                {
                                    if (i != lines.Length - 1)
                                    {
                                        formattedText += (i + 1) + ". " + line.Trim() + "\r\n";
                                    }
                                    else
                                    {
                                        formattedText += (i + 1) + ". " + line.Trim();
                                    }
                                }
                            }
                        }
                        break;
                    }
                    case TextFormats.UnorderedList:
                    {
                        input = input.Replace("\r", "");
                        var lines = input.Split('\n');
                        for (var i = 0; i < lines.Length; i++)
                        {
                            var line = lines[i];
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (line.StartsWith("* ") || line.StartsWith("- "))
                                {
                                    if (i != lines.Length - 1)
                                    {
                                        formattedText += line.Substring(2, line.Length - 2).Trim() + "\r\n";
                                    }
                                    else
                                    {
                                        formattedText += line.Substring(2, line.Length - 2).Trim();
                                    }
                                }
                                else
                                {
                                    if (i != lines.Length - 1)
                                    {
                                        formattedText += "* " + line.Trim() + "\r\n";
                                    }
                                    else
                                    {
                                        formattedText += "* " + line.Trim();
                                    }
                                }
                            }
                        }
                        break;
                    }
                    case TextFormats.StrikeThrough:
                    {
                        if (input.StartsWith("~~") && input.EndsWith("~~"))
                        {
                            formattedText = input.Substring(2, input.Length - 4);
                        }
                        else
                        {
                            formattedText = "~~" + input + "~~";
                        }

                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            Logger.GetInstance().Debug("<< Format()");
            return formattedText;
        }
    }
}
