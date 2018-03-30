using Core;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.UI.Layout;
using Universal.World;

namespace Universal.UI.Elements.Challenges {
    public class PullTapChallenge : TapChallenge {
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

        public override UpdateAction Update (Touch.Action action, Touch touch) {
            switch (action) {
                case Touch.Action.Begin:
                    if (_ScorerBox.Collides(touch.RelativePosition)) {
                        dragOffset = _ScorerBox.Position - touch.RelativePosition;
                        dragging = true;
                        IsDirty = true;
                        return UpdateAction.Ignore;
                    } else {
                        return UpdateAction.Miss;
                    }
                case Touch.Action.Move:
                    if (dragging) {
                        _ScorerBox.Position = touch.RelativePosition + dragOffset;
                        IsDirty = true;
                        Vector2 diff = _GoalBox.Position - _ScorerBox.Position + sizeDiff;
                        _ScorerBox.Rotation = (float)Math.Atan2(diff.Y, diff.X) * 180 / Mathf.PI - 90;
                        if (diff.X * diff.X + diff.Y * diff.Y < collisionThreshold) {
                            return UpdateAction.Complete;
                        }
                    }
                    return UpdateAction.Ignore;
                case Touch.Action.End:
                    dragging = false;
                    IsDirty = true;
                    return UpdateAction.Ignore;
            }
            return UpdateAction.Ignore;
        }
    }
}
