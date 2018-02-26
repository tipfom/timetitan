using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Universal.Graphics;
using Universal.UI.Elements;
using Universal.UI.Layout;

namespace Universal.UI.Screens {
    public class MainMenuScreen : Screen {
        public MainMenuScreen ( ) {
            Button playButton = new Button(this, new Container(new Margin(.35f, .2f), MarginType.Absolute, Position.Center, Position.Center), "PLAY");
            playButton.Release += ( ) => {
                GameplayScreen gameplayScreen = new GameplayScreen( );
                gameplayScreen.Load( );
                Screen.Active = gameplayScreen;
            };

            Button leaderboardButton = new Button(this, new Container(new Margin(.4f,.4f,-0.4f, .2f), MarginType.Absolute, Position.Top, Position.Center), "LEADERBOARD", 0.1f, Depth.Foreground, Color.White);
            leaderboardButton.Release += ( ) => {
                Manager.Leaderboard.ShowLeaderboard( );
            };
        }
    }
}
