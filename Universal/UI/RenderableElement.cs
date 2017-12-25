using Core;
using Core.Graphics;

namespace Universal.UI {
    public class RenderableElement : RenderableObject {
        public int Depth { get; set; }

        public RenderableElement (float[ ] verticies, string texture, int depth) : this(verticies, texture, depth, Color.White) {
        }

        public RenderableElement (float[ ] verticies, string texture, int depth, Color color) : base(verticies, texture, color) {
            Depth = depth;
        }
    }
}
