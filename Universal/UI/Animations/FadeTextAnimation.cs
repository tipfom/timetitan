using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Universal.UI.Elements;

namespace Universal.UI.Animations {
    public class FadeTextAnimation : Animation {
        private Label label;
        private float speed;
        private float alphaDecline;

        public FadeTextAnimation (Label label, float speed, float alphadecline) {
            this.label = label;
            this.speed = speed;
            this.alphaDecline = alphadecline;
        }

        public override void Dispose ( ) {
            label.Dispose( );
        }

        public override bool Update (DeltaTime dt) {
            label.Container.Margin.Top -= speed * dt.Seconds;
            label.Color = new Color(label.Color.R, label.Color.G, label.Color.B, label.Color.A - alphaDecline * dt.Seconds);
            return label.Color.A <= 0;
        }
    }
}
