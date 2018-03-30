using System;
using System.Collections.Generic;
using System.Text;
using Core.Graphics;

namespace Universal.World.Mobs {
    public class Chest : Mob {
        public Chest ( ) : base(EntitySpecies.CHEST, 15, 250) {
        }

        public override IEnumerable<RenderableObject> Draw ( ) {
            float top = 1.5f + offset;
            yield return new RenderableObject(new float[ ] { -1.5f, top, -1.5f, offset, 0f, offset, 0f, top }, "body", GetColor( ));
        }
    }
}
