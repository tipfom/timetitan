using Core;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.UI.Layout;
using Universal.World;

namespace Universal.UI.Elements.Challenges {
    public class DoubleTapChallenge : TapChallenge {
        private const float BIG_SIZE_FACTOR = 1.2f;

        private Box bigBox;
        private Box smallBox;
        private bool clickedOnce = false;

        public Box Box { get { return clickedOnce ? smallBox : bigBox; } }

        public DoubleTapChallenge (Vector2 containerLocation, Vector2 containerSize, float relativeTargetSize) : base(ChallengeType.DoubleTap) {
            Vector2 smallSize = new Vector2(relativeTargetSize * containerSize.X, relativeTargetSize * containerSize.X);
            Vector2 bigSize = smallSize * BIG_SIZE_FACTOR;
            Vector2 position = new Vector2(Mathf.Random(bigSize.X / 2f, containerSize.X - bigSize.X / 2f), -Mathf.Random(bigSize.Y / 2f, containerSize.Y - bigSize.Y / 2f));
            bigBox = new Box(containerLocation + position + new Vector2(-bigSize.X / 2f, bigSize.Y / 2f), bigSize);
            smallBox = new Box(containerLocation + position + new Vector2(-smallSize.X / 2f, smallSize.Y / 2f), smallSize);
        }

        public override UpdateAction Update (Touch.Action action, Touch touch) {
            if (action == Touch.Action.Begin) {
                if (Box.Collides(touch.RelativePosition)) {
                    if (clickedOnce) {
                        return UpdateAction.Complete;
                    } else {
                        IsDirty = true;
                        clickedOnce = true;
                        return UpdateAction.Ignore;
                    }
                } else {
                    return UpdateAction.Miss;
                }
            }
            return UpdateAction.Ignore;
        }
    }
}
