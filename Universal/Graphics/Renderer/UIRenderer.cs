using Core.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.Graphics.Buffer;
using Universal.UI;
using static Universal.Graphics.Programs.ColorProgram;

namespace Universal.Graphics.Renderer {
    public class UIRenderer : IDisposable {
        public const int MAX_QUADS_PER_BUFFER = 40000;
        public const int MIN_DEPTH = -100;
        public const int MAX_DEPTH = 100;

        public static UIRenderer SharedRenderer;

        public Spritebatch2D Texture;

        private BufferBatch preBuffer, postBuffer;
        private CachedGPUBuffer preVertexBuffer, postVertexBuffer;
        private CachedGPUBuffer preTextureBuffer, postTextureBuffer;
        private CachedGPUBuffer preColorBuffer, postColorBuffer;
        private int preBufferLength, postBufferLength;
        private int preRenderCount, postRenderCount;
        private bool preBufferChanged, postBufferChanged;
        private Dictionary<Element, BufferEntry> preBufferEntries, postBufferEntries;

        private Queue<Element> updateQueue;

        private Matrix matrix;

        public UIRenderer (Spritebatch2D texture) {
            Texture = texture;

            Window.Changed += ( ) => {
                matrix = new Matrix(Window.ProjectionSize, MIN_DEPTH, MAX_DEPTH);
            };
            matrix = new Matrix(Window.ProjectionSize, MIN_DEPTH, MAX_DEPTH);

            preVertexBuffer = new CachedGPUBuffer(3, MAX_QUADS_PER_BUFFER, PrimitiveType.Quad);
            preTextureBuffer = new CachedGPUBuffer(2, MAX_QUADS_PER_BUFFER, PrimitiveType.Quad);
            preColorBuffer = new CachedGPUBuffer(4, MAX_QUADS_PER_BUFFER, PrimitiveType.Quad);

            postVertexBuffer = new CachedGPUBuffer(3, MAX_QUADS_PER_BUFFER, PrimitiveType.Quad);
            postTextureBuffer = new CachedGPUBuffer(2, MAX_QUADS_PER_BUFFER, PrimitiveType.Quad);
            postColorBuffer = new CachedGPUBuffer(4, MAX_QUADS_PER_BUFFER, PrimitiveType.Quad);

            IndexBuffer sharedIndexBuffer = new IndexBuffer(MAX_QUADS_PER_BUFFER);
            preBuffer = new BufferBatch(sharedIndexBuffer, preVertexBuffer, preColorBuffer, preTextureBuffer);
            postBuffer = new BufferBatch(sharedIndexBuffer, postVertexBuffer, postColorBuffer, postTextureBuffer);

            preBufferLength = postBufferLength = 0;
            preRenderCount = postRenderCount = 0;
            preBufferChanged = postBufferChanged = false;
            preBufferEntries = new Dictionary<Element, BufferEntry>( );
            postBufferEntries = new Dictionary<Element, BufferEntry>( );

            updateQueue = new Queue<Element>( );
        }

        public void Dispose ( ) {
            preBuffer.Dispose( );
            postBuffer.Dispose( );
        }

        public void Add(Element element) {
            preBufferEntries.Add(element, new BufferEntry());
            postBufferEntries.Add(element, new BufferEntry());
            RequestUpdate(element);
        }

        public void RequestUpdate (Element element) {
            updateQueue.Enqueue(element);
        }

        private void Update (Element element) {
            BufferEntry preBufferEntry = preBufferEntries[element];
            if (preBufferEntry.Length != 0) {
                int end = preBufferEntry.Start+ preBufferEntry.Length;
                Array.Copy(preVertexBuffer.Cache, 3 * end, preVertexBuffer.Cache, 3 * preBufferEntry.Start, preVertexBuffer.Cache.Length - 3 * end);
                Array.Copy(preTextureBuffer.Cache, 2 * end, preTextureBuffer.Cache, 2 * preBufferEntry.Start, preTextureBuffer.Cache.Length - 2 * end);
                Array.Copy(preColorBuffer.Cache, 4 * end, preColorBuffer.Cache, 4 * preBufferEntry.Start, preColorBuffer.Cache.Length - 4 * end);
                preBufferChanged = true;

                foreach (Element elementInPreBuffer in preBufferEntries.Keys) {
                    BufferEntry entry = preBufferEntries[elementInPreBuffer];
                    if (entry.Start > preBufferEntry.Start) {
                        preBufferEntries[elementInPreBuffer].Start = entry.Start - preBufferEntry.Length;
                    }
                }
            }
            preBufferLength -= preBufferEntry.Length;

            BufferEntry postBufferEntry = postBufferEntries[element];
            if (postBufferEntry.Length != 0) {
                int end = postBufferEntry.Start+ postBufferEntry.Length;
                Array.Copy(postVertexBuffer.Cache, 3 * end, postVertexBuffer.Cache, 3 * postBufferEntry.Start, postVertexBuffer.Cache.Length - 3 * end);
                Array.Copy(postTextureBuffer.Cache, 2 * end, postTextureBuffer.Cache, 2 * postBufferEntry.Start, postTextureBuffer.Cache.Length - 2 * end);
                Array.Copy(postColorBuffer.Cache, 4 * end, postColorBuffer.Cache, 4 * postBufferEntry.Start, postColorBuffer.Cache.Length - 4 * end);
                postBufferChanged = true;

                foreach (Element elementInPostBuffer in postBufferEntries.Keys) {
                    BufferEntry entry = postBufferEntries[elementInPostBuffer];
                    if (entry.Start > postBufferEntry.Start) {
                        postBufferEntries[elementInPostBuffer].Start = entry.Start - postBufferEntry.Length;
                    }
                }
            }
            postBufferLength -= postBufferEntry.Length;

            preBufferEntry.Start = preBufferLength;
            preBufferEntry.Length = 0;
            postBufferEntry.Start = postBufferLength;
            postBufferEntry.Length = 0;

            if (element.Visible) {
                foreach (RenderableElement renderableElement in element.Draw( )) {
                    if (renderableElement.Depth < 0) {
                        // preBuffer
                        int index = preBufferEntry.Start + preBufferEntry.Length;

                        for (int offset = 0; offset < 4; offset++) {
                            int indexInVertexBuffer = 3 * (index + offset);
                            int indexInRenderableElement = 2 * offset;
                            preVertexBuffer[indexInVertexBuffer] = renderableElement.Verticies[indexInRenderableElement];
                            preVertexBuffer[indexInVertexBuffer + 1] = renderableElement.Verticies[indexInRenderableElement + 1];
                            preVertexBuffer[indexInVertexBuffer + 2] = renderableElement.Depth;
                        }
                        Array.Copy(Texture[renderableElement.Texture], 0, preTextureBuffer.Cache, 2 * index, 8);
                        Array.Copy(renderableElement.Color.ToArray4( ), 0, preColorBuffer.Cache, 4 * index, 16);

                        preBufferEntry.Length += 4;
                        preBufferChanged = true;
                    } else {
                        // postBuffer
                        int index = postBufferEntry.Start + postBufferEntry.Length;

                        for (int offset = 0; offset < 4; offset++) {
                            int indexInVertexBuffer = 3 * (index + offset);
                            int indexInRenderableElement = 2 * offset;
                            postVertexBuffer[indexInVertexBuffer] = renderableElement.Verticies[indexInRenderableElement];
                            postVertexBuffer[indexInVertexBuffer + 1] = renderableElement.Verticies[indexInRenderableElement + 1];
                            postVertexBuffer[indexInVertexBuffer + 2] = renderableElement.Depth;
                        }
                        Array.Copy(Texture[renderableElement.Texture], 0, postTextureBuffer.Cache, 2 * index, 8);
                        Array.Copy(renderableElement.Color.ToArray4( ), 0, postColorBuffer.Cache, 4 * index, 16);

                        postBufferEntry.Length += 4;
                        postBufferChanged = true;
                    }
                }
            }

            preBufferLength += preBufferEntry.Length;
            postBufferLength += postBufferEntry.Length;

            preBufferEntries[element] = preBufferEntry;
            postBufferEntries[element] = postBufferEntry;

            postRenderCount = postBufferLength * 3 / 2;
            preRenderCount = preBufferLength * 3 / 2;
        }

        public void Update ( ) {
            if (updateQueue.Count > 0) {
                while (updateQueue.Count > 0) {
                    Update(updateQueue.Dequeue( ));
                }
                if (preBufferChanged) {
                    preVertexBuffer.Apply( );
                    preTextureBuffer.Apply( );
                    preColorBuffer.Apply( );
                }
                if (postBufferChanged) {
                    postVertexBuffer.Apply( );
                    postTextureBuffer.Apply( );
                    postColorBuffer.Apply( );
                }
                preBufferChanged = postBufferChanged = false;
            }
        }

        public void Prepare (Screen screen) {
            preBufferLength = postBufferLength = 0;
            preRenderCount = postRenderCount = 0;

            preBufferEntries.Clear( );
            postBufferEntries.Clear( );
            updateQueue.Clear( );

            foreach (Element element in screen.Elements) {
                Add(element);
            }
        }

        public void PreDraw ( ) {
            Program.Begin( );
            Program.Draw(preBuffer, Texture, matrix, preRenderCount, 0, true);
            Program.End( );
        }

        public void PostDraw ( ) {
            Program.Begin( );
            Program.Draw(postBuffer, Texture, matrix, postRenderCount, 0, true);
            Program.End( );
        }

        public class BufferEntry {
            public int Start;
            public int Length;
        }
    }
}
