using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Gms.Games.Snapshot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Universal.Data {
    public class CloudCopyState {
        public const int VERSION = 3;
        public const int BUFFER_LENGTH = 1 * sizeof(int) + 2 * sizeof(long) + 1 * sizeof(float);
        public const string SNAPSHOT_NAME = "beta_snapshot";

        public readonly string ID;
        public readonly long Timestamp;
        public readonly long Gold = -1;
        public readonly long Stage = 0;
        public readonly float Damage = 2.3f;

        public CloudCopyState (ISnapshot snapshot) : this(snapshot.SnapshotContents.ReadFully( ), snapshot.Metadata.SnapshotId, snapshot.Metadata.LastModifiedTimestamp) {
        }

        public CloudCopyState (byte[ ] data, string id, long timestamp) {
            Timestamp = timestamp;
            ID = id;
            if (data.Length > 0) {
                using (MemoryStream memoryStream = new MemoryStream(data))
                using (BinaryReader binaryReader = new BinaryReader(memoryStream)) {
                    switch (binaryReader.ReadInt32( )) {
                        case 1: // Version 1
                            Gold = binaryReader.ReadInt64( );
                            break;
                        case 2:
                            Gold = binaryReader.ReadInt64( );
                            Stage = binaryReader.ReadInt64( );
                            break;
                        case 3:
                            Gold = binaryReader.ReadInt64( );
                            Stage = binaryReader.ReadInt64( );
                            Damage = binaryReader.ReadSingle( );
                            break;
                    }
                }
            }
        }

        public bool IsAhead (CloudCopyState conflictingState) {
            if (Damage > conflictingState.Damage) {
                return true;
            } else if (Stage > conflictingState.Stage) {
                return true;
            } else if (Stage < conflictingState.Stage) {
                return false;
            } else if (Gold > conflictingState.Gold) {
                return true;
            }
            return false;
        }

        public static byte[ ] GetData (long Gold, long Stage, float Damage) {
            byte[ ] data = new byte[BUFFER_LENGTH];
            using (MemoryStream memoryStream = new MemoryStream(data)) {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream)) {
                    binaryWriter.Write(VERSION);
                    binaryWriter.Write(Gold);
                    binaryWriter.Write(Stage);
                    binaryWriter.Write(Damage);
                }
            }
            return data;
        }

        public async static Task<long> Update (long Gold, long Stage, float Damage, GoogleApiClient googleApiClient) {
            ISnapshot snapshot = await GetSnapshot(googleApiClient);

            snapshot.SnapshotContents.WriteBytes(CloudCopyState.GetData(Gold, Stage, Damage));

            SnapshotMetadataChangeBuilder metadataChangeBuilder = new SnapshotMetadataChangeBuilder( );
            metadataChangeBuilder.SetCoverImage(Assets.Icon);
            metadataChangeBuilder.SetDescription($"Time Titan Stage {Stage}, {Gold} Gold, {Damage} Damage");
            metadataChangeBuilder.SetProgressValue(Stage);
            ISnapshotMetadataChange metadataChange = metadataChangeBuilder.Build( );

            ISnapshotsCommitSnapshotResult commitSnapshotResult = await GamesClass.Snapshots.CommitAndCloseAsync(googleApiClient, snapshot, metadataChange);

            return commitSnapshotResult.SnapshotMetadata.LastModifiedTimestamp;
        }

        public async static Task<ISnapshot> GetSnapshot (GoogleApiClient googleApiClient, string name = SNAPSHOT_NAME) {
            Task<ISnapshotsOpenSnapshotResult> openSnapshotTask = GamesClass.Snapshots.OpenAsync(googleApiClient, name, true, Snapshots.ResolutionPolicyManual);
            ISnapshotsOpenSnapshotResult openSnapshotResult = await openSnapshotTask;
            return await GetSnapshotFromResult(googleApiClient, openSnapshotResult);
        }

        private async static Task<ISnapshot> GetSnapshotFromResult (GoogleApiClient googleApiClient, ISnapshotsOpenSnapshotResult openSnapshotResult) {
            switch (openSnapshotResult.Status.StatusCode) {
                case GamesStatusCodes.StatusOk:
                    return openSnapshotResult.Snapshot;
                case GamesStatusCodes.StatusSnapshotConflict:
                    CloudCopyState state = new CloudCopyState(openSnapshotResult.Snapshot);
                    CloudCopyState conflict = new CloudCopyState(openSnapshotResult.ConflictingSnapshot);
                    ISnapshotsOpenSnapshotResult resolveSnapshotResult;
                    if (conflict.IsAhead(state)) {
                        resolveSnapshotResult = await GamesClass.Snapshots.ResolveConflictAsync(googleApiClient, openSnapshotResult.ConflictId, openSnapshotResult.ConflictingSnapshot);
                    } else {
                        resolveSnapshotResult = await GamesClass.Snapshots.ResolveConflictAsync(googleApiClient, openSnapshotResult.ConflictId, openSnapshotResult.Snapshot);
                    }
                    return await GetSnapshotFromResult(googleApiClient, resolveSnapshotResult);
            }
            return null;
        }
    }
}
