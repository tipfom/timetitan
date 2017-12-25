﻿using OpenTK.Graphics.ES20;
using OpenTK;

namespace Universal.Graphics.Handle {
    public class UniformMatrixHandle {
        public readonly int Location;

        public UniformMatrixHandle (int program, string name) {
            Location = GL.GetUniformLocation(program, name);
        }

        public void Set (Matrix4 matrix) {
            GL.UniformMatrix4(Location,1, false, ref matrix.Row0.X);
        }
    }
}
