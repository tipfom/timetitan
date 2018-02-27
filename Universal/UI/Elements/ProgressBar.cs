using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Universal.UI.Layout;

namespace Universal.UI.Elements {
    public class ProgressBar : Element {
        public static readonly Color BACKGROUND_COLOR = new Color(32, 32, 32, 32);
        public static readonly Color FOREGROUND_COLOR = new Color(32, 32, 32, 64);

        public float Max;

        private float _Value;
        public float Value { get { return _Value; } set { IsDirty = value != _Value; _Value = value; } }

        public float Percentage { get { return Value / Max; } }

        private Color _Color = FOREGROUND_COLOR;
        public Color Color { get { return _Color; } set { _Color = value; IsDirty = true; } }

        public ProgressBar (Screen owner, Container container, int depth) : base(owner, container, depth, false) {
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            yield return new RenderableElement(Container.Box.Verticies, "blank", Depth, BACKGROUND_COLOR);
            yield return new RenderableElement(Box.GetVerticies(Container.X, Container.Y, Container.Width * Percentage, Container.Height), "blank", Depth, _Color);
        }
    }
}
