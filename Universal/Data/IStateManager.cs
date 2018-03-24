using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Data {
    public interface IStateManager : IDisposable {
        void SubmitToLeaderboard (int score);
        void ShowLeaderboard ( );
        int Highscore { get; }
        LocalCopyState State { get; }
        event Action<int> HighscoreChanged;
    }
}
