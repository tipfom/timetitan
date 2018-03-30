using Core.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.World.Mobs {
    public class Octopus : Mob {
        public Octopus ( ) : base(EntitySpecies.OCTOPUS) {
            Health = 5;
        }

        public override IEnumerable<RenderableObject> Draw ( ) {
            float top = 1f + offset;
            yield return new RenderableObject(new float[ ] { -1f, top, -1f, offset, 0f, offset, 0f, top }, "body", GetColor( ));
        }
    }
}
