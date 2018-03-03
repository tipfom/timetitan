using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Universal.UI.Layout;
using Universal.World;

namespace Universal.UI.Elements {
    public class TargetArea : Element {
        private static readonly Color ACTIVE_SINGLETAP_COLOR = new Color(0, 204, 0, 255);
        private static readonly Color ACTIVE_DOUBLETAP_COLOR = new Color(204, 204, 0, 255);
        private static readonly Color[ ] PREVIEW_COLOR = new[ ] { new Color(144, 144, 144, 128), new Color(144, 144, 144, 32) };
        private static readonly Color FAILED_COLOR = new Color(204, 0, 0, 255);

        private Action<int> hitCallback;

        private bool active;
        private List<TapChallenge> challenges = new List<TargetArea.TapChallenge>(10);

        public TargetArea (Screen owner, Container container, int depth) : base(owner, container, depth, false) {
        }

        public void Clear ( ) {
            challenges.Clear( );
            IsDirty = true;
        }

        public void Challenge (int singleTapChallengeCount, int doubleTapChallengeCount, float relativeTargetSize, Action<int> targetHitCallback) {
            GenerateChallenges(singleTapChallengeCount, doubleTapChallengeCount, relativeTargetSize);
            hitCallback = targetHitCallback;
            active = true;
            IsDirty = true;
        }

        public void Stop ( ) {
            active = false;
            IsDirty = true;
        }

        public override bool HandleTouch (Touch.Action action, Touch touch) {
            if (active && challenges[0].IsCompleted(action, touch)) {
                UpdateTarget( );
                hitCallback?.Invoke(challenges.Count);
            }
            return true;
        }

        public override void Update (DeltaTime dt) {
            if (active && challenges[0].IsDirty) IsDirty = true;
            base.Update(dt);
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            for (int i = 1; i < challenges.Count && i < 3; i++) {
                yield return GetRenderableElement(challenges[0], i-1);
            }

            if (challenges.Count > 0) {
                yield return GetRenderableElement(challenges[0]);
            }
        }

        private RenderableElement GetRenderableElement (TapChallenge challenge, int previewIndex = -1) {
            switch (challenge.Type) {
                case ChallengeType.SingleTap:
                    return new RenderableElement(challenges[0].Box.Verticies, "singletap", Depth, (previewIndex > 0) ? PREVIEW_COLOR[previewIndex] : (active ? ACTIVE_SINGLETAP_COLOR : FAILED_COLOR));
                case ChallengeType.DoubleTap:
                    return new RenderableElement(challenges[0].Box.Verticies, "doubletap", Depth, (previewIndex > 0) ? PREVIEW_COLOR[previewIndex] : (active ? ACTIVE_DOUBLETAP_COLOR : FAILED_COLOR));
            }
            return null;
        }

        private void GenerateChallenges (int singleTapChallengeCount, int doubleTapChallengeCount, float relativeTargetSize) {
            while (singleTapChallengeCount-- > 0) {
                challenges.Add(new SingleTapChallenge(Container.Location, Container.Size, relativeTargetSize));
            }
            while (doubleTapChallengeCount-- > 0) {
                challenges.Insert(Mathi.Random(0, challenges.Count), new DoubleTapChallenge(Container.Location, Container.Size, relativeTargetSize));
            }
        }

        private void UpdateTarget ( ) {
            if (challenges.Count > 0) challenges.RemoveAt(0);
            IsDirty = true;
        }

        private abstract class TapChallenge {
            public readonly ChallengeType Type;
            public bool IsDirty;

            public abstract Box Box { get; }

            public TapChallenge (ChallengeType type) {
                Type = type;
            }

            public abstract bool IsCompleted (Touch.Action action, Touch touch);
        }

        private class SingleTapChallenge : TapChallenge {
            private Box _Box;
            public override Box Box { get { return _Box; } }

            public SingleTapChallenge (Vector2 containerLocation, Vector2 containerSize, float relativeTargetSize) : base(ChallengeType.SingleTap) {
                Vector2 size = new Vector2(relativeTargetSize * containerSize.X, relativeTargetSize * containerSize.X);
                Vector2 position = new Vector2(Mathf.Random(0, containerSize.X - size.X), -Mathf.Random(0, containerSize.Y - size.Y));
                _Box = new Box(containerLocation + position, size);
            }

            public override bool IsCompleted (Touch.Action action, Touch touch) {
                if (action == Touch.Action.Begin && Box.Collides(touch.RelativePosition)) {
                    return true;
                }
                return false;
            }
        }

        private class DoubleTapChallenge : TapChallenge {
            private const float BIG_SIZE_FACTOR = 1.2f;

            private Box bigBox;
            private Box smallBox;
            private bool clickedOnce = false;

            public override Box Box { get { return clickedOnce ? smallBox : bigBox; } }

            public DoubleTapChallenge (Vector2 containerLocation, Vector2 containerSize, float relativeTargetSize) : base(ChallengeType.DoubleTap) {
                Vector2 smallSize = new Vector2(relativeTargetSize * containerSize.X, relativeTargetSize * containerSize.X);
                Vector2 bigSize = smallSize * BIG_SIZE_FACTOR;
                Vector2 position = new Vector2(Mathf.Random(bigSize.X / 2f, containerSize.X - bigSize.X / 2f), -Mathf.Random(bigSize.Y / 2f, containerSize.Y - bigSize.Y / 2f));
                bigBox = new Box(containerLocation + position + new Vector2(-bigSize.X / 2f, bigSize.Y / 2f), bigSize);
                smallBox = new Box(containerLocation + position + new Vector2(-smallSize.X / 2f, smallSize.Y / 2f), smallSize);
            }

            public override bool IsCompleted (Touch.Action action, Touch touch) {
                if (action == Touch.Action.Begin && Box.Collides(touch.RelativePosition)) {
                    if (clickedOnce) {
                        return true;
                    } else {
                        IsDirty = true;
                        clickedOnce = true;
                        return false;
                    }
                }
                return false;
            }
        }
    }
}
