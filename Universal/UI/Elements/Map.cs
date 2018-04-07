using System;
using System.Collections.Generic;
using System.Text;
using Universal.World;
using Universal.UI.Layout;
using Core;
using Universal.Graphics.Renderer;

namespace Universal.UI.Elements {
    public class Map : Element {
        public const int HEIGHT = 20;
        public const float FLOOR_HEIGHT_RELATIVE = 1f / (float)HEIGHT;
        public const float CHAR_SPACING_MULTIPLIER = 0.08f;

        public Vector2 MobPosition;

        public Map (Screen owner, Container container, int depth) : base(owner, container, depth, false) {
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            yield return new RenderableElement(Container.Box.Verticies, "map_background", Depth);

            float floorHeightTotal = Container.Height * FLOOR_HEIGHT_RELATIVE;
            yield return new RenderableElement(Box.GetVerticies(Container.X, Container.Y - Container.Height + floorHeightTotal, Container.Width, floorHeightTotal), "map_floor", Depth);

            MobPosition = new Vector2(Container.X + Container.Width / 2f, Container.Y - Container.Height + floorHeightTotal);
        }

        public override void Update (DeltaTime dt) {
        }
    }
}
