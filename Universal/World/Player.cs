using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Core.Graphics;
using Universal.UI.Layout;

namespace Universal.World {
    public class Player : Entity {
        private const float TIME_BETWEEN_FRAMES = 0.150f;

        private int current = 0;
        private bool attacking = false;
        private float frameTimeLeft = TIME_BETWEEN_FRAMES;

        public float Damage = 2.3f;

        public Player ( ) : base(Entity.PLAYER) {

        }

        public void Attack ( ) {
            attacking = true;
            current = 0;
        }

        public override void Update (DeltaTime deltaTime) {
            base.Update(deltaTime);

            if (attacking) {
                frameTimeLeft -= deltaTime.TotalSeconds;
                if (frameTimeLeft < 0) {
                    frameTimeLeft = TIME_BETWEEN_FRAMES;
                    current++;

                    if (current > 3) {
                        current = 0;
                        attacking = false;
                    }
                }
            }
        }

        public override IEnumerable<RenderableObject> Draw ( ) {
            yield return new RenderableObject(new float[ ] { 0, 1, 0, 0, 23f / 19f, 0, 23f / 19f, 1 }, current.ToString( ), Color.White);
        }
    }
}
