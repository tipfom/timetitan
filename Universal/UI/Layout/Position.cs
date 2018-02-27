using System;

namespace Universal.UI.Layout {
    [Flags]
    public enum Position : byte {
        Center = 1 << 0,
        Left = 1 << 1,
        Right = 1 << 2,
        Top = 1 << 3,
        Bottom = 1 << 4,
    }
}
