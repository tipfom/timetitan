using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Universal.UI.Layout;

namespace Universal.UI.Elements {
    public class ProgressBar : Element {
        public float Max;

        private float _Value;
        public float Value { get { return _Value; } set { IsDirty = value != _Value;  _Value = value; } }

        public float Percentage { get { return Value / Max; } }

        public ProgressBar (Screen owner, Container container, int depth) : base(owner, container, depth, false) {
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            yield return new RenderableElement(Container.Box.Verticies, "blank", Depth, Color.Black);
            yield return new RenderableElement(Box.GetVerticies(Container.X, Container.Y, Container.Width * Percentage, Container.Height), "blank", Depth, Color.Red);
        }
    }
}
