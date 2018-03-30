using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Core.Graphics;

namespace Universal.World {
    public abstract class Mob : Entity {
        public const int DEATH_DELAY = 1000;

        protected float offset = 0f;

        private float fadePercentage = 1f;
        private bool dying = false;
        private Action<Mob> deadCallback;
        private int dyingEndTime;

        public float Health;

        public Mob (EntitySpecies species) : base(species) {
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
                //Debug.Print(this, offset);
                if (dyingEndTime < Environment.TickCount) {
                    dying = false;
                    deadCallback?.Invoke(this);
                }
            }
        }

        protected Color GetColor ( ) {
            if (fadePercentage != 1f) {
                return new Color(1f, 1f, 1f, fadePercentage);
            } else {
                return Color.White;
            }
        }
    }
}
