using System;
using System.Collections.Generic;
using System.Text;
using Universal.World;

namespace Universal.UI.Elements.Challenges {
    public abstract class TapChallenge {
        public readonly ChallengeType Type;
        public bool IsDirty;

        public TapChallenge (ChallengeType type) {
            Type = type;
        }

        public abstract UpdateAction Update (Touch.Action action, Touch touch);
    }
}
