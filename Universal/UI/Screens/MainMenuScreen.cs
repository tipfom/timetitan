using System;
using System.Collections.Generic;
using System.Text;
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
        }
    }
}
