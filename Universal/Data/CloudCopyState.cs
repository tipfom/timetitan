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
        public const int VERSION = 2;
        public const int BUFFER_LENGTH = 1 * sizeof(int) + 2 * sizeof(long);
        public const string SNAPSHOT_NAME = "beta_snapshot";

        public readonly string ID;
        public readonly long Timestamp;
        public readonly long Gold = -1;
        public readonly long Stage = 0;

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
                    }
                }
            }
        }

        public static byte[ ] GetData (long Gold, long Stage) {
            byte[ ] data = new byte[BUFFER_LENGTH];
            using (MemoryStream memoryStream = new MemoryStream(data)) {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream)) {
                    binaryWriter.Write(VERSION);
                    binaryWriter.Write(Gold);
                    binaryWriter.Write(Stage);
                }
            }
            return data;
        }

        public async static Task<long> Update (long Gold, long Stage, GoogleApiClient googleApiClient) {
            ISnapshot snapshot = await GetSnapshot(googleApiClient);

            snapshot.SnapshotContents.WriteBytes(CloudCopyState.GetData(Gold, Stage));

            SnapshotMetadataChangeBuilder metadataChangeBuilder = new SnapshotMetadataChangeBuilder( );
            metadataChangeBuilder.SetCoverImage(Assets.Icon);
            metadataChangeBuilder.SetDescription("Time Titan, " + Gold + " Gold");
            metadataChangeBuilder.SetProgressValue(Gold);
            ISnapshotMetadataChange metadataChange = metadataChangeBuilder.Build( );

            ISnapshotsCommitSnapshotResult commitSnapshotResult = await GamesClass.Snapshots.CommitAndCloseAsync(googleApiClient, snapshot, metadataChange);

            return commitSnapshotResult.SnapshotMetadata.LastModifiedTimestamp;
        }

        public async static Task<ISnapshot> GetSnapshot (GoogleApiClient googleApiClient, string name = SNAPSHOT_NAME) {
            Task<ISnapshotsOpenSnapshotResult> openSnapshotTask = GamesClass.Snapshots.OpenAsync(googleApiClient, name, true, Snapshots.ResolutionPolicyHighestProgress);
            ISnapshotsOpenSnapshotResult openShapshotResult = await openSnapshotTask;
            if (openShapshotResult.Status.StatusCode == GamesStatusCodes.StatusOk) {
                return openShapshotResult.Snapshot;
            }
            return null;
        }
    }
}
