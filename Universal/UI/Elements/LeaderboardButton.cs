using System;
using System.Collections.Generic;
using System.Text;
using Universal.UI.Layout;

namespace Universal.UI.Elements {
    public class LeaderboardButton : Element {
        public LeaderboardButton (Screen owner, Container container, int depth) : base(owner, container, depth, false) {
            Release += LeaderboardButton_Release;
        }

        private void LeaderboardButton_Release ( ) {
            // Manager.StateManager.ShowLeaderboard( );
        }

        public override IEnumerable<RenderableElement> Draw ( ) {
            yield return new RenderableElement(Container.Box.Verticies, "button_leaderboard", Depth, Core.Color.Black);
        }
    }
}
