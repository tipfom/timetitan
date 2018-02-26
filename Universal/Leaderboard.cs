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

namespace Universal {
    public class AndroidLeaderboard : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener {
        private const int REQUEST_CODE_RESOLVE = 9001;

        private GoogleApiClient googleApiClient;
        private bool currentlyResolvingConnection = false;

        public AndroidLeaderboard ( ) {
            GoogleApiClient.Builder builder = new GoogleApiClient.Builder(Assets.Context, this, this);
            builder.AddApi(GamesClass.API);
            builder.AddScope(GamesClass.ScopeGames);
            // für anmeldung beim nächsten starten: builder.SetAccountName( );
            googleApiClient = builder.Build( );
            googleApiClient.Connect( );
        }

        public void OnConnected (Bundle connectionHint) {
            //GamesClass.Requests.RegisterRequestListener(googleApiClient, Assets.Context);
            global::Android.Widget.Toast.MakeText(Assets.Context, "wuhu", Android.Widget.ToastLength.Short).Show( );
        }

        public void OnConnectionFailed (ConnectionResult result) {
            if (currentlyResolvingConnection) return;

            if (result.HasResolution) {
                currentlyResolvingConnection = true;
                result.StartResolutionForResult((Activity)Assets.Context, REQUEST_CODE_RESOLVE);
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

        public void SubmitToLeaderboards (int score) {
            GamesClass.Leaderboards.SubmitScore(googleApiClient, "CgkI3dz0sMcMEAIQAQ", score);
        }

        public void ShowLeaderboards ( ) {
            var intent = GamesClass.Leaderboards.GetLeaderboardIntent(googleApiClient, "CgkI3dz0sMcMEAIQAQ");
            ((Activity)Assets.Context).StartActivityForResult(intent, 9002);
        }
    }
}
