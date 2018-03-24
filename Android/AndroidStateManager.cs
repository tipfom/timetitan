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
    public class AndroidStateManager : Java.Lang.Object, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, Universal.Data.IStateManager, Universal.Data.LocalCopyState.IDatabaseProvider {
        private const int REQUEST_CODE_RESOLVE = 9001;
        private const int REQUEST_CODE_LEADERBOARD = 9002;
        private const string SETTINGS_NAME = "googleplay";
        private const string SETTINGS_STRING_ACCOUNTNAME = "id";
        private const string LEADERBOARD_ID = "CgkI3dz0sMcMEAIQAQ";
        private const string LOCAL_LEADERBOARD_NAME = "localleaderboard";
        private const string LOCAL_LEADERBOARD_HIGHSCORE = "highscore";

        private const string LOCAL_COPY_STATE_NAME = "localstate";

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

        private LocalCopyState _State;
        public LocalCopyState State { get { return _State; } }

        private ISharedPreferences localStateDatabase;

        public AndroidStateManager (Activity activity) {
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
                googleApiClient = builder.Build( );

                using (ISharedPreferences settings = activity.GetSharedPreferences(LOCAL_LEADERBOARD_NAME, FileCreationMode.Private)) {
                    Highscore = settings.GetInt(LOCAL_LEADERBOARD_HIGHSCORE, 0);
                }

                googleApiClient.Connect( );
            }

            localStateDatabase = activity.GetSharedPreferences(LOCAL_COPY_STATE_NAME, FileCreationMode.Private);
            _State = new LocalCopyState(this);
        }

        public void OnConnected (Bundle connectionHint) {
            using (ISharedPreferences settings = activity.GetSharedPreferences(SETTINGS_NAME, FileCreationMode.Private)) {
                using (ISharedPreferencesEditor editor = settings.Edit( )) {
                    editor.PutString(SETTINGS_STRING_ACCOUNTNAME, GamesClass.GetCurrentAccountName(googleApiClient));
                    editor.Commit( );
                }
            }

            new Thread(async ( ) => {
                State.Sync(googleApiClient);
            }, "InitGameStateThread").Start( );

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
            if (requestCode == REQUEST_CODE_RESOLVE) {
                if (resultCode == Result.Ok) {
                    googleApiClient.Connect( );
                }
                currentlyResolvingConnection = false;
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
