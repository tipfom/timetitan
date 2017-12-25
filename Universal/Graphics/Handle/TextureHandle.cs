﻿using OpenTK.Graphics.ES20;

namespace Universal.Graphics.Handle {
    public class TextureHandle {
        public readonly int Location;

        public TextureHandle (int program) : this(program, "u_texture") {

        }

        public TextureHandle (int program, string name) {
            Location = GL.GetUniformLocation(program, name);
        }

        public void Set (int texture) {
            Set(texture, 0);
        }

        public void Set (int texture, int unit) {
            GL.ActiveTexture((TextureUnit)(TextureUnit.Texture0 + unit));
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.Uniform1(Location, unit);
        }
    }
}
