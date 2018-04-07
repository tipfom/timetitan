using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Universal.UI.Layout;

namespace Universal.UI.Elements {
    public class Swoosh : Element {
        const float FADE_SPEED = 1.5f;

        private float opacity = 0f;
        private string texture = "swoosh1";

        public Swoosh (Screen owner, Container container, int depth) : base(owner, container, depth, false) {
        }

        public override bool HandleTouch (Touch.Action action, Touch touch) {
            return false;
        }

        public override void Update (DeltaTime dt) {
            base.Update(dt);

            if (opacity > 0) {
                opacity -= Math.Max(0, dt.Seconds * FADE_SPEED);
                IsDirty = true;
            }
        }

        public void Appear ( ) {
            opacity = 1f;
            IsDirty = true;
            texture = "swoosh" + Mathi.Random(1, 3);
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            yield return new RenderableElement(Container.Box.Verticies, texture, Depth, new Color(1f, 1f, 1f, opacity));
        }
    }
}
