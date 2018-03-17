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

        public void Challenge (int singleTapChallengeCount, int doubleTapChallengeCount, int pullTapChallengeCount, float relativeTargetSize, Action<int> targetHitCallback) {
            GenerateChallenges(singleTapChallengeCount, doubleTapChallengeCount, pullTapChallengeCount, relativeTargetSize);
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
                foreach (RenderableElement renderableElement in GetRenderableElement(challenges[i], i - 1))
                    yield return renderableElement;
            }

            if (challenges.Count > 0) {
                foreach (RenderableElement renderableElement in GetRenderableElement(challenges[0]))
                    yield return renderableElement;
            }
        }

        private IEnumerable<RenderableElement> GetRenderableElement (TapChallenge challenge, int previewIndex = -1) {
            switch (challenge.Type) {
                case ChallengeType.SingleTap:
                    yield return new RenderableElement(((SingleTapChallenge)challenge).Box.Verticies, "singletap", Depth, (previewIndex >= 0) ? PREVIEW_COLOR[previewIndex] : (active ? ACTIVE_SINGLETAP_COLOR : FAILED_COLOR));
                    break;
                case ChallengeType.DoubleTap:
                    yield return new RenderableElement(((DoubleTapChallenge)challenge).Box.Verticies, "doubletap", Depth, (previewIndex >= 0) ? PREVIEW_COLOR[previewIndex] : (active ? ACTIVE_DOUBLETAP_COLOR : FAILED_COLOR));
                    break;
                case ChallengeType.PullTap:
                    yield return new RenderableElement(((PullTapChallenge)challenge).GoalBox.Verticies, "singletap", Depth, (previewIndex >= 0) ? PREVIEW_COLOR[previewIndex] : (active ? ACTIVE_DOUBLETAP_COLOR : FAILED_COLOR));
                    yield return new RenderableElement(((PullTapChallenge)challenge).ScorerBox.Verticies, "pulltap", Depth, (previewIndex >= 0) ? PREVIEW_COLOR[previewIndex] : (active ? ACTIVE_DOUBLETAP_COLOR : FAILED_COLOR));
                    break;
            }
        }

        private void GenerateChallenges (int singleTapChallengeCount, int doubleTapChallengeCount, int pullTapChallengeCount, float relativeTargetSize) {
            while (singleTapChallengeCount-- > 0) {
                challenges.Add(new SingleTapChallenge(Container.Location, Container.Size, relativeTargetSize));
            }
            while (doubleTapChallengeCount-- > 0) {
                challenges.Add(new DoubleTapChallenge(Container.Location, Container.Size, relativeTargetSize));
            }
            while (pullTapChallengeCount-- > 0) {
                challenges.Add(new PullTapChallenge(Container.Location, Container.Size, relativeTargetSize));
            }
        }

        private void UpdateTarget ( ) {
            if (challenges.Count > 0) challenges.RemoveAt(0);
            IsDirty = true;
        }

        private abstract class TapChallenge {
            public readonly ChallengeType Type;
            public bool IsDirty;

            public TapChallenge (ChallengeType type) {
                Type = type;
            }

            public abstract bool IsCompleted (Touch.Action action, Touch touch);
        }

        private class SingleTapChallenge : TapChallenge {
            private Box _Box;
            public Box Box { get { return _Box; } }

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

            public Box Box { get { return clickedOnce ? smallBox : bigBox; } }

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

        private class PullTapChallenge : TapChallenge {
            private Box _GoalBox;
            public Box GoalBox { get { return _GoalBox; } }

            private Box _ScorerBox;
            public Box ScorerBox { get { return _ScorerBox; } }

            private Vector2 dragOffset;
            private Vector2 sizeDiff;
            private bool dragging = false;
            private float collisionThreshold;

            public PullTapChallenge (Vector2 containerLocation, Vector2 containerSize, float relativeTargetSize) : base(ChallengeType.PullTap) {
                Vector2 pullVector = Vector2.FromPolar(0.4f * containerSize.X, Mathf.Random(0, 360));
                Vector2 goalSize = new Vector2(relativeTargetSize * containerSize.X, relativeTargetSize * containerSize.X);
                Vector2 scorerSize = new Vector2(0.9f * relativeTargetSize * containerSize.X, relativeTargetSize * containerSize.X);
                sizeDiff = (goalSize - scorerSize) / 2f;

                collisionThreshold = (goalSize.X / 2f + scorerSize.X / 2f) / 2f;
                collisionThreshold *= collisionThreshold;

                float xmin, xmax;
                if (pullVector.X < 0) {
                    xmin = scorerSize.X / 2f - pullVector.X;
                    xmax = containerSize.X - goalSize.X / 2f;
                } else {
                    xmin = goalSize.X / 2f;
                    xmax = containerSize.X - scorerSize.X / 2f - pullVector.X;
                }

                float ymin, ymax;
                if (pullVector.Y < 0) {
                    ymin = goalSize.Y / 2f;
                    ymax = containerSize.Y - scorerSize.Y / 2f + pullVector.Y;
                } else {
                    ymin = scorerSize.Y / 2f + pullVector.Y;
                    ymax = containerSize.Y - goalSize.Y / 2f;
                }

                Vector2 position = new Vector2(Mathf.Random(xmin, xmax), -Mathf.Random(ymin, ymax));// -Mathf.Random(Math.Max(0, pullVector.Y), containerSize.Y - goalSize.Y + Math.Max(0, -pullVector.Y)));
                position += containerLocation;

                Vector2 goalPosition = position + new Vector2(-goalSize.X / 2f, goalSize.Y / 2f);
                Vector2 scorerPosition = position + pullVector + new Vector2(-scorerSize.X / 2f, scorerSize.Y / 2f);
                Vector2 diff = goalPosition - scorerPosition;

                _GoalBox = new Box(goalPosition, goalSize);
                _ScorerBox = new Box(scorerPosition, scorerSize, (float)Math.Atan2(diff.Y, diff.X) * 180 / Mathf.PI - 90);
            }

            public override bool IsCompleted (Touch.Action action, Touch touch) {
                switch (action) {
                    case Touch.Action.Begin:
                        if (_ScorerBox.Collides(touch.RelativePosition)) {
                            dragOffset = _ScorerBox.Position - touch.RelativePosition;
                            dragging = true;
                            IsDirty = true;
                        }
                        break;
                    case Touch.Action.Move:
                        if (dragging) {
                            _ScorerBox.Position = touch.RelativePosition + dragOffset;
                            IsDirty = true;
                            Vector2 diff = _GoalBox.Position - _ScorerBox.Position + sizeDiff;
                            _ScorerBox.Rotation = (float)Math.Atan2(diff.Y, diff.X) * 180 / Mathf.PI - 90;
                            return diff.X * diff.X + diff.Y * diff.Y < collisionThreshold;
                        }
                        return false;
                    case Touch.Action.End:
                        dragging = false;
                        IsDirty = true;
                        break;
                }
                return false;
            }
        }
    }
}
