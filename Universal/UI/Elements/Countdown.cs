using Core;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.UI.Layout;

namespace Universal.UI.Elements {
    public class Countdown : Element {
        public event Action Finished;

        public int Delay;

        private float timeLeft;
        private float charSize;

        public Countdown (Screen owner, Container container, int depth, float size, int delay) : base(owner, container, depth, false) {
            Delay = delay;
            charSize = size;
            Visible = false;
        }

        public void Start ( ) {
            timeLeft = Delay;
            Visible = true;
        }

        public override void Update (DeltaTime dt) {
            base.Update(dt);

            if (timeLeft > 0) {
                timeLeft -= dt.TotalSeconds;
                if (timeLeft < 0) {
                    Visible = false;
                    Finished?.Invoke( );
                }
                IsDirty = true;
            }
        }

        public override bool HandleTouch (Touch.Action action, Touch touch) {
            if (!Visible) return false;

            if (timeLeft > 1 && action == Touch.Action.Begin) timeLeft -= .5f;

            return true;
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            return Label.GetRenderableElements(new string[ ] { timeLeft.ToString("0.0") }, Label.TextAlignment.Center, new Vector2(Container.X + Container.Width / 2f, Container.Y - Container.Height / 2f + charSize/2f), charSize, Depth, Color.White);
        }
    }
}
