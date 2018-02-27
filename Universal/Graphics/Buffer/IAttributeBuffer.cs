using System;
using System.Collections.Generic;
using System.Text;
using Universal.Graphics.Handle;

namespace Universal.Graphics.Buffer {
    public interface IAttributeBuffer : IBuffer {
        int Dimensions { get; }
        int Stride { get; set; }

        void Bind (AttributeHandle attribute);
        void Bind (int location);
    }
}
