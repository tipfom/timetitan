using Core;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.UI.Layout;
using Universal.World;

namespace Universal.UI.Elements.Challenges {
    public class SingleTapChallenge : TapChallenge {
        private Box _Box;
        public Box Box { get { return _Box; } }

        public SingleTapChallenge (Vector2 containerLocation, Vector2 containerSize, float relativeTargetSize) : base(ChallengeType.SingleTap) {
            Vector2 size = new Vector2(relativeTargetSize * containerSize.X, relativeTargetSize * containerSize.X);
            Vector2 position = new Vector2(Mathf.Random(0, containerSize.X - size.X), -Mathf.Random(0, containerSize.Y - size.Y));
            _Box = new Box(containerLocation + position, size);
        }

        public override UpdateAction Update (Touch.Action action, Touch touch) {
            if (action == Touch.Action.Begin) {
                if (Box.Collides(touch.RelativePosition)) {
                    return UpdateAction.Complete;
                } else {
                    return UpdateAction.Miss;
                }
            }
            return UpdateAction.Ignore;
        }
    }
}
