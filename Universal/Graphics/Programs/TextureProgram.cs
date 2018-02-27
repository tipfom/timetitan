﻿using System;
using System.Collections.Generic;
using System.Text;
using Universal.Graphics.Buffer;
using Universal.Graphics.Handle;

namespace Universal.Graphics.Programs
{
    public abstract class TextureProgram : Program {
        private TextureHandle textureHandle;
        private AttributeHandle textureCoordsHandle;

        public TextureProgram (string vertexShader, string fragmentShader) : base(vertexShader, fragmentShader) {
            textureCoordsHandle = new AttributeHandle(glProgram, "a_texcoord");
            textureHandle = new TextureHandle(glProgram);
        }

        public override void Begin ( ) {
            base.Begin( );
            textureCoordsHandle.Enable( );
        }

        public override void End ( ) {
            textureCoordsHandle.Disable( );
            base.End( );
        }

        protected void Apply(int texture, IndexBuffer indexbuffer, IAttributeBuffer vertexbuffer, IAttributeBuffer texturebuffer, bool alphaBlending) {
            Apply(vertexbuffer, alphaBlending);
            texturebuffer.Bind(textureCoordsHandle);
            textureHandle.Set(texture);
            indexbuffer.Bind( );
        }
    }
}
