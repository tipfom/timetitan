using Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.UI.Animations {
    public abstract class Animation {
        public abstract bool Update (DeltaTime dt);
    }
}
