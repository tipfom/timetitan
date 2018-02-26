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
        private Label stageLabel;
        private Countdown countdown;

        private int stage = 0;
        private int maxTime = 10000;
        private int startTime = 0;
        private bool finished = true;

        public override void Load ( ) {
            base.Load( );

            map = new Map(this, new Container(new Margin(0f, 1f, 0f, .3f), MarginType.Relative, Position.Left | Position.Top), Depth.Center);
            timeLeftBar = new ProgressBar(this, new Container(new Margin(0f, 1f, 0.3f, 0.025f), MarginType.Relative), Depth.Foreground) { Max = maxTime };
            hitsLeftBar = new ProgressBar(this, new Container(new Margin(0f, 1f, 0.325f, 0.025f), MarginType.Relative), Depth.Foreground) { Max = 10 };
            targetArea = new TargetArea(this, new Container(new Margin(0f, 1f, 0.35f, 0.65f), MarginType.Relative, Position.Left | Position.Top), Depth.Center);
            stageLabel = new Label(this, new Container(new Margin(0.05f, 0.05f), MarginType.Relative), Depth.Foreground, 0.1f, "Stage: " + stage);
            countdown = new Countdown(this, new Container(new Margin(0f, 1f, 0.35f, 0.65f), MarginType.Relative), Depth.Foreground, 0.2f, 3);

            countdown.Finished += ( ) => {
                targetArea.Challenge(10, 0.2f, ChallengeProgressCallback);
                startTime = Environment.TickCount;
                finished = false;
            };

            player = new Player( );
            mobs.Add(new Mob(Entity.PLUGGER));

            EntityRenderer.VertexSize = map.Container.Height / 3f;

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
                timeLeftBar.Value = maxTime - (Environment.TickCount - startTime);
                if (timeLeftBar.Value < 0) {
                    new Label(this, new Container(new Margin(0, 0), MarginType.Absolute), .1f, "GAME OVER");
                    timeLeftBar.Value = 0;
                    finished = true;
                    targetArea.Stop( );
                    Manager.lb.SubmitToLeaderboards(stage);
                }
            }

            player.Update(dt);
            foreach (Mob mob in mobs) {
                mob.Update(dt);
            }
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
            targetArea.Challenge(10, 0.2f, ChallengeProgressCallback);
            hitsLeftBar.Value = 10;
            maxTime = maxTime * 95 / 100;
            startTime = Environment.TickCount;
            stage++;
            stageLabel.Text = "Stage: " + stage;

            timeLeftBar.Max = maxTime;
            timeLeftBar.Value = maxTime;
        }
    }
}
