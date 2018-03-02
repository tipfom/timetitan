using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Universal.Graphics;
using Universal.Graphics.Renderer;
using Universal.UI.Elements;
using Universal.UI.Layout;
using Universal.World;

namespace Universal.UI.Screens {
    public class GameplayScreen : Screen {
        private Player player;
        private List<Mob> mobs = new List<Mob>( );

        private Map map;
        private TargetArea targetArea;
        private ProgressBar timeLeftBar;
        private ProgressBar hitsLeftBar;
        private Label scoreLabel;
        private Countdown countdown;
        private Button restartButton;
        private LeaderboardButton leaderboardButton;

        private int score = 0;
        private int maxTime = 10000;
        private int startTime = 0;
        private bool finished = true;

        public override void Load ( ) {
            base.Load( );

            player = new Player( );

            map = new Map(this, new Container(new Margin(0f, 1f, 0f, .3f), MarginType.Relative, Position.Left | Position.Top), Depth.Center);
            scoreLabel = new Label(this, new Container(new Margin(0.025f, 0.025f), MarginType.Absolute), Depth.Foreground, 0.1f, score.ToString( ));

            Label highscoreLabel = new Label(this, new Container(new Margin(0.025f, 0f), MarginType.Absolute, dock: Position.Right | Position.Top, relative: scoreLabel), Depth.Foreground, 0.05f, new Color(255, 255, 255, 127), Manager.Leaderboard.Highscore.ToString( ), Label.TextAlignment.Right);
            Manager.Leaderboard.HighscoreChanged += (newHighscore) => {
                highscoreLabel.Text = newHighscore.ToString( );
            };


            timeLeftBar = new ProgressBar(this, new Container(new Margin(0f, 1f, 0.3f, 0.05f), MarginType.Relative), Depth.Foreground) { Max = maxTime, Value = maxTime, Color = new Color(255, 20, 20, 255) };
            hitsLeftBar = new ProgressBar(this, new Container(new Margin(0f, 1f, 0.9875f, 0.0125f), MarginType.Relative), Depth.Foreground) { Max = 10, Value = 10 };

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

            if (!finished) {
                timeLeftBar.Value = Math.Max(0, maxTime - (Environment.TickCount - startTime));
                if (timeLeftBar.Value == 0) {
                    Finished( );
                }
            }

            player.Update(dt);
            foreach (Mob mob in mobs) {
                mob.Update(dt);
            }
        }

        private void Prepare ( ) {
            maxTime = 10000;
            score = 0;

            timeLeftBar.Max = maxTime;
            timeLeftBar.Value = maxTime;
            timeLeftBar.Visible = true;
            hitsLeftBar.Max = 10;
            hitsLeftBar.Value = 10;
            hitsLeftBar.Visible = true;
            scoreLabel.Text = score.ToString( );
            restartButton.Visible = false;
            leaderboardButton.Visible = false;
            targetArea.Clear( );

            mobs.Clear( );
            mobs.Add(new Mob(Entity.PLUGGER));
        }

        private void Start ( ) {
            targetArea.Challenge(7, 2, 0.22f, ChallengeProgressCallback);
            startTime = Environment.TickCount;
            finished = false;
        }

        private void Finished ( ) {
            finished = true;
            targetArea.Stop( );
            Manager.Leaderboard.SubmitToLeaderboard(score);

            restartButton.Visible = true;
            timeLeftBar.Visible = false;
            hitsLeftBar.Visible = false;
            leaderboardButton.Visible = true;
        }

        private void ChallengeProgressCallback (int targetsLeft) {
            hitsLeftBar.Value = targetsLeft;
            if (targetsLeft == 0 && !finished) {
                mobs[0].Die(null);

                Next( );
            }
        }

        private void Next ( ) {
            mobs.Insert(0, new Mob(Entity.PLUGGER));
            targetArea.Challenge(7, 2, 0.22f, ChallengeProgressCallback);
            hitsLeftBar.Value = 10;
            maxTime = maxTime * 95 / 100;
            startTime = Environment.TickCount;
            score++;
            scoreLabel.Text = score.ToString( );

            timeLeftBar.Max = maxTime;
            timeLeftBar.Value = maxTime;
        }
    }
}
