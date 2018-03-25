using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Drive;
using Android.Gms.Games;
using Android.Gms.Games.LeaderBoard;
using Android.Gms.Games.Snapshot;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Java.Lang;
using Universal.Data;

namespace AndroidPlatform {
    public class AndroidDatabaseProvider : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, Universal.Data.IDatabaseProvider {
        private const int REQUEST_CODE_RESOLVE = 9001;
        private const int REQUEST_CODE_LEADERBOARD = 9002;
        private const string SETTINGS_NAME = "googleplay";
        private const string SETTINGS_STRING_ACCOUNTNAME = "id";
        private const string LOCAL_COPY_STATE_NAME = "localstate";

        private Activity activity;

        public event Action OnConnectedToGoogle;

        private bool currentlyResolvingConnection = false;
        private GoogleApiClient _GoogleApiClient;
        public GoogleApiClient GoogleApiClient { get { return _GoogleApiClient; } }

        private ISharedPreferences localStateDatabase;

        public AndroidDatabaseProvider (Activity activity) {
            this.activity = activity;

            GoogleApiAvailability googleApiAvailability = GoogleApiAvailability.Instance;
            if (googleApiAvailability.IsGooglePlayServicesAvailable(activity) == ConnectionResult.Success) {
                string accountName;
                using (ISharedPreferences settings = activity.GetSharedPreferences(SETTINGS_NAME, FileCreationMode.Private)) {
                    accountName = settings.GetString(SETTINGS_STRING_ACCOUNTNAME, string.Empty);
                }

                global::Android.Widget.Toast.MakeText(activity, "awdawd", Android.Widget.ToastLength.Short);

                GoogleApiClient.Builder builder = new GoogleApiClient.Builder(activity, this, this)
                    .AddApi(GamesClass.API)
                    .AddScope(GamesClass.ScopeGames)
                    .AddApi(DriveClass.API)
                    .AddScope(DriveClass.ScopeAppfolder);

                if (!string.IsNullOrEmpty(accountName)) builder.SetAccountName(accountName);
                _GoogleApiClient = builder.Build( );

                _GoogleApiClient.Connect( );
            }

            localStateDatabase = activity.GetSharedPreferences(LOCAL_COPY_STATE_NAME, FileCreationMode.Private);
        }

        public void OnConnected (Bundle connectionHint) {
            using (ISharedPreferences settings = activity.GetSharedPreferences(SETTINGS_NAME, FileCreationMode.Private)) {
                using (ISharedPreferencesEditor editor = settings.Edit( )) {
                    editor.PutString(SETTINGS_STRING_ACCOUNTNAME, GamesClass.GetCurrentAccountName(_GoogleApiClient));
                    editor.Commit( );
                }
            }

            OnConnectedToGoogle?.Invoke( );
        }

        public void OnConnectionFailed (ConnectionResult result) {
            if (currentlyResolvingConnection) return;

            if (result.HasResolution) {
                currentlyResolvingConnection = true;
                result.StartResolutionForResult(activity, REQUEST_CODE_RESOLVE);
            }
        }

        public void OnConnectionSuspended (int cause) {
            _GoogleApiClient.Connect( );
        }

        public void OnActivityResult (int requestCode, Result resultCode, Intent data) {
            if (requestCode == REQUEST_CODE_RESOLVE) {
                if (resultCode == Result.Ok) {
                    _GoogleApiClient.Connect( );
                }
                currentlyResolvingConnection = false;
            }
        }

        public void Write (string key, string value) {
            using (ISharedPreferencesEditor editor = localStateDatabase.Edit( )) {
                editor.PutString(key, value);
                editor.Commit( );
            }

        }

        public void Write (string key, long value) {
            using (ISharedPreferencesEditor editor = localStateDatabase.Edit( )) {
                editor.PutLong(key, value);
                editor.Commit( );
            }
        }

        public string ReadString (string key, string defaultValue = null) {
            return localStateDatabase.GetString(key, defaultValue);
        }

        public long ReadLong (string key, long defaultValue = 0) {
            return localStateDatabase.GetLong(key, defaultValue);
        }

        public new void Dispose ( ) {
            base.Dispose( );
            localStateDatabase.Dispose( );
        }
    }
}
