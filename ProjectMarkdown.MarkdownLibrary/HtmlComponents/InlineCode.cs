﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class InlineCode : HtmlComponent
    {
        public InlineCode(string text) : base(text, TagTypes.InlineCode)
        {
            
        }

        public override string ToString()
        {
            Text = Regex.Replace(Text, "<.*?>", string.Empty);
            return "<code>" + Text + "</code>";
        }
    }
}
