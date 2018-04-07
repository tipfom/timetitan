using System.Collections.Generic;
using Core;
using Universal.UI.Layout;

namespace Universal.UI.Elements {

    public class Button : Element {
        private const float DEFAULT_TEXT_SIZE = 0.1f;
        private const float EDGE_WIDTH_HEIGHT_RATIO = 3f / 20f;

        private Color _Color;
        private string _Text;
        private float charSize;
        private string[ ] lines;

        public Button (Screen owner, Container container, string text) : this(owner, container, text, DEFAULT_TEXT_SIZE, 0, Color.White) {
        }

        public Button (Screen owner, Container container, string text, int depth, Color color) : this(owner, container, text, DEFAULT_TEXT_SIZE, depth, color) {
        }

        public Button (Screen owner, Container container, string text, float textsize, int depth, Color color) : base(owner, container, depth, false) {
            _Text = text;
            lines = text.Split('\n');
            charSize = textsize;
            Color = color;
            Click += ( ) => IsDirty = true;
            Release += ( ) => IsDirty = true;
            Leave += ( ) => IsDirty = true;
        }

        public Color Color { get { return _Color; } set { _Color = value; IsDirty = true; ; } }
        public string Text { get { return _Text; } set { _Text = value; lines = _Text.Split('\n'); IsDirty = true; } }

        public override IEnumerable<RenderableElement> Draw ( ) {
            string textureDomain = "btn_" + (Clicked ? "p" : "i");
            float w = EDGE_WIDTH_HEIGHT_RATIO * Container.Height;
            yield return new RenderableElement(Box.GetVerticies(Container.X, Container.Y, w, Container.Height), textureDomain + "l", Depth, Color);
            yield return new RenderableElement(Box.GetVerticies(Container.X + w, Container.Y, Container.Width - 2 * w, Container.Height), textureDomain + "c", Depth, Color);
            yield return new RenderableElement(Box.GetVerticies(Container.X + Container.Width - w, Container.Y, w, Container.Height), textureDomain + "r", Depth, Color);

            Vector2 textPosition = new Vector2(Container.X + Container.Width * 0.5f, Container.Y - (Container.Height - lines.Length * charSize) * 0.5f);

            foreach (RenderableElement d in Label.GetRenderableElements(lines, Label.TextAlignment.Center, textPosition, charSize, Depth, Color.White)) {
                yield return d;
            }
        }
    }
}