using Android.App;
using Android.Content.PM;
using Android.OS;
using Universal;

namespace Android {
    [Activity(
        Label = "@string/app_name", 
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, 
        ScreenOrientation = ScreenOrientation.SensorLandscape, 
        Theme = "@style/thisTheme",
        Icon = "@drawable/icon", 
        HardwareAccelerated = true,
        MainLauncher = true)]
    public class MainActivity : Activity {
        protected override void OnCreate (Bundle bundle) {
            base.Window.DecorView.SystemUiVisibility = Constants.STATUS_BAR_VISIBILITY;
            base.Window.DecorView.SystemUiVisibilityChange += (sender, e) => {
                if (Window.DecorView.SystemUiVisibility != Constants.STATUS_BAR_VISIBILITY) 
                    Window.DecorView.SystemUiVisibility = Constants.STATUS_BAR_VISIBILITY;
            };
            base.OnCreate(bundle);

            // Create our OpenGL view, and display it
            Universal.Assets.Context = this;

            View view = new View(this);
            view.SetOnTouchListener(TouchHandler.Instance);
            SetContentView(view);
        }

        protected override void OnDestroy ( ) {
            Manager.Destroy( );
            base.OnDestroy( );
        }
    }
}
