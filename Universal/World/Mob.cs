using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Core.Graphics;

namespace Universal.World {
    public class Mob : Entity {
        public const int DEATH_DELAY = 1000;

        private float offset = 0f;

        private float fadePercentage = 1f;
        private bool dying = false;
        private Action<Mob> deadCallback;
        private int dyingEndTime;

        public Mob (int species) : base(species) {
        }

        public void Die (Action<Mob> dyingFinishedCallback) {
            dyingEndTime = Environment.TickCount + DEATH_DELAY;
            deadCallback = dyingFinishedCallback;
            dying = true;
        }

        public override void Update (DeltaTime deltaTime) {
            if (dying) {
                float delta = deltaTime.TotalMilliseconds / DEATH_DELAY;
                fadePercentage -= delta;
                offset += delta;
                Debug.Print(this, offset);
                if (dyingEndTime < Environment.TickCount) {
                    dying = false;
                    deadCallback?.Invoke(this);
                }
            }
        }

        public override IEnumerable<RenderableObject> Draw ( ) {
            float top = 1f + offset;
            yield return new RenderableObject(new float[ ] { -27f / 20f, top, -27f / 20f, offset, 0f, offset, 0f, top }, "body", new Color(1f, 1f, 1f, fadePercentage));
        }
    }
}
