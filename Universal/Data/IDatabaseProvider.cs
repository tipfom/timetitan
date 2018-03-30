using Android.Gms.Common.Apis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Data {
    public interface IDatabaseProvider : IDisposable {
        void Write (string key, string value);
        void Write (string key, long value);
        string ReadString (string key, string defaultValue = null);
        long ReadLong (string key, long defaultValue = 0);

        GoogleApiClient GoogleApiClient { get; }
        event Action OnConnectedToGoogle;
    }
}
