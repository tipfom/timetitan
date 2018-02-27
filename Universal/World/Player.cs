using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Core.Graphics;
using Universal.UI.Layout;

namespace Universal.World {
    public class Player : Entity {
        public Player ( ) : base(Entity.PLAYER) {

        }

        public override IEnumerable<RenderableObject> Draw ( ) {
            yield return new RenderableObject(new float[ ] { 0, 1, 0, 0, 23f / 19f, 0, 23f / 19f, 1 }, "body", Color.White);
        }
    }
}
