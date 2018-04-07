using System;
using System.Diagnostics;
using Core;
using Universal.Graphics;
using Universal.Graphics.Programs;
using OpenTK.Graphics.ES20;
using Core.Graphics;
using Universal.Graphics.Renderer;
using AndroidPlatform;
using Universal.Data;

namespace Universal {
    public static class Manager {
        public static LocalCopyState State;

        public static void Initialize ( ) {
            ColorProgram.Init( );
            MatrixProgram.Init( );
            FBOProgram.Init( );
            GaussianBlurProgram.Init( );
            DarkenProgram.Init( );
            UIAbilityIconProgram.Init( );
            LineProgram.Init( );

            //LightManager.Init( );
            UIRenderer.SharedRenderer = new UIRenderer(Assets.GetSprite("interface"));
            EntityRenderer.Init( );

#if __ANDROID__
            State = new LocalCopyState(new AndroidDatabaseProvider((Android.App.Activity)Assets.Context));
#endif

            Screen.MainMenu.Load( );
            Screen.Active = Screen.MainMenu;

            Window.Background = new Color(225, 225, 225, 255);
        }
        
        public static void Update ( ) {
            Time.Update( );
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Screen.Active.Update(Time.FrameTime);
#if DEBUG
            Time.UpdateFinished( );
#endif

            Screen.Active.Draw( );

#if DEBUG
            Time.DrawFinished( );
#endif
        }

        public static void Destroy ( ) {
            Assets.Destroy( );
            Screen.MainMenu.Dispose( );
            UIRenderer.SharedRenderer.Dispose( );
            //LightManager.Destroy( );
            State.Dispose( );

            ColorProgram.Destroy( );
            MatrixProgram.Destroy( );
            FBOProgram.Destroy( );
            GaussianBlurProgram.Destroy( );
            DarkenProgram.Destroy( );
            UIAbilityIconProgram.Destroy( );
            LineProgram.Destroy( );
        }
    }
}
