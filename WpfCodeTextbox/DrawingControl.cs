using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfCodeTextbox
{
    public class DrawingControl : FrameworkElement
    {
        private readonly VisualCollection _visuals;
        private readonly DrawingVisual _drawingVisual;

        protected override int VisualChildrenCount
        {
            get { return _visuals.Count; }
        }

        public DrawingControl()
        {
            _visuals = new VisualCollection(this);
            _drawingVisual = new DrawingVisual();
            _visuals.Add(_drawingVisual);
        }

        public DrawingContext GetContext()
        {
            return _drawingVisual.RenderOpen();
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }
    }
}
