using Core.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.World.Mobs {
    public class Plugger : Mob {
        public Plugger ( ) : base(EntitySpecies.PLUGGER) {
            Health = 10;
        }

        public override IEnumerable<RenderableObject> Draw ( ) {
            float top = 1f + offset;
            yield return new RenderableObject(new float[ ] { -27f / 20f, top, -27f / 20f, offset, 0f, offset, 0f, top }, "body", GetColor( ));
        }
    }
}
