using System;
using System.Collections.Generic;
using Core;
using Universal.Graphics;
using Universal.Graphics.Renderer;
using Universal.UI;
using Universal.UI.Screens;
using static Universal.Graphics.Renderer.UIRenderer;

namespace Universal {

    public class Screen : IDisposable {
        private static Screen _Active;
        public static Screen Active { get { return _Active; } set { SharedRenderer.Prepare(value); _Active.IsActive = false; value.IsActive = true; value.Activated( ); _Active = value; } }

        public static MainMenuScreen MainMenu;

        public List<Element> Elements = new List<Element>( );

        static Screen ( ) {
            MainMenu = new MainMenuScreen( );
            _Active = MainMenu;
        }

        public bool IsActive { get; private set; }

        public virtual void Dispose ( ) {
        }

        public void Draw ( ) {
            SharedRenderer.PreDraw( );
            DrawAdditionalContent( );
            SharedRenderer.PostDraw( );
        }

        protected virtual void DrawAdditionalContent ( ) {
        }

        public virtual void Load ( ) {
        }

        public virtual void Update (DeltaTime dt) {
            for (int i = 0; i < Elements.Count; i++)
                Elements[i].Update(dt);
            SharedRenderer.Update( );
        }

        public void AddElement (Element element) {
            Elements.Add(element);
            if (IsActive) SharedRenderer.Add(element);
        }

        protected virtual void Activated ( ) {
        }
    }
}