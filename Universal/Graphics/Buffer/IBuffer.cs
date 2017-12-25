using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Graphics.Buffer {
    public interface IBuffer : IDisposable {
        int Length { get; }
        int Bytes { get; }
    }
}
