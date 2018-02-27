using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Universal.UI.Layout;

namespace Universal.UI.Elements {
    public class TargetArea : Element {
        private static readonly Color ACTIVE_COLOR = new Color(0, 204, 0, 255);
        private static readonly Color[ ] INACTIVE_COLOR = new[ ] { new Color(144, 144, 144, 32), new Color(144, 144, 144, 128), new Color(144, 144, 144, 0) };
        private static readonly Color FAILED_COLOR = new Color(204, 0, 0, 255);

        private Action<int> hitCallback;

        private bool active;
        private Vector2 targetSize;
        private Stack<Box> targets = new Stack<Box>(10);

        public TargetArea (Screen owner, Container container, int depth) : base(owner, container, depth, false) {
        }

        public void Clear ( ) {
            targets.Clear( );
            IsDirty = true;
        }

        public void Challenge (int targetCount, float relativeTargetSize, Action<int> targetHitCallback) {
            targetSize = new Vector2(relativeTargetSize * Container.Width, relativeTargetSize * Container.Width);
            GenerateTargets(targetCount);
            hitCallback = targetHitCallback;
            active = true;
            IsDirty = true;
        }

        public void Stop ( ) {
            active = false;
            IsDirty = true;
        }

        public override bool HandleTouch (Touch.Action action, Touch touch) {
            if (action == Touch.Action.Begin && active && IsHit(touch)) {
                UpdateTarget( );
                hitCallback?.Invoke(targets.Count);
            }
            return true;
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            int i = 2;
            foreach (Box box in targets) {
                if (i < 0) break;
                yield return new RenderableElement(box.Verticies, "target", Depth, INACTIVE_COLOR[i]);
                i--;
            }

            if (targets.Count > 0)
                yield return new RenderableElement(targets.Peek( ).Verticies, "target", Depth, active ? ACTIVE_COLOR : FAILED_COLOR);
        }

        private bool IsHit (Touch touch) {
            if (targets.Count == 0) return false;
            return targets.Peek( ).Collides(touch.RelativePosition);
        }

        private Box CreateTarget ( ) {
            Vector2 targetPosition = new Vector2(Mathf.Random(0, Container.Width - targetSize.X), -Mathf.Random(0, Container.Height - targetSize.Y));
            return new Box(Container.Location + targetPosition, targetSize);
        }

        private void GenerateTargets (int count) {
            while (count-- > 0) {
                targets.Push(CreateTarget( ));
            }
        }

        private void UpdateTarget ( ) {
            if (targets.Count > 0) targets.Pop( );
            IsDirty = true;
        }
    }
}
