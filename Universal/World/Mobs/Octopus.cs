using Core;
using Core.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.Graphics.Animations;

namespace Universal.World.Mobs {
    public class Octopus : Mob {
        private SpriteAnimation spriteAnimation = new SpriteAnimation("animations/octopus.txt");

        public Octopus ( ) : base(EntitySpecies.OCTOPUS, 5, 75) {
        }

        public override void Update (DeltaTime dt) {
            spriteAnimation.Update(dt);
            base.Update(dt);
        }

        public override IEnumerable<RenderableObject> Draw ( ) {
            float top = 1f + offset;
            yield return new RenderableObject(new float[ ] { -1f, top, -1f, offset, 0f, offset, 0f, top }, spriteAnimation.Texture, GetColor( ));
        }
    }
}
