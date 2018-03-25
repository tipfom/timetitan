﻿using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Gms.Games.Snapshot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace Universal.Data {
    public class LocalCopyState {
        public const int VERSION = 1;
        public const int SECONDS_BETWEEN_CLOUDUPDATES = 100;
        public const string DATABASE_GOLD_KEY = "gold";
        public const string DATABASE_LAST_SYNCED_TIMESTAMP = "lstimestamp";


        public long LastSyncedTimestamp { get; private set; }

        public event Action<long> GoldChanged;
        private long _Gold;
        public long Gold {
            get { return _Gold; }
            set {
                if(_Gold != value) {
                    _Gold = value;
                    databaseProvider.Write(DATABASE_GOLD_KEY, value);
                    IsCloudUpdateRequired = true;
                    GoldChanged?.Invoke(value);
                }
            }
        }

        private GoogleApiClient googleApiClient;
        private IDatabaseProvider databaseProvider;
        private Timer updateCloudCopyTimer;

        public bool IsCloudUpdateRequired = false;
        private bool Syncing = false;

        public LocalCopyState (IDatabaseProvider databaseprovider) {
            this.databaseProvider = databaseprovider;

            _Gold = databaseProvider.ReadLong(DATABASE_GOLD_KEY);
            LastSyncedTimestamp = databaseProvider.ReadLong(DATABASE_LAST_SYNCED_TIMESTAMP);

            updateCloudCopyTimer = new Timer(new TimerCallback((o) => UpdateCloudCopy( )), null, Timeout.Infinite, Timeout.Infinite);
        }

        public async void Sync (GoogleApiClient googleApiClient) {
            this.googleApiClient = googleApiClient;

            ISnapshot snapshot = await CloudCopyState.GetSnapshot(googleApiClient);
            CloudCopyState cloudCopyState = new CloudCopyState(snapshot);
            if (cloudCopyState.Timestamp < LastSyncedTimestamp || cloudCopyState.Gold < Gold) {
                IsCloudUpdateRequired = true;
                UpdateCloudCopy( );
            } else {
                Gold = cloudCopyState.Gold;
                LastSyncedTimestamp = cloudCopyState.Timestamp;
            }

            databaseProvider.Write(DATABASE_GOLD_KEY, Gold);
            databaseProvider.Write(DATABASE_LAST_SYNCED_TIMESTAMP, LastSyncedTimestamp);

            updateCloudCopyTimer.Change(0, SECONDS_BETWEEN_CLOUDUPDATES * 1000);
        }

        public async void UpdateCloudCopy ( ) {
            if (IsCloudUpdateRequired && !Syncing) {
                Syncing = true;
                IsCloudUpdateRequired = false;
#if __ANDROID__
                LastSyncedTimestamp = await CloudCopyState.Update(Gold, googleApiClient);
                databaseProvider.Write(DATABASE_LAST_SYNCED_TIMESTAMP, LastSyncedTimestamp);
#endif
                Syncing = false;
            }
        }

        public interface IDatabaseProvider {
            void Write (string key, string value);
            void Write (string key, long value);
            string ReadString (string key, string defaultValue = null);
            long ReadLong (string key, long defaultValue = 0);
        }
    }
}