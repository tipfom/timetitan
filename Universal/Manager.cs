using System;
using System.Diagnostics;
using Core;
using Universal.Graphics;
using Universal.Graphics.Programs;
using OpenTK.Graphics.ES20;
using Core.Graphics;
using Universal.Graphics.Renderer;

namespace Universal {
    public static class Manager {
        public static ILeaderboard Leaderboard;

        public static void Initialize ( ) {
            ColorProgram.Init( );
            MatrixProgram.Init( );
            FBOProgram.Init( );
            GaussianBlurProgram.Init( );
            DarkenProgram.Init( );
            UIAbilityIconProgram.Init( );
            LineProgram.Init( );

            //LightManager.Init( );
            UIRenderer.Init( );
            UIRenderer.Texture = Assets.GetSprite("interface");
            EntityRenderer.Init( );

            Screen.MainMenu.Load( );
            Screen.Active = Screen.MainMenu;

#if __ANDROID__
            Leaderboard = new Android.AndroidLeaderboard((Android.App.Activity)Assets.Context);
#endif

            Window.Background = new Color(25, 25, 50, 255);
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
            UIRenderer.Dispose( );
            //LightManager.Destroy( );

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
