using System;
using System.Collections.Generic;
using System.Text;
using Universal.UI.Layout;

namespace Universal.UI.Elements
{
    public class Aligner : Element {
        public Aligner (Screen owner, Container container) : base(owner, container, 0, false) {
        }

        public override bool HandleTouch (Touch.Action action, Touch touch) {
            return false;
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            yield break;
        }
    }
}
