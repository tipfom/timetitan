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
using Java.Lang;

namespace Android {
    public class AndroidLeaderboard : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, Universal.ILeaderboard {
        private const int REQUEST_CODE_RESOLVE = 9001;
        private const int REQUEST_CODE_LEADERBOARD = 9002;
        private const string SETTINGS_NAME = "googleplay";
        private const string SETTINGS_STRING_ACCOUNTNAME = "id";
        private const string LEADERBOARD_ID = "CgkI3dz0sMcMEAIQAQ";
        private const string LOCAL_LEADERBOARD_NAME = "localleaderboard";
        private const string LOCAL_LEADERBOARD_HIGHSCORE = "highscore";

        private Activity activity;

        private GoogleApiClient googleApiClient;
        private bool currentlyResolvingConnection = false;

        public event Action<int> HighscoreChanged;

        private int _Highscore = -1;
        public int Highscore {
            get { return _Highscore; }
            private set {
                if (value > _Highscore) {
                    _Highscore = value;
                    using (ISharedPreferences settings = activity.GetSharedPreferences(LOCAL_LEADERBOARD_NAME, FileCreationMode.Private)) {
                        using (ISharedPreferencesEditor editor = settings.Edit( )) {
                            editor.PutInt(LOCAL_LEADERBOARD_HIGHSCORE, value);
                            editor.Commit( );
                        }
                    }
                    HighscoreChanged?.Invoke(value);
                }
            }
        }

        public AndroidLeaderboard (Activity activity) {
            this.activity = activity;

            GoogleApiAvailability googleApiAvailability = GoogleApiAvailability.Instance;
            if (googleApiAvailability.IsGooglePlayServicesAvailable(activity) == ConnectionResult.Success) {
                string accountName;
                using (ISharedPreferences settings = activity.GetSharedPreferences(SETTINGS_NAME, FileCreationMode.Private)) {
                    accountName = settings.GetString(SETTINGS_STRING_ACCOUNTNAME, string.Empty);
                }

                GoogleApiClient.Builder builder = new GoogleApiClient.Builder(activity, this, this);
                builder.AddApi(GamesClass.API);
                builder.AddScope(GamesClass.ScopeGames);
                if (!string.IsNullOrEmpty(accountName)) builder.SetAccountName(accountName);
                googleApiClient = builder.Build( );

                using (ISharedPreferences settings = activity.GetSharedPreferences(LOCAL_LEADERBOARD_NAME, FileCreationMode.Private)) {
                    Highscore = settings.GetInt(LOCAL_LEADERBOARD_HIGHSCORE, 0);
                }

                googleApiClient.Connect( );
            }
        }

        public void OnConnected (Bundle connectionHint) {
            using (ISharedPreferences settings = activity.GetSharedPreferences(SETTINGS_NAME, FileCreationMode.Private)) {
                using (ISharedPreferencesEditor editor = settings.Edit( )) {
                    editor.PutString(SETTINGS_STRING_ACCOUNTNAME, GamesClass.GetCurrentAccountName(googleApiClient));
                    editor.Commit( );
                }
            }

            GetHighscoreFromGoogle( );
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
            Highscore = score;
            if (googleApiClient != null && googleApiClient.IsConnected) {
                GamesClass.Leaderboards.SubmitScore(googleApiClient, "CgkI3dz0sMcMEAIQAQ", score);
            }
        }

        public void ShowLeaderboard ( ) {
            if (googleApiClient != null && googleApiClient.IsConnected) {
                var intent = GamesClass.Leaderboards.GetLeaderboardIntent(googleApiClient, "CgkI3dz0sMcMEAIQAQ");
                activity.StartActivityForResult(intent, REQUEST_CODE_LEADERBOARD);
            }
        }

        private void GetHighscoreFromGoogle ( ) {
            if (googleApiClient != null && googleApiClient.IsConnected) {
                GamesClass.Leaderboards.LoadCurrentPlayerLeaderboardScore(googleApiClient, LEADERBOARD_ID, LeaderboardVariant.TimeSpanAllTime, LeaderboardVariant.CollectionPublic).SetResultCallback<ILeaderboardsLoadPlayerScoreResult>(GetHighscoreCallback);
            }
        }

        private void GetHighscoreCallback (ILeaderboardsLoadPlayerScoreResult result) {
            if (result != null && result.Status != null && result.Status.StatusCode == GamesStatusCodes.StatusOk) {
                int loadedHighscore = (int)result.Score.RawScore;
                if (Highscore > loadedHighscore) {
                    SubmitToLeaderboard(Highscore);
                } else {
                    Highscore = loadedHighscore;
                }
            }
        }
    }
}
