using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Universal.UI.Animations;
using Universal.UI.Layout;

namespace Universal.UI.Elements {
    public class CoinLabel : Label {
        private ChangeNumericTextAnimation changeAnimation;

        public CoinLabel (Screen owner, Container layout, float size, int depth = 1, TextAlignment alignment = TextAlignment.Left) : base(owner, layout, size, Manager.State.Gold.ToString( ), Core.Color.Gold, depth, alignment) {
            Manager.State.GoldChanged += (newGold) => {
                changeAnimation = new ChangeNumericTextAnimation(this, (int)newGold, 0.3f);
            };
        }

        public override void Update (DeltaTime dt) {
            if (changeAnimation?.Update(dt) ?? false) {
                changeAnimation = null;
            }
            base.Update(dt);
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            foreach (RenderableElement element in base.Draw( ))
                yield return element;
            yield return new RenderableElement(Box.GetVerticies(Container.Location.X - Container.Height*1.1f, Container.Location.Y, Container.Height, Container.Height), "coin", Depth);
        }
    }
}
