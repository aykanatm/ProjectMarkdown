using System;

namespace ProjectMarkdown.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static Uri ToUri(this string input)
        {
            try
            {
                Uri output;

                if (Uri.TryCreate(input, UriKind.RelativeOrAbsolute, out output))
                {
                    return output;
                }

                throw new Exception("Unable to create URI from input '" + input + "'");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
