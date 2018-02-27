using System;
using Core;
using Universal.Graphics;
using Universal.Graphics.Renderer;
using Universal.UI.Screens;

namespace Universal {

    public class Screen : IDisposable {
        private static Screen _Active;
        public static Screen Active { get { return _Active; } set { UIRenderer.Prepare(value); _Active.IsActive = false; value.IsActive = true; value.Activated( ); _Active = value; } }

        public static MainMenuScreen MainMenu;

        static Screen ( ) {
            MainMenu = new MainMenuScreen( );
            _Active = MainMenu;
        }

        public bool IsActive { get; private set; }

        public virtual void Dispose ( ) {
            UIRenderer.Delete(this);
        }

        public virtual void Draw ( ) {
            UIRenderer.Draw( );
        }

        public virtual void Load ( ) {
        }

        public virtual void Update (DeltaTime dt) {
            UIRenderer.Update(dt);
        }

        protected virtual void Activated ( ) {
        }
    }
}