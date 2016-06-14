using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMarkdown.MarkdownLibrary.HtmlComponents
{
    public class Image : HtmlComponent
    {
        private readonly string _imageUrl;
        private readonly int _height;
        private readonly int _width;

        public Image(string imageUrl, string imageAltText) : base(imageAltText, TagTypes.Image)
        {
            _imageUrl = imageUrl;
        }
        public Image(string imageUrl, string imageAltText, int height, int width) : base(imageAltText, TagTypes.Image)
        {
            _imageUrl = imageUrl;
            _height = height;
            _width = width;
        }

        public override string ToString()
        {
            if (_height == 0 && _width == 0)
            {
                return "<img src=\"" + _imageUrl + "\" alt=\"" + Text + "\">";
            }
            else
            {
                return "<img src=\"" + _imageUrl + "\" alt=\"" + Text + "\" height=\"" + _height + "\" width=\"" +_width+ "\"" +">";
            }
        }
    }
}
