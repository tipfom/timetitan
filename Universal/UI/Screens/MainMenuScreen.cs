using System;
using System.Collections.Generic;
using System.Text;
using Universal.Graphics;
using Universal.UI.Elements;
using Universal.UI.Layout;

namespace Universal.UI.Screens {
    public class MainMenuScreen : Screen {
        public MainMenuScreen ( ) {
            Button playButton = new Button(this, new Container(new Margin(.7f,.4f), MarginType.Absolute, Position.Center, Position.Center), "PLAY");
            playButton.Release += ( ) => {
                global::Android.Widget.Toast.MakeText(Assets.Context, "hallO", global::Android.Widget.ToastLength.Long).Show( );
            };
        }
    }
}
