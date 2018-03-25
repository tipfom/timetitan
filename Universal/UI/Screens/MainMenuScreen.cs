using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Universal.Graphics;
using Universal.UI.Animations;
using Universal.UI.Elements;
using Universal.UI.Layout;

namespace Universal.UI.Screens {
    public class MainMenuScreen : Screen {
        private ChangeNumericTextAnimation goldLabelAnimation;

        public MainMenuScreen ( ) {
            Label timeLabel = new Label(this, new Container(new Margin(0f, 0f, 0.05f, 0f), MarginType.Absolute, Position.Center, Position.Top | Position.Center), Depth.Foreground, 0.2f, new Color(18, 196, 98), "TIME", Label.TextAlignment.Center);
            Label titanLabel = new Label(this, new Container(new Margin(0f, 0f, 0.27f, 0f), MarginType.Absolute, Position.Center, Position.Top | Position.Center), Depth.Foreground, 0.2f, new Color(18, 196, 98), "TITAN", Label.TextAlignment.Center);

            Label highscoreLabel = new Label(this, new Container(new Margin(0.05f, 0.05f), MarginType.Absolute, anchor: Position.Bottom | Position.Right, dock: Position.Right | Position.Bottom), Depth.Foreground, 0.1f, new Color(18, 196, 98), Manager.StateManager.Highscore.ToString( ), Label.TextAlignment.Right);
            Manager.StateManager.HighscoreChanged += (newHighscore) => {
                highscoreLabel.Text = newHighscore.ToString( );
            };

            Label goldLabel = new Label(this, new Container(new Margin(0f, 0.025f), MarginType.Absolute, anchor: Position.Bottom | Position.Right, dock: Position.Right | Position.Top, relative: highscoreLabel), Depth.Foreground, 0.1f, new Color(255, 223, 0), Manager.StateManager.State.Gold.ToString( ), Label.TextAlignment.Right);
            Manager.StateManager.State.GoldChanged += (newGold) => {
                goldLabelAnimation = new ChangeNumericTextAnimation(goldLabel, (int)newGold, 0.3f);
            };

            PlayButton playButton = new PlayButton(this, new Container(new Margin(0.15f, 0.15f, 22f / 14f * 0.15f, 22f / 14f * 0.15f), MarginType.Absolute, Position.Center, Position.Center), Depth.Center);
            playButton.Release += ( ) => {
                GameplayScreen gameplayScreen = new GameplayScreen( );
                gameplayScreen.Load( );
                Screen.Active = gameplayScreen;
            };

            LeaderboardButton leaderboardButton = new LeaderboardButton(this, new Container(new Margin(0.05f, .2f, .2f * 23f / 19f, 0.05f), MarginType.Absolute, Position.Bottom | Position.Left, Position.Bottom | Position.Left), Depth.Foreground);
        }

        public override void Update (DeltaTime dt) {
            base.Update(dt);
            if(goldLabelAnimation != null) {
                if (goldLabelAnimation.Update(dt)) {
                    goldLabelAnimation = null;
                }
            }
        }
    }
}
