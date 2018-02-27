using System;
using System.Collections.Generic;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Gms.Games.LeaderBoard;
using Android.OS;
using Android.Views;

namespace Android {
    public class AndroidLeaderboard : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, Universal.ILeaderboard {
        private const int REQUEST_CODE_RESOLVE = 9001;
        private const int REQUEST_CODE_LEADERBOARD = 9002;
        private const string SETTINGS_NAME = "googleplay";
        private const string SETTINGS_STRING_ACCOUNTNAME = "id";

        private Activity activity;

        private GoogleApiClient googleApiClient;
        private bool currentlyResolvingConnection = false;

        public AndroidLeaderboard (Activity activity) {
            this.activity = activity;

            ISharedPreferences settings = activity.GetSharedPreferences(SETTINGS_NAME, FileCreationMode.Private);
            string accountName = settings.GetString(SETTINGS_STRING_ACCOUNTNAME, string.Empty);

            GoogleApiClient.Builder builder = new GoogleApiClient.Builder(activity, this, this);
            builder.AddApi(GamesClass.API);
            builder.AddScope(GamesClass.ScopeGames);
            if (!string.IsNullOrEmpty(accountName)) builder.SetAccountName(accountName);
            googleApiClient = builder.Build( );
            googleApiClient.Connect( );
        }

        public void OnConnected (Bundle connectionHint) {
            using (ISharedPreferences settings = activity.GetSharedPreferences(SETTINGS_NAME, FileCreationMode.Private)) {
                using (ISharedPreferencesEditor editor = settings.Edit( )) {
                    editor.PutString(SETTINGS_STRING_ACCOUNTNAME, GamesClass.GetCurrentAccountName(googleApiClient));
                    editor.Commit( );
                }
            }
        }

        public void OnConnectionFailed (ConnectionResult result) {
            if (currentlyResolvingConnection) return;

            if (result.HasResolution) {
                currentlyResolvingConnection = true;
                result.StartResolutionForResult(activity, REQUEST_CODE_RESOLVE);
            }
        }

        public void OnConnectionSuspended (int cause) {
            googleApiClient.Connect( );
        }

        public void OnActivityResult (int requestCode, Result resultCode, Intent data) {
            if (requestCode == REQUEST_CODE_RESOLVE && resultCode == Result.Ok) {
                googleApiClient.Connect( );
            }
        }

        public void SubmitToLeaderboard (int score) {
            if (googleApiClient != null && googleApiClient.IsConnected) {
                GamesClass.Leaderboards.SubmitScore(googleApiClient, "CgkI3dz0sMcMEAIQAQ", score);
            } else {
                global::Android.Widget.Toast.MakeText(activity, "leaderboard failed :(. im sorry might fix it later", global::Android.Widget.ToastLength.Short).Show( );
            }
        }

        public void ShowLeaderboard ( ) {
            if (googleApiClient != null && googleApiClient.IsConnected) {
                var intent = GamesClass.Leaderboards.GetLeaderboardIntent(googleApiClient, "CgkI3dz0sMcMEAIQAQ");
                activity.StartActivityForResult(intent, REQUEST_CODE_LEADERBOARD);
            } else {
                global::Android.Widget.Toast.MakeText(activity, "leaderboard failed :(. im sorry might fix it later", global::Android.Widget.ToastLength.Short).Show( );
            }
        }
    }
}
