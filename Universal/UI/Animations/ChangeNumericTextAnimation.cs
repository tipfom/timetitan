using Core;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.UI.Elements;

namespace Universal.UI.Animations {
    public class ChangeNumericTextAnimation : Animation {
        private Label label;
        private int start;
        private int finish;
        private float totalTime;
        private float progress = 0f;

        public ChangeNumericTextAnimation (Label label, int target, float seconds) {
            this.label = label;
            this.start = int.Parse(label.Text);
            this.finish = target;
            this.totalTime = seconds;
        }

        public override bool Update (DeltaTime dt) {
            progress = Mathf.Clamp01(progress + dt.Seconds / totalTime);
            label.Text = ((int)(start + (finish - start) * progress)).ToString( );
            return progress == 1f;
        }
    }
}
