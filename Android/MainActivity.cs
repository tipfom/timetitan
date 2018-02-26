using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.OS;
using Android.Runtime;
using Universal;

namespace Android {
    [Activity(
        Label = "@string/app_name", 
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, 
        ScreenOrientation = ScreenOrientation.Portrait, 
        Theme = "@style/thisTheme",
        Icon = "@drawable/icon", 
        HardwareAccelerated = true,
        MainLauncher = true)]
    public class MainActivity : Activity {
        public View View;

        protected override void OnCreate (Bundle bundle) {
            base.Window.DecorView.SystemUiVisibility = Constants.STATUS_BAR_VISIBILITY;
            base.Window.DecorView.SystemUiVisibilityChange += (sender, e) => {
                if (Window.DecorView.SystemUiVisibility != Constants.STATUS_BAR_VISIBILITY) 
                    Window.DecorView.SystemUiVisibility = Constants.STATUS_BAR_VISIBILITY;
            };
            base.OnCreate(bundle);

            // Create our OpenGL view, and display it
            Universal.Assets.Context = this;

            View = new View(this);
            View.SetOnTouchListener(TouchHandler.Instance);
            SetContentView(View);
        }

        protected override void OnDestroy ( ) {
            Manager.Destroy( );
            base.OnDestroy( );
        }

        protected override void OnActivityResult (int requestCode, [GeneratedEnum] Result resultCode, Intent data) {
            if (Manager.Leaderboard != null) {
                ((AndroidLeaderboard)Manager.Leaderboard).OnActivityResult(requestCode, resultCode, data);
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}
