using System;
using System.Collections.Generic;
using System.Text;

namespace Universal {
    public interface ILeaderboard {
        void SubmitToLeaderboard (int score);
        void ShowLeaderboard ( );
    }
}
