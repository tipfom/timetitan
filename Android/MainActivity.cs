using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Universal;

namespace AndroidPlatform {
    [Activity(
        Label = "@string/app_name",
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden,
        ScreenOrientation = ScreenOrientation.Portrait,
        Theme = "@style/thisTheme",
        Icon = "@drawable/armor256px",
        HardwareAccelerated = true,
        MainLauncher = true,
        ResizeableActivity = true,
        Immersive = true)]
    public class MainActivity : Activity {
        public View View;

        protected override void OnCreate (Bundle bundle) {
            Android.Widget.Toast.MakeText(this, "ACHTUNG: Wenn du dachtest das Spiel wäre vorher schon Ultra-Alpha, dann hast du das noch nicht gespielt. Wird bald besser.", Android.Widget.ToastLength.Long).Show( );

            base.Window.DecorView.SystemUiVisibility = Constants.STATUS_BAR_VISIBILITY;
            base.Window.DecorView.SystemUiVisibilityChange += (sender, e) => {
                if (Window.DecorView.SystemUiVisibility != Constants.STATUS_BAR_VISIBILITY)
                    Window.DecorView.SystemUiVisibility = Constants.STATUS_BAR_VISIBILITY;
            };
            base.OnCreate(bundle);

            // Create our OpenGL view, and display it
            Universal.Assets.Context = this;
            Universal.Assets.Icon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.armor256px);


            View = new View(this);
            View.SetOnTouchListener(TouchHandler.Instance);
            SetContentView(View);
        }

        protected override void OnDestroy ( ) {
            Manager.Destroy( );
            base.OnDestroy( );
        }

        protected override void OnActivityResult (int requestCode, [GeneratedEnum] Result resultCode, Intent data) {
            if (Manager.State != null) {
                ((AndroidDatabaseProvider)Manager.State.DatabaseProvider).OnActivityResult(requestCode, resultCode, data);
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}
