using System;
using System.Linq;
using System.Text.RegularExpressions;
using ProjectMarkdown.MarkdownLibrary.HtmlComponents;

namespace ProjectMarkdown.MarkdownLibrary.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static string ReplaceSpecialCharacters(this string input)
        {
            try
            {
                var output = input.Replace("&", "&amp").Replace("<", "&lt;").Replace(">", "&gt;");
                return output;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string ConvertMarkdownToHtml(this string input)
        {
            try
            {
                // Code should be the last because it strips all html tags from its content
                return input.ConvertPairedMarkdownToHtml("**", MarkdownParser.PairedMarkdownTags.Bold)
                    .ConvertPairedMarkdownToHtml("__", MarkdownParser.PairedMarkdownTags.Bold)
                    .ConvertPairedMarkdownToHtml("*", MarkdownParser.PairedMarkdownTags.Italic)
                    .ConvertPairedMarkdownToHtml("_", MarkdownParser.PairedMarkdownTags.Italic)
                    .ConvertPairedMarkdownToHtml("~~", MarkdownParser.PairedMarkdownTags.StrikeThrough)
                    .ConvertPairedMarkdownToHtml("`", MarkdownParser.PairedMarkdownTags.InlineCode)
                    .GenerateInlineImages()
                    .GenerateHtmlLinks()
                    .GenerateAutomaticLinks();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string ConvertPairedMarkdownToHtml(this string input, string markdownTag, MarkdownParser.PairedMarkdownTags tag)
        {
            try
            {
                string output = string.Empty;
                var tagSplit = input.Split(new string[] { markdownTag }, StringSplitOptions.None);
                if (markdownTag.Length == 1 && tag != MarkdownParser.PairedMarkdownTags.InlineCode)
                {
                    tagSplit = FillEmptyValues(tagSplit, markdownTag);
                }

                for (int i = 0; i < tagSplit.Length; i++)
                {
                    if (i != 0 && i != tagSplit.Length - 1)
                    {
                        if (tagSplit[i].Length - 1 > 0 && tagSplit[i][0] != ' ' && tagSplit[i][tagSplit[i].Length - 1] != ' ')
                        {
                            if (tag == MarkdownParser.PairedMarkdownTags.InlineCode)
                            {
                                tagSplit[i] = new InlineCode(tagSplit[i]).ToString();
                            }
                            else
                            {
                                if (!(tagSplit[i][0] == '~' || tagSplit[i][0] == '_' || tagSplit[i][0] == '*' ||
                                tagSplit[i][tagSplit[i].Length - 1] == '~' || tagSplit[i][tagSplit[i].Length - 1] == '_' || tagSplit[i][tagSplit[i].Length - 1] == '*'))
                                {
                                    if (tag == MarkdownParser.PairedMarkdownTags.Bold)
                                    {
                                        if (!IsInCodeTag(tagSplit, i))
                                        {
                                            tagSplit[i] = new Bold(tagSplit[i]).ToString();
                                        }
                                        else
                                        {
                                            tagSplit[i] = markdownTag + tagSplit[i] + markdownTag;
                                        }
                                    }
                                    else if (tag == MarkdownParser.PairedMarkdownTags.Italic)
                                    {
                                        if (!IsInCodeTag(tagSplit, i))
                                        {
                                            tagSplit[i] = new Italic(tagSplit[i]).ToString();
                                        }
                                        else
                                        {
                                            tagSplit[i] = markdownTag + tagSplit[i] + markdownTag;
                                        }
                                    }
                                    else if (tag == MarkdownParser.PairedMarkdownTags.StrikeThrough)
                                    {
                                        tagSplit[i] = new StrikeThrough(tagSplit[i]).ToString();
                                    }
                                }
                            }
                        }
                    }
                    output += tagSplit[i];
                }

                return output;
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while converting paired Markdown to HTML. " + e.Message);
            }
        }

        private static string[] FillEmptyValues(string[] tagSplit, string markdownTag)
        {
            for (var i = 0; i < tagSplit.Length; i++)
            {
                if (string.IsNullOrEmpty(tagSplit[i]))
                {
                    tagSplit[i] = markdownTag;
                }
            }

            return tagSplit;
        }

        private static bool IsInCodeTag(string input, string match)
        {
            var start = input.IndexOf("<code>");
            var end = input.IndexOf("</code>");

            if (start == -1 || end == -1)
            {
                return false;
            }

            var between = input.Substring(start, end - start);
            if (between.Contains(match))
            {
                return true;
            }

            return false;
        }

        private static bool IsInCodeTag(string[] tagSplit, int index)
        {
            if (index != 0 && index != tagSplit.Length - 1)
            {
                var hasFirstCodeTag = false;
                var hasLastCodeTag = false;

                var i = index;
                do
                {
                    if (tagSplit[i].Contains("`"))
                    {
                        hasFirstCodeTag = true;
                        break;
                    }
                    i--;
                } while (i > -1);

                i = index;
                do
                {
                    if (tagSplit[i].Contains("`"))
                    {
                        hasLastCodeTag = true;
                        break;
                    }
                    i++;
                } while (i != tagSplit.Length);

                if (hasFirstCodeTag && hasLastCodeTag)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public static string GenerateInlineImages(this string input)
        {
            try
            {
                var output = Regex.Replace(input, @"!\[.*?\]\(.*?\)", match => ConvertToImage(match, input));
                return output;
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while generating inline images. " + e.Message);
            }
        }

        private static string ConvertToImage(Match match, string input)
        {
            try
            {
                var isInCodeTag = IsInCodeTag(input, match.ToString());
                if (!isInCodeTag)
                {
                    var imageAltText = new string(match.ToString().SkipWhile(s => s != '[').Skip(1).TakeWhile(s => s != ']').ToArray()).Trim();
                    var imageUrl = new string(match.ToString().SkipWhile(s => s != '(').Skip(1).TakeWhile(s => s != ')').ToArray()).Trim();
                    return new Image(imageUrl, imageAltText).ToString();
                }
                return match.ToString();
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while converting Markdown to image. " + e.Message);
            }
        }

        public static string GenerateHtmlLinks(this string input)
        {
            try
            {
                var output = Regex.Replace(input, @"\[.*?\]\(.*?\)", match => ConvertToLink(match, input));
                return output;
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while generating HTML links. " + e.Message);
            }
        }

        public static string GenerateAutomaticLinks(this string input)
        {
            try
            {
                string output = string.Empty;

                var words = input.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    if (Regex.IsMatch(words[i], "^(https|http):\\/\\/.+"))
                    {
                        var isInCodeTag = IsInCodeTag(input, words[i]);
                        if (!isInCodeTag)
                        {
                            words[i] = new Link(words[i], words[i]).ToString();
                        }
                    }
                    if (i != words.Length - 1)
                    {
                        output += words[i] + " ";
                    }
                    else
                    {
                        output += words[i];
                    }
                }
                return output;
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while generating automatic links. " + e.Message);
            }
        }

        private static string ConvertToLink(Match match, string input)
        {
            try
            {
                var isInCodeTag = IsInCodeTag(input, match.ToString());
                if (!isInCodeTag)
                {
                    var linkText = new string(match.ToString().SkipWhile(s => s != '[').Skip(1).TakeWhile(s => s != ']').ToArray()).Trim();
                    var urlText = new string(match.ToString().SkipWhile(s => s != '(').Skip(1).TakeWhile(s => s != ')').ToArray()).Trim();
                    return new Link(linkText, urlText).ToString();
                }
                return match.ToString();
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while converting Markdown to HTML link. " + e.Message);
            }
        }
    }
}
