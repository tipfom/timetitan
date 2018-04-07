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
        public MainMenuScreen ( ) {
            Label timeLabel = new Label(this, new Container(new Margin(0f, 0f, 0.05f, 0f), MarginType.Absolute, Position.Top | Position.Center, Position.Top | Position.Center), 0.2f, "TIME", new Color(18, 196, 98), 10, Label.TextAlignment.Center);
            Label titanLabel = new Label(this, new Container(new Margin(0f, 0f, 0.05f, 0f), MarginType.Absolute, anchor: Position.Top | Position.Center, dock: Position.Bottom | Position.Center, relative: timeLabel), 0.2f, "TITAN", new Color(18, 196, 98), 10, Label.TextAlignment.Center);

            Label stageLabel = new Label(this, new Container(new Margin(0.05f, 0.05f), MarginType.Absolute, anchor: Position.Bottom | Position.Right, dock: Position.Right | Position.Bottom), 0.1f, Manager.State.Stage.ToString( ), new Color(100, 100, 100), 10, Label.TextAlignment.Right);
            Manager.State.StageChanged += (newStage) => {
                stageLabel.Text = newStage.ToString( );
            };

            CoinLabel goldLabel = new CoinLabel(this, new Container(new Margin(0f, 0.025f), MarginType.Absolute, anchor: Position.Bottom | Position.Right, dock: Position.Right | Position.Top, relative: stageLabel), 0.1f, 10, Label.TextAlignment.Right);

            Label damageLabel = new Label(this, new Container(new Margin(0f, 0.025f), MarginType.Absolute, anchor: Position.Bottom | Position.Right, dock: Position.Right | Position.Top, relative: goldLabel), 0.1f, Manager.State.Damage.ToString("0.0"), new Color(100, 100, 100), 10, Label.TextAlignment.Right);
            Manager.State.DamageChanged += (newDamage) => {
                damageLabel.Text = newDamage.ToString("0.0");
            };

            PlayButton playButton = new PlayButton(this, new Container(new Margin(0.15f, 0.15f, 22f / 14f * 0.15f, 22f / 14f * 0.15f), MarginType.Absolute, Position.Center, Position.Center), 0);
            playButton.Release += ( ) => {
                GameplayScreen gameplayScreen = new GameplayScreen( );
                gameplayScreen.Load( );
                Screen.Active = gameplayScreen;
            };

            Button increaseDamageButton = new Button(this, new Container(new Margin(0f, 0.4f, 0.4f, 0f), MarginType.Absolute, Position.Left | Position.Bottom, Position.Left | Position.Bottom), "inc dmg\n4\n1000gold\n(btn is a\nplaceholder)", 0.05f, 10, Color.White);
            increaseDamageButton.Click += ( ) => {
                Manager.State.Damage *= 1.05f;
                Manager.State.Gold -= 1000;
            };
        }
    }
}
