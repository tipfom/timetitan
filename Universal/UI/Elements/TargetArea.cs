﻿using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Universal.UI.Elements.Challenges;
using Universal.UI.Layout;
using Universal.World;

namespace Universal.UI.Elements {
    public class TargetArea : Element {
        private static readonly Color ACTIVE_SINGLETAP_COLOR = new Color(0, 204, 0, 255);
        private static readonly Color ACTIVE_DOUBLETAP_COLOR = new Color(204, 204, 0, 255);
        private static readonly Color[ ] PREVIEW_COLOR = new[ ] { new Color(144, 144, 144, 128), new Color(144, 144, 144, 32) };
        private static readonly Color FAILED_COLOR = new Color(204, 0, 0, 255);

        public delegate void HitCallback (bool isHit, int targetsLeft);

        private HitCallback hitCallback;

        private bool active;
        private List<TapChallenge> challenges = new List<TapChallenge>(10);

        public TargetArea (Screen owner, Container container, int depth) : base(owner, container, depth, false) {
        }

        public void Clear ( ) {
            challenges.Clear( );
            IsDirty = true;
        }

        public void Challenge (int singleTapChallengeCount, int doubleTapChallengeCount, int pullTapChallengeCount, float relativeTargetSize, HitCallback callback) {
            GenerateChallenges(singleTapChallengeCount, doubleTapChallengeCount, pullTapChallengeCount, relativeTargetSize);
            hitCallback = callback;
            active = true;
            IsDirty = true;
        }

        public void Stop ( ) {
            active = false;
            IsDirty = true;
        }

        public override bool HandleTouch (Touch.Action action, Touch touch) {
            if (active) {
                switch(challenges[0].Update(action, touch)) {
                    case UpdateAction.Complete:
                        UpdateTarget( );
                        hitCallback?.Invoke(true, challenges.Count);
                        break;
                    case UpdateAction.Ignore:
                        break;
                    case UpdateAction.Miss:
                        hitCallback?.Invoke(false, challenges.Count);
                        break;
                }
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
    }
}
