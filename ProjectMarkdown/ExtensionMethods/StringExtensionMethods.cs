using System;
using System.Text.RegularExpressions;
using LogUtils;

namespace ProjectMarkdown.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static string RemoveScripts(this string input)
        {
            Logger.GetInstance().Debug("RemoveScripts() >>");

            try
            {
                var rRemScript = new Regex(@"<script[^>]*>[\s\S]*?</script>");
                var output = rRemScript.Replace(input, "");

                Logger.GetInstance().Debug("<< RemoveScripts()");
                return output;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Uri ToUri(this string input)
        {
            Logger.GetInstance().Debug("ToUri() >>");
            try
            {
                Uri output;

                if (Uri.TryCreate(input, UriKind.RelativeOrAbsolute, out output))
                {
                    Logger.GetInstance().Debug("<< ToUri()");
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
