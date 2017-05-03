using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static Uri ToUri(this string input)
        {
            var output = new Uri(input);
            return output;
        }
    }
}
