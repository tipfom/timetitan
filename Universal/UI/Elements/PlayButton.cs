using System;
using System.Collections.Generic;
using System.Text;
using Universal.UI.Layout;

namespace Universal.UI.Elements {
    public class PlayButton : Element {
        bool pressed = false;

        public PlayButton (Screen owner, Container container, int depth) : base(owner, container, depth, false) {
        }

        public override bool HandleTouch (Touch.Action action, Touch touch) {
            if(action != Touch.Action.Move) {
                pressed = action == Touch.Action.Begin || action == Touch.Action.Enter;
                IsDirty = true;
            }

            return base.HandleTouch(action, touch);
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            yield return new RenderableElement(Container.Box.Verticies, "button_play" + (pressed ? "_pressed" : "" ), Depth);
        }
    }
}
