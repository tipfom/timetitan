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

        private Player player;
        private List<Mob> mobs = new List<Mob>( );

        private Map map;
        private TargetArea targetArea;
        private ProgressBar multiplierBar;
        private ProgressBar stageProgressBar;
        private ProgressBar healthLeftBar;
        private Countdown countdown;
        private Button restartButton;
        private LeaderboardButton leaderboardButton;
        private HeartViewer heartViewer;
        private List<FadeTextAnimation> textAnimations = new List<FadeTextAnimation>( );

        private ChangeNumericTextAnimation goldLabelAnimation;

        private float multiplier = 1f;

        public override void Load ( ) {
            base.Load( );

            player = new Player( );

            map = new Map(this, new Container(new Margin(0f, 1f, 0f, .3f), MarginType.Relative, Position.Left | Position.Top), Depth.Center);

            stageProgressBar = new ProgressBar(this, new Container(new Margin(0f, 1f, 0.9875f, 0.0125f), MarginType.Relative), Depth.Foreground) { Max = 5, Value = 1 };
            multiplierBar = new ProgressBar(this, new Container(new Margin(0, 1f, 0, 0.0125f), MarginType.Relative), Depth.Foreground) { Max = 10, Value = multiplier, Color = Color.Gold };
            healthLeftBar = new ProgressBar(this, new Container(new Margin(0f, 1f, 0.3f, 0.05f), MarginType.Relative), Depth.Foreground) { Max = 1, Value = 1, Color = new Color(255, 20, 20, 255) };

            heartViewer = new HeartViewer(this, new Container(new Margin(0.025f, 0.09f, 0.0125f, 0.09f), MarginType.Absolute, dock: Position.Bottom | Position.Left, relative: multiplierBar), Depth.Foreground, 3);

            Label goldLabel = new Label(this, new Container(new Margin(0, 0.025f), MarginType.Absolute, anchor: Position.Center | Position.Top, dock: Position.Center | Position.Bottom, relative: multiplierBar), Depth.Foreground, 0.05f, Color.Gold, Manager.State.Gold.ToString( ), Label.TextAlignment.Left);
            Manager.State.GoldChanged += (newGold) => {
                goldLabelAnimation = new ChangeNumericTextAnimation(goldLabel, (int)newGold, 0.3f);
            };

            Label stageLabel = new Label(this, new Container(new Margin(0.025f, 0.025f), MarginType.Absolute, anchor: Position.Top | Position.Right, dock: Position.Right | Position.Bottom, relative: multiplierBar), Depth.Foreground, 0.09f, new Color(100, 100, 100, 100), Manager.State.Stage.ToString( ), Label.TextAlignment.Right);
            Manager.State.StageChanged += (newStage) => {
                stageLabel.Text = newStage.ToString( );
            };

            targetArea = new TargetArea(this, new Container(new Margin(0f, 1f, 0.35f, 0.625f), MarginType.Relative, Position.Left | Position.Top), Depth.Center, ChallengeProgressCallback, GetTapChallenge);

            countdown = new Countdown(this, new Container(new Margin(0f, 1f, 0.35f, 0.625f), MarginType.Relative), Depth.Foreground, 0.2f, 3);
            restartButton = new Button(this, new Container(new Margin(0.2f, 0.6f, 0.6f, 0.2f), MarginType.Relative), "RESTART", Depth.Foreground, Color.White) { Visible = false };
            leaderboardButton = new LeaderboardButton(this, new Container(new Margin(0.15f, 0.05f, 0.15f * 23f / 19f, 0.05f), MarginType.Absolute, Position.Right | Position.Bottom, Position.Right | Position.Bottom), Depth.Foreground) { Visible = false };

            countdown.Finished += ( ) => {
                Start( );
            };

            restartButton.Release += ( ) => {
                Prepare( );
                countdown.Start( );
            };

            EntityRenderer.VertexSize = map.Container.Height / 3f;

            Prepare( );
            countdown.Start( );
        }

        public override void Draw ( ) {
            base.Draw( );

            EntityRenderer.Draw(player, map.PlayerPosition);
            foreach (Mob mob in mobs)
                EntityRenderer.Draw(mob, map.MobPosition);
        }

        public override void Update (DeltaTime dt) {
            base.Update(dt);

            player.Update(dt);
            foreach (Mob mob in mobs) {
                mob.Update(dt);
            }

            if (goldLabelAnimation != null) {
                if (goldLabelAnimation.Update(dt)) {
                    goldLabelAnimation = null;
                }
            }

            for (int i = 0; i < textAnimations.Count; i++) {
                if (textAnimations[i].Update(dt)) {
                    textAnimations.RemoveAt(i);
                    i--;
                }
            }

            multiplier = Math.Max(1, multiplier * (float)Math.Pow(MULTIPLIER_DECLINE, dt.TotalSeconds));
            multiplierBar.Value = multiplier;
        }

        private void Prepare ( ) {
            restartButton.Visible = false;
            leaderboardButton.Visible = false;
            heartViewer.Active = heartViewer.Count;
            targetArea.Clear( );

            mobs.Clear( );
            mobs.Add(GetMob( ));

            healthLeftBar.Max = mobs[0].Health;
            healthLeftBar.Value = mobs[0].Health;
        }

        private void Start ( ) {
            targetArea.Start( );
        }

        private void GameOver ( ) {
            targetArea.Stop( );

            stageProgressBar.Value = 1;
            restartButton.Visible = true;
            leaderboardButton.Visible = true;
        }

        private void ChallengeProgressCallback (bool isHit, ChallengeType type) {
            if (isHit) {
                player.Attack( );
                float damage = player.GetDamage(type);
                textAnimations.Add(new FadeTextAnimation(new Label(this, new Container(new Margin(0, 0, map.MobPosition.Y - 0.075f, 0), MarginType.Absolute, anchor: Position.Center | Position.Top, dock: Position.Center | Position.Top), 0.05f, damage.ToString( ), Label.TextAlignment.Center), 0.05f, 0.75f));
                if ((mobs[0].Health -= damage) < 0) {
                    mobs[0].Die(null);

                    Next( );
                }

                healthLeftBar.Value = mobs[0].Health;
                multiplier += 0.2f;
            } else {
                heartViewer.Active--;
                if (heartViewer.Active == 0) {
                    GameOver( );
                }
            }
        }

        private void Next ( ) {
            int gold = (int)(mobs[0].Value * multiplier);
            textAnimations.Add(new FadeTextAnimation(new Label(this, new Container(new Margin(0, 0, map.MobPosition.Y - 0.075f, 0), MarginType.Absolute, anchor: Position.Center | Position.Top, dock: Position.Center | Position.Top), Depth.Foreground, 0.075f, Color.Gold, "+"+gold.ToString( ), Label.TextAlignment.Center), 0.075f, 0.5f));
            Manager.State.Gold += gold;

            mobs.Insert(0, GetMob( ));
            healthLeftBar.Max = mobs[0].Health;
            healthLeftBar.Value = mobs[0].Health;

            stageProgressBar.Value++;
            if (stageProgressBar.Value > stageProgressBar.Max) {
                stageProgressBar.Value = 1;
                Manager.State.Stage++;
                heartViewer.Active = heartViewer.Count;
            }
        }

        private Mob GetMob ( ) {
            float random = Mathf.Random( );
            if (random < 0.4f) {
                return new Plugger( );
            } else if (random < 0.7f) {
                return new Octopus( );
            } else {
                return new Chest( );
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
