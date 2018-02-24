using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Universal.UI.Layout;

namespace Universal.UI.Elements {
    public class TargetArea : Element {
        private static readonly Color ACTIVE_COLOR = new Color(0, 204, 0, 255);
        private static readonly Color INACTIVE_COLOR = new Color(144, 144, 144, 64);
        private static readonly Color FAILED_COLOR = new Color(204, 0, 0, 255);


        private Action<int> hitCallback;
        private int targetsLeft;

        private Vector2 targetSize;
        private Box targetBox;
        private Stack<Box> oldTargets = new Stack<Box>(10);

        public TargetArea (Screen owner, Container container, int depth) : base(owner, container, depth, false) {
        }

        public void Challenge (int targetCount, float relativeTargetSize, Action<int> targetHitCallback) {
            oldTargets.Clear( );
            targetSize = new Vector2(relativeTargetSize * Container.Width, relativeTargetSize * Container.Width);
            UpdateTarget( );
            hitCallback = targetHitCallback;
            targetsLeft = targetCount;
        }

        public void Stop ( ) {
            targetsLeft = -1;
            IsDirty = true;
        }

        public override bool HandleTouch (Touch.Action action, Touch touch) {
            if (action == Touch.Action.Begin && targetsLeft > 0 && IsHit(touch)) {
                targetsLeft--;
                UpdateTarget( );
                if (targetsLeft == 0) {
                    targetBox = null;
                }
                hitCallback?.Invoke(targetsLeft);
            }
            return true;
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            foreach (Box box in oldTargets) {
                yield return new RenderableElement(box.Verticies, "target", Depth, INACTIVE_COLOR);
            }

            if (targetBox != null)
                yield return new RenderableElement(targetBox.Verticies, "target", Depth, targetsLeft > 0 ? ACTIVE_COLOR : FAILED_COLOR);
        }

        private bool IsHit (Touch touch) {
            return targetBox.Collides(touch.RelativePosition);
        }

        private void UpdateTarget ( ) {
            if (targetBox != null)
                oldTargets.Push(targetBox);

            Vector2 targetPosition = new Vector2(Mathf.Random(0, Container.Width - targetSize.X), -Mathf.Random(0, Container.Height - targetSize.Y));
            targetBox = new Box(Container.Location + targetPosition, targetSize);
            IsDirty = true;
        }
    }
}
