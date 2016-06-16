using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectMarkdown.MarkdownLibrary.ExtensionMethods;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Blockquote : HtmlComponent
    {
        public Blockquote(string text) : base(text, TagTypes.BlockCode)
        {
            
        }

        public override string ToString()
        {
            return "<blockquote><p>" 
                + Text.ConvertPairedMarkdownToHtml("**", MarkdownParser.PairedMarkdownTags.Bold)
                    .ConvertPairedMarkdownToHtml("__", MarkdownParser.PairedMarkdownTags.Bold)
                    .ConvertPairedMarkdownToHtml("*", MarkdownParser.PairedMarkdownTags.Italic)
                    .ConvertPairedMarkdownToHtml("_", MarkdownParser.PairedMarkdownTags.Italic)
                    .ConvertPairedMarkdownToHtml("~~", MarkdownParser.PairedMarkdownTags.StrikeThrough) 
                    + "</p></blockquote>";
        }
    }
}
