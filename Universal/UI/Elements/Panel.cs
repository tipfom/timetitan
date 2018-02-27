using System;
using System.Collections.Generic;
using System.Text;
using Universal.UI.Layout;

namespace Universal.UI.Elements {
    public class Panel : Element {
        public Panel (Screen owner, Container layout, bool multiclick = false) : base(owner, layout, 0, multiclick) {
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            yield break;
        }
    }
}
