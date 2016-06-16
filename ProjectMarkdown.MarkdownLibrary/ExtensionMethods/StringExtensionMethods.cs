using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProjectMarkdown.MarkdownLibrary.HtmlComponents;

namespace ProjectMarkdown.MarkdownLibrary.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static string ConvertPairedMarkdownToHtml(this string input, string markdownTag, MarkdownParser.PairedMarkdownTags tag)
        {
            string output = string.Empty;
            var tagSplit = input.Split(new string[] {markdownTag}, StringSplitOptions.None);

            for (int i = 0; i < tagSplit.Length; i++)
            {
                if (i != 0 && i != tagSplit.Length - 1)
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
                output += tagSplit[i];
            }

            return output;
        }

        public static string GenerateInlineImages(this string input)
        {
            var output = Regex.Replace(input, @"!\[.*?\]\(.*?\)", new MatchEvaluator(ConvertToImage));
            return output;
        }

        private static string ConvertToImage(Match match)
        {
            var imageAltText = new string(match.ToString().SkipWhile(s => s != '[').Skip(1).TakeWhile(s => s != ']').ToArray()).Trim();
            var imageUrl = new string(match.ToString().SkipWhile(s => s != '(').Skip(1).TakeWhile(s => s != ')').ToArray()).Trim();
            return new Image(imageUrl,imageAltText).ToString();
        }

        public static string GenerateHtmlLinks(this string input)
        {
            var output = Regex.Replace(input, @"\[.*?\]\(.*?\)", new MatchEvaluator(ConvertToLink));
            return output;
        }

        public static string GenerateAutomaticLinks(this string input)
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

        private static string ConvertToLink(Match match)
        {
            var linkText = new string(match.ToString().SkipWhile(s => s != '[').Skip(1).TakeWhile(s => s != ']').ToArray()).Trim();
            var urlText = new string(match.ToString().SkipWhile(s => s != '(').Skip(1).TakeWhile(s => s != ')').ToArray()).Trim();
            return new Link(linkText, urlText).ToString();
        }
    }
}
