using System;
using System.Collections.Generic;
using System.Text;
using Universal.UI.Layout;

namespace Universal.UI.Elements {
    public class HeartViewer : Element {
        private int _Active;
        public int Active { get { return _Active; } set { _Active = value; IsDirty = true; } }

        public int Count { get; }

        public HeartViewer (Screen owner, Container container, int depth, int count) : base(owner, container, depth) {
            Active = count;
            Count = count;
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            for (int i = 0; i < Count; i++) {
                if (i < Active) {
                    yield return new RenderableElement(Box.GetVerticies(Container.X + 1.1f * Container.Height * i, Container.Y, Container.Height, Container.Height), "heart_full", Depth);
                } else {
                    yield return new RenderableElement(Box.GetVerticies(Container.X + 1.1f * Container.Height * i, Container.Y, Container.Height, Container.Height), "heart_empty", Depth);
                }
            }
        }
    }
}
