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
            var output = input.Replace("&", "&amp").Replace("<", "&lt;").Replace(">", "&gt;");
            return output;
        }

        public static string ConvertMarkdownToHtml(this string input)
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

        public static string ConvertPairedMarkdownToHtml(this string input, string markdownTag, MarkdownParser.PairedMarkdownTags tag)
        {
            try
            {
                string output = string.Empty;
                var tagSplit = input.Split(new string[] { markdownTag }, StringSplitOptions.None);

                for (int i = 0; i < tagSplit.Length; i++)
                {
                    if (i != 0 && i != tagSplit.Length - 1)
                    {
                        if (tagSplit[i][0] != ' ' && tagSplit[i][tagSplit[i].Length - 1] != ' ')
                        {
                            if (tag == MarkdownParser.PairedMarkdownTags.Bold)
                            {
                                tagSplit[i] = new Bold(tagSplit[i]).ToString();
                            }
                            else if (tag == MarkdownParser.PairedMarkdownTags.Italic)
                            {
                                tagSplit[i] = new Italic(tagSplit[i]).ToString();
                            }
                            else if (tag == MarkdownParser.PairedMarkdownTags.InlineCode)
                            {
                                tagSplit[i] = new InlineCode(tagSplit[i]).ToString();
                            }
                            else if (tag == MarkdownParser.PairedMarkdownTags.StrikeThrough)
                            {
                                tagSplit[i] = new StrikeThrough(tagSplit[i]).ToString();
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

        public static string GenerateInlineImages(this string input)
        {
            try
            {
                var output = Regex.Replace(input, @"!\[.*?\]\(.*?\)", new MatchEvaluator(ConvertToImage));
                return output;
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while generating inline images. " + e.Message);
            }
        }

        private static string ConvertToImage(Match match)
        {
            try
            {
                var imageAltText = new string(match.ToString().SkipWhile(s => s != '[').Skip(1).TakeWhile(s => s != ']').ToArray()).Trim();
                var imageUrl = new string(match.ToString().SkipWhile(s => s != '(').Skip(1).TakeWhile(s => s != ')').ToArray()).Trim();
                return new Image(imageUrl, imageAltText).ToString();
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
                var output = Regex.Replace(input, @"\[.*?\]\(.*?\)", new MatchEvaluator(ConvertToLink));
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
                    if (Regex.IsMatch(words[i], "^http://"))
                    {
                        words[i] = new Link(words[i], words[i]).ToString();
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

        private static string ConvertToLink(Match match)
        {
            try
            {
                var linkText = new string(match.ToString().SkipWhile(s => s != '[').Skip(1).TakeWhile(s => s != ']').ToArray()).Trim();
                var urlText = new string(match.ToString().SkipWhile(s => s != '(').Skip(1).TakeWhile(s => s != ')').ToArray()).Trim();
                return new Link(linkText, urlText).ToString();
            }
            catch (Exception e)
            {
                throw new Exception("An error occured while converting Markdown to HTML link. " + e.Message);
            }
        }
    }
}
