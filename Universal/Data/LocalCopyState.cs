using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Gms.Games.Snapshot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace Universal.Data {
    public class LocalCopyState : IDisposable {
        public const int VERSION = 1;
        public const int SECONDS_BETWEEN_CLOUDUPDATES = 100;
        public const string DATABASE_GOLD_KEY = "gold";
        public const string DATABASE_STAGE_KEY = "stage";
        public const string DATABASE_LAST_SYNCED_TIMESTAMP = "lstimestamp";

        public long LastSyncedTimestamp { get; private set; }

        public event Action<long> GoldChanged;
        private long _Gold;
        public long Gold {
            get { return _Gold; }
            set {
                if (_Gold != value) {
                    _Gold = value;
                    DatabaseProvider.Write(DATABASE_GOLD_KEY, value);
                    IsCloudUpdateRequired = true;
                    GoldChanged?.Invoke(value);
                }
            }
        }

        public event Action<long> StageChanged;
        private long _Stage;
        public long Stage {
            get { return _Stage; }
            set {
                if (_Stage != value) {
                    _Stage = value;
                    DatabaseProvider.Write(DATABASE_STAGE_KEY, _Stage);
                    IsCloudUpdateRequired = true;
                    StageChanged?.Invoke(value);
                }
            }
        }

        public readonly IDatabaseProvider DatabaseProvider;
        private Timer updateCloudCopyTimer;

        public bool IsCloudUpdateRequired = false;
        private bool Syncing = false;

        public LocalCopyState (IDatabaseProvider databaseprovider) {
            DatabaseProvider = databaseprovider;
            DatabaseProvider.OnConnectedToGoogle += OnConnectedToGoogle;

            _Gold = DatabaseProvider.ReadLong(DATABASE_GOLD_KEY);
            _Stage = DatabaseProvider.ReadLong(DATABASE_STAGE_KEY);
            LastSyncedTimestamp = DatabaseProvider.ReadLong(DATABASE_LAST_SYNCED_TIMESTAMP);

            updateCloudCopyTimer = new Timer(new TimerCallback((o) => UpdateCloudCopy( )), null, Timeout.Infinite, Timeout.Infinite);
        }

        private void OnConnectedToGoogle ( ) {
            Sync( );
        }

        private async void Sync ( ) {
            ISnapshot snapshot = await CloudCopyState.GetSnapshot(DatabaseProvider.GoogleApiClient);
            CloudCopyState cloudCopyState = new CloudCopyState(snapshot);
            if (cloudCopyState.Timestamp < LastSyncedTimestamp || cloudCopyState.Stage < Stage) {
                IsCloudUpdateRequired = true;
                UpdateCloudCopy( );
            } else {
                Gold = cloudCopyState.Gold;
                Stage = cloudCopyState.Stage;
                LastSyncedTimestamp = cloudCopyState.Timestamp;
            }

            DatabaseProvider.Write(DATABASE_GOLD_KEY, Gold);
            DatabaseProvider.Write(DATABASE_LAST_SYNCED_TIMESTAMP, LastSyncedTimestamp);

            updateCloudCopyTimer.Change(0, SECONDS_BETWEEN_CLOUDUPDATES * 1000);
        }

        private async void UpdateCloudCopy ( ) {
            if (IsCloudUpdateRequired && !Syncing) {
                Syncing = true;
                IsCloudUpdateRequired = false;
#if __ANDROID__
                LastSyncedTimestamp = await CloudCopyState.Update(Gold, Stage, DatabaseProvider.GoogleApiClient);
                DatabaseProvider.Write(DATABASE_LAST_SYNCED_TIMESTAMP, LastSyncedTimestamp);
#endif
                Syncing = false;
            }
        }

        public void Dispose ( ) {
            DatabaseProvider.Dispose( );
        }
    }
}
