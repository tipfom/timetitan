using Core;
using Core.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.Graphics.Animations;

namespace Universal.World.Mobs {
    public class Octopus : Mob {
        private SpriteAnimation spriteAnimation = new SpriteAnimation("animations/octopus.txt");

        public Octopus (long stage) : base(MobType.OCTOPUS, 5, 75, stage) {
        }

        public override void Update (DeltaTime dt) {
            spriteAnimation.Update(dt);
            base.Update(dt);
        }

        public override IEnumerable<RenderableObject> Draw ( ) {
            float top = 1f + offset;
            yield return new RenderableObject(new float[ ] { -0.5f, top, -0.5f, offset, 0.5f, offset, 0.5f, top }, spriteAnimation.Texture, GetColor( ));
        }
    }
}
