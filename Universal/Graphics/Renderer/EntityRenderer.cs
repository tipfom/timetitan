using Core;
using Core.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.Graphics.Buffer;
using Universal.World;
using static Universal.Graphics.Programs.ColorProgram;

namespace Universal.Graphics.Renderer {
    public static class EntityRenderer {
        public const int MAX_VERTEX_COUNT = 20;

        private static Spritebatch2D[ ] entityTextures = new Spritebatch2D[2];

        private static BufferBatch buffer;
        private static ClientBuffer vertexBuffer;
        private static ClientBuffer textureBuffer;
        private static ClientBuffer colorBuffer;

        private static float _VertexSize;
        public static float VertexSize { get { return _VertexSize; } set { _VertexSize = value; matrix = new Matrix(new Vector2(1f / value * Window.Ratio, 1f / value)); } }

        private static Matrix matrix;

        public static void Init ( ) {
            vertexBuffer = new ClientBuffer(2, MAX_VERTEX_COUNT, PrimitiveType.Quad);
            textureBuffer = new ClientBuffer(2, MAX_VERTEX_COUNT, PrimitiveType.Quad);
            colorBuffer = new ClientBuffer(4, MAX_VERTEX_COUNT, PrimitiveType.Quad);
            buffer = new BufferBatch(new IndexBuffer(MAX_VERTEX_COUNT), vertexBuffer, colorBuffer, textureBuffer);

            entityTextures[Entity.PLAYER] = Assets.GetSprite("player/default");
            entityTextures[Entity.PLUGGER] = Assets.GetSprite("plugger");
        }

        public static void Draw (Entity entity, Vector2 position) {
            // room to improve the matrix allocation
            matrix.ResetView( );
            matrix.TranslateView(position.X / _VertexSize, position.Y / _VertexSize, 0);
            matrix.CalculateMVP( );

            int index = 0;
            foreach (RenderableObject renderableObject in entity.Draw( )) {
                Array.Copy(renderableObject.Verticies, 0, vertexBuffer.Data, index, 8);
                Array.Copy(entityTextures[entity.Species][renderableObject.Texture], 0, textureBuffer.Data, index, 8);
                Array.Copy(renderableObject.Color.ToArray4( ), 0, colorBuffer.Data, index * 2, 16);
                index += 8;
            }

            Program.Begin( );
            Program.Draw(buffer, entityTextures[entity.Species], matrix, true);
            Program.End( );
        }
    }
}
