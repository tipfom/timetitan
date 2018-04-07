using System;

namespace Core {
    public struct DeltaTime {
        public static DeltaTime operator * (DeltaTime dt, float scale) {
            return new DeltaTime(dt.Milliseconds * scale);
        }

        public float Seconds;
        public float Milliseconds;

        public DeltaTime (float seconds, float milliseconds) {
            Seconds = seconds + milliseconds / 1000f;
            Milliseconds = seconds * 1000f + milliseconds;
        }

        public DeltaTime (float milliseconds) {
            Milliseconds = milliseconds;
            Seconds = milliseconds / 1000f;
        }

        public DeltaTime (TimeSpan span) {
            Milliseconds = (float)span.TotalMilliseconds;
            Seconds = (float)span.TotalSeconds;
        }
    }
}
