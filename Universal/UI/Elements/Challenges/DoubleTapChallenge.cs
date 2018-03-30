using Core;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.UI.Layout;
using Universal.World;

namespace Universal.UI.Elements.Challenges {
    public class DoubleTapChallenge : TapChallenge {
        private const float BIG_SIZE_FACTOR = 1.2f;

        private Box box;

        public Box Box { get { return box; } }
        public int State = 1;

        public DoubleTapChallenge (Vector2 containerLocation, Vector2 containerSize, float relativeTargetSize) : base(ChallengeType.DoubleTap) {
            Vector2 bigSize = new Vector2(relativeTargetSize * containerSize.X, relativeTargetSize * containerSize.X) * BIG_SIZE_FACTOR;
            Vector2 position = new Vector2(Mathf.Random(bigSize.X / 2f, containerSize.X - bigSize.X / 2f), -Mathf.Random(bigSize.Y / 2f, containerSize.Y - bigSize.Y / 2f));
            box = new Box(containerLocation + position + new Vector2(-bigSize.X / 2f, bigSize.Y / 2f), bigSize);
        }

        public override UpdateAction Update (Touch.Action action, Touch touch) {
            if (action == Touch.Action.Begin) {
                if (Box.Collides(touch.RelativePosition)) {
                    if (State == 2) {
                        return UpdateAction.Complete;
                    } else {
                        IsDirty = true;
                        State = 2;
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
