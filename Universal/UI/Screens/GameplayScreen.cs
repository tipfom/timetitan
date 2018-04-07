using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Universal.Graphics;
using Universal.Graphics.Renderer;
using Universal.UI.Animations;
using Universal.UI.Elements;
using Universal.UI.Elements.Challenges;
using Universal.UI.Layout;
using Universal.World;
using Universal.World.Mobs;

namespace Universal.UI.Screens {
    public class GameplayScreen : Screen {
        public const float MULTIPLIER_DECLINE = 0.9f;

        private List<Mob> mobs = new List<Mob>( );

        private Map map;
        private TargetArea targetArea;
        private ProgressBar multiplierBar;
        private ProgressBar stageProgressBar;
        private ProgressBar healthLeftBar;
        private Countdown countdown;
        private Aligner goldLabelAligner;
        private Aligner damageLabelAligner;
        private Swoosh swoosh;
        private List<FadeTextAnimation> textAnimations = new List<FadeTextAnimation>( );

        private float _multiplier = 1f;
        private float multiplier { get { return _multiplier; } set { _multiplier = Math.Max(1f, value); } }

        private bool playing;

        public override void Load ( ) {
            base.Load( );

            map = new Map(this, new Container(new Margin(0f, 1f, 0f, .3f), MarginType.Relative, Position.Left | Position.Top), -10);

            stageProgressBar = new ProgressBar(this, new Container(new Margin(0f, 1f, 0.9875f, 0.0125f), MarginType.Relative), 10) { Max = 5, Value = 1 };
            multiplierBar = new ProgressBar(this, new Container(new Margin(0, 1f, 0, 0.0125f), MarginType.Relative), 50) { Max = 10, Value = multiplier, Color = Color.Gold };
            healthLeftBar = new ProgressBar(this, new Container(new Margin(0f, 1f, 0.3f, 0.05f), MarginType.Relative), 50) { Max = 1, Value = 1, Color = new Color(255, 20, 20, 255) };

            goldLabelAligner = new Aligner(this, new Container(new Margin(0.2f, 0f, 0f, 0f), MarginType.Absolute, Position.Left | Position.Top, Position.Left | Position.Top));
            damageLabelAligner = new Aligner(this, new Container(new Margin(0, 0.2f, 0f, 0f), MarginType.Absolute, Position.Right | Position.Top, Position.Right | Position.Top));

            swoosh = new Swoosh(this, new Container(new Margin(0.4f, 0.4f, 0.05f, 0.6f), MarginType.Absolute, Position.Top, Position.Top), 1);

            CoinLabel goldLabel = new CoinLabel(this, new Container(new Margin(0, 0.025f), MarginType.Absolute, anchor: Position.Center | Position.Top, dock: Position.Center | Position.Bottom, relative: multiplierBar), 0.05f, 75, Label.TextAlignment.Left);

            Label stageLabel = new Label(this, new Container(new Margin(0.025f, 0.025f), MarginType.Absolute, anchor: Position.Top | Position.Right, dock: Position.Right | Position.Bottom, relative: multiplierBar), 0.09f, Manager.State.Stage.ToString( ), new Color(100, 100, 100, 100), 75, Label.TextAlignment.Right);
            Manager.State.StageChanged += (newStage) => {
                stageLabel.Text = newStage.ToString( );
            };

            targetArea = new TargetArea(this, new Container(new Margin(0f, 1f, 0.35f, 0.625f), MarginType.Relative, Position.Left | Position.Top), 0, ChallengeProgressCallback, GetTapChallenge);

            countdown = new Countdown(this, new Container(new Margin(0f, 1f, 0.35f, 0.625f), MarginType.Relative), 10, 0.2f, 3);

            countdown.Finished += ( ) => {
                Start( );
            };

            EntityRenderer.VertexSize = map.Container.Height / 3f;

            Prepare( );
            countdown.Start( );
        }

        protected override void DrawAdditionalContent ( ) {
            foreach (Mob mob in mobs)
                EntityRenderer.Draw(mob, map.MobPosition);
        }

        public override void Update (DeltaTime dt) {
            base.Update(dt);

            foreach (Mob mob in mobs) {
                mob.Update(dt);
            }

            for (int i = 0; i < textAnimations.Count; i++) {
                if (textAnimations[i].Update(dt)) {
                    textAnimations[i].Dispose( );
                    textAnimations.RemoveAt(i);
                    i--;
                }
            }

            if (playing) {
                multiplier *= (float)Math.Pow(MULTIPLIER_DECLINE, dt.Seconds);
                multiplierBar.Value = multiplier;
            }
        }

        private void Prepare ( ) {
            targetArea.Clear( );

            mobs.Clear( );
            mobs.Add(GetMob( ));

            healthLeftBar.Max = mobs[0].Health;
            healthLeftBar.Value = mobs[0].Health;
        }

        private void Start ( ) {
            targetArea.Start( );
            playing = true;
        }

        //private void GameOver ( ) {
        //    targetArea.Stop( );
        //    playing = false;

        //    stageProgressBar.Value = 1;
        //    restartButton.Visible = true;
        //    leaderboardButton.Visible = true;
        //}

        private void ChallengeProgressCallback (bool isHit, ChallengeType type) {
            if (isHit) {
                swoosh.Appear( );
                float damage = Manager.State.Damage;
                textAnimations.Add(new FadeTextAnimation(new Label(this, new Container(new Margin(0, 0, map.MobPosition.Y - 0.075f, 0), MarginType.Absolute, anchor: Position.Center | Position.Top, dock: Position.Right | Position.Top, relative: damageLabelAligner), 0.05f, damage.ToString( ), alignment: Label.TextAlignment.Center), 0.05f, 0.75f));
                if ((mobs[0].Health -= damage) < 0) {
                    mobs[0].Die(null);

                    Next( );
                }

                healthLeftBar.Value = mobs[0].Health;
                multiplier += 0.2f;
            } else {
                // heartViewer.Active--;
                // if (heartViewer.Active == 0) {
                //     GameOver( );
                // }
                multiplier -= 0.5f;
            }
        }

        private void Next ( ) {
            int gold = (int)(mobs[0].Value * multiplier);
            textAnimations.Add(new FadeTextAnimation(new Label(this, new Container(new Margin(0, 0, map.MobPosition.Y - 0.075f, 0), MarginType.Absolute, anchor: Position.Center | Position.Top, dock: Position.Left | Position.Top, relative: goldLabelAligner), 0.075f, "+" + gold.ToString( ), Color.Gold, 75, Label.TextAlignment.Center), 0.075f, 0.5f));
            Manager.State.Gold += gold;

            mobs.Insert(0, GetMob( ));
            healthLeftBar.Max = mobs[0].Health;
            healthLeftBar.Value = mobs[0].Health;

            stageProgressBar.Value++;
            if (stageProgressBar.Value > stageProgressBar.Max) {
                stageProgressBar.Value = 1;
                Manager.State.Stage++;
            }
        }

        private Mob GetMob ( ) {
            float random = Mathf.Random( );
            if (random < 0.7f) {
                return new Octopus(Manager.State.Stage);
            } else {
                return new Chest(Manager.State.Stage);
            }
        }

        private TapChallenge GetTapChallenge ( ) {
            float random = Mathf.Random( );
            if (random > 0.9f) {
                return new PullTapChallenge(targetArea.Container.Location, targetArea.Container.Size, 0.2f);
            } else if (random > 0.65f) {
                return new DoubleTapChallenge(targetArea.Container.Location, targetArea.Container.Size, 0.2f);
            } else {
                return new SingleTapChallenge(targetArea.Container.Location, targetArea.Container.Size, 0.2f);
            }
        }
    }
}
