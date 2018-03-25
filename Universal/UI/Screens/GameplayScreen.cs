﻿using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Universal.Graphics;
using Universal.Graphics.Renderer;
using Universal.UI.Animations;
using Universal.UI.Elements;
using Universal.UI.Layout;
using Universal.World;

namespace Universal.UI.Screens {
    public class GameplayScreen : Screen {
        public const int START_TIME = 10000;
        public const float MULTIPLIER_DECLINE = 0.9f;

        private Player player;
        private List<Mob> mobs = new List<Mob>( );

        private Map map;
        private TargetArea targetArea;
        private ProgressBar hitsLeftBar;
        private ProgressBar multiplierBar;
        private Countdown countdown;
        private Button restartButton;
        private LeaderboardButton leaderboardButton;
        private HeartViewer heartViewer;

        private ChangeNumericTextAnimation goldLabelAnimation;

        private float multiplier = 1f;

        public override void Load ( ) {
            base.Load( );

            player = new Player( );

            map = new Map(this, new Container(new Margin(0f, 1f, 0f, .3f), MarginType.Relative, Position.Left | Position.Top), Depth.Center);

            Label goldLabel = new Label(this, new Container(new Margin(0.025f, 0.025f), MarginType.Absolute, anchor: Position.Top | Position.Right, dock: Position.Right | Position.Top), Depth.Foreground, 0.05f, Color.Gold, Manager.StateManager.State.Gold.ToString( ), Label.TextAlignment.Right);
            Manager.StateManager.State.GoldChanged += (newGold) => {
                goldLabelAnimation = new ChangeNumericTextAnimation(goldLabel, (int)newGold, 0.3f);
            };


            hitsLeftBar = new ProgressBar(this, new Container(new Margin(0f, 1f, 0.9875f, 0.0125f), MarginType.Relative), Depth.Foreground) { Max = 10, Value = 10 };
            multiplierBar = new ProgressBar(this, new Container(new Margin(0, 1f, 0, 0.0125f), MarginType.Relative), Depth.Foreground) { Max = 10, Value = multiplier, Color = Color.Gold };

            heartViewer = new HeartViewer(this, new Container(new Margin(0.025f, 1f, 0.0125f, 0.075f), MarginType.Absolute, dock: Position.Bottom | Position.Left, relative: multiplierBar), Depth.Foreground, 3);

            targetArea = new TargetArea(this, new Container(new Margin(0f, 1f, 0.35f, 0.625f), MarginType.Relative, Position.Left | Position.Top), Depth.Center);

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

            multiplier = Math.Max(1, multiplier * (float)Math.Pow(MULTIPLIER_DECLINE, dt.TotalSeconds));
            multiplierBar.Value = multiplier;
        }

        private void Prepare ( ) {
            hitsLeftBar.Max = 10;
            hitsLeftBar.Value = 10;
            hitsLeftBar.Visible = true;
            restartButton.Visible = false;
            leaderboardButton.Visible = false;
            targetArea.Clear( );

            mobs.Clear( );
            mobs.Add(new Mob(Entity.PLUGGER));
        }

        private void Start ( ) {
            Challenge( );
        }

        private void Finished ( ) {
            targetArea.Stop( );

            restartButton.Visible = true;
            hitsLeftBar.Visible = false;
            leaderboardButton.Visible = true;

        }

        private void ChallengeProgressCallback (int targetsLeft) {
            hitsLeftBar.Value = targetsLeft;
            player.Attack( );
            if (targetsLeft == 0) {
                mobs[0].Die(null);

                Next( );
            }
            multiplier += 0.2f;
        }

        private void Next ( ) {
            Manager.StateManager.State.Gold += (int)(100 * multiplier);

            mobs.Insert(0, new Mob(Entity.PLUGGER));
            hitsLeftBar.Value = 10;

            Challenge( );
        }

        private void Challenge ( ) {
            //if (score < 10) {
            targetArea.Challenge(7, 0, 0, 0.22f, ChallengeProgressCallback);
            //} else if (score < 20) {
            //    targetArea.Challenge(7, 1, 0, 0.21f, ChallengeProgressCallback);
            //} else if (score < 30) {
            //    targetArea.Challenge(7, 1, 1, 0.20f, ChallengeProgressCallback);
            //} else if (score < 40) {
            //    targetArea.Challenge(7, 2, 1, 0.19f, ChallengeProgressCallback);
            //} else {
            //    targetArea.Challenge(8, 2, 1, 0.19f, ChallengeProgressCallback);
            //}
        }
    }
}
