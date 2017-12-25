using System;
using System.Collections.Generic;
using Core;
using Core.Graphics;
using Universal.Graphics.Buffer;
using Universal.UI;
using static Universal.Graphics.Programs.ColorProgram;

namespace Universal.Graphics.Renderer {
    public static class UIRenderer {
        public static Spritebatch2D Texture;
        private const int MAX_QUADS = 800;
        private static BufferBatch buffer;
        private static Screen currentScreen;
        private static List<Tuple<Element, int>>[ ] indexUsage;
        private static int renderCount;
        private static Dictionary<Screen, List<Element>> elements;
        private static Dictionary<Screen, int> elementsOffset;
        private static int vertexCount;
        private static Queue<Element> updateQueue;
        private static int[ ] startPositions;
        private static Queue<RenderableElement>[ ] updateBufferData;

        private static CachedGPUBuffer vertexBuffer;
        private static CachedGPUBuffer textureBuffer;
        private static CachedGPUBuffer colorBuffer;

        public static void Init ( ) {
            startPositions = new int[ ] { 0, 0, 0 };
            renderCount = 0;
            vertexCount = 0;
            updateQueue = new Queue<Element>( );

            IndexBuffer sharedIndexBuffer = new IndexBuffer(MAX_QUADS);
            vertexBuffer = new CachedGPUBuffer(2, MAX_QUADS, PrimitiveType.Quad);
            textureBuffer = new CachedGPUBuffer(2, MAX_QUADS, PrimitiveType.Quad);
            colorBuffer = new CachedGPUBuffer(4, MAX_QUADS, PrimitiveType.Quad);
            buffer = new BufferBatch(sharedIndexBuffer, vertexBuffer, colorBuffer, textureBuffer);

            elements = new Dictionary<Screen, List<Element>>( );
            elementsOffset = new Dictionary<Screen, int>( );
            indexUsage = new List<Tuple<Element, int>>[ ] { new List<Tuple<Element, int>>( ), new List<Tuple<Element, int>>( ), new List<Tuple<Element, int>>( ) };
            updateBufferData = new Queue<RenderableElement>[ ] { new Queue<RenderableElement>( ), new Queue<RenderableElement>( ), new Queue<RenderableElement>( ) };
        }

        public static void Dispose ( ) {
            buffer.Dispose( );
            elements.Clear( );
            indexUsage[0].Clear( );
            updateQueue.Clear( );
        }

        public static List<Element> Current { get { return elements[currentScreen]; } }

        public static void Add (Screen screen, Element element) {
            if (!elements.ContainsKey(screen)) {
                elements.Add(screen, new List<Element>( ));
                elementsOffset.Add(screen, 0);
            }
            switch (element.Depth) {
                case Depth.Foreground:
                    elements[screen].Insert(0, element);
                    elementsOffset[screen]++;
                    break;
                case Depth.Center:
                    elements[screen].Insert(elementsOffset[screen], element);
                    break;
                case Depth.Background:
                    elements[screen].Add(element);
                    break;
            }
        }

        public static void Remove (Element element) {
            if (!elements.ContainsKey(element.Screen)) return;
            elements[element.Screen].Remove(element);
            element.Visible = false;
            if (element.Depth == Depth.Foreground) elementsOffset[element.Screen]--;
            if (element.Screen.IsActive) Update(element);
        }

        public static void Delete ( ) {
            elements.Clear( );
        }

        public static void Delete (Screen target) {
            elements.Remove(target);
        }

        public static void Draw ( ) {
            Program.Begin( );
            Program.Draw(buffer, Texture, Matrix.Default, renderCount, 0, true);
            Program.End( );
        }

        public static void Prepare (Screen target) {
            currentScreen = target;
            vertexCount = 0;
            renderCount = 0;
            Array.Clear(startPositions, 0, 3);

            indexUsage[0].Clear( );
            indexUsage[1].Clear( );
            indexUsage[2].Clear( );
            updateQueue.Clear( );

            if (elements.ContainsKey(target)) {
                foreach (Element element in elements[target]) {
                    Update(element);
                }
            } else {
                elements.Add(target, new List<Element>( ));
                elementsOffset.Add(target, 0);
            }
        }

        public static void Update (Element element) {
            updateQueue.Enqueue(element);
        }

        private static void UpdateBuffer (Element element) {
            if (element.Visible) {
                updateBufferData[0].Clear( );
                updateBufferData[1].Clear( );
                updateBufferData[2].Clear( );

                foreach (RenderableElement vertexData in element.Draw( )) {
                    updateBufferData[vertexData.Depth].Enqueue(vertexData);
                }

                void updateBufferAt(int i) {
                    Queue<RenderableElement> queue = updateBufferData[i];
                    int verticies = queue.Count * 8;
                    int position, index = FindCurrentIndex(element, i, out position);
                    int delta = verticies - ((index == -1) ? 0 : indexUsage[i][index].Item2);

                    if (delta != 0) {
                        // create space
                        int oldend = position + ((index == -1) ? 0 : indexUsage[i][index].Item2), newend = position + verticies, length = Math.Max(oldend, newend);
                        Array.Copy(vertexBuffer.Cache, oldend, vertexBuffer.Cache, newend, vertexBuffer.Cache.Length - length);
                        Array.Copy(textureBuffer.Cache, oldend, textureBuffer.Cache, newend, textureBuffer.Cache.Length - length);
                        Array.Copy(colorBuffer.Cache, oldend * 2, colorBuffer.Cache, newend * 2, colorBuffer.Cache.Length - length * 2);
                    }

                    if (index != -1) indexUsage[i][index] = new Tuple<Element, int>(element, verticies);
                    else indexUsage[i].Add(new Tuple<Element, int>(element, verticies));

                    vertexCount += delta;
                    renderCount += delta * 6 / 8;
                    while (queue.Count > 0) {
                        RenderableElement vertexData = queue.Dequeue( );
                        Array.Copy(vertexData.Verticies, 0, vertexBuffer.Cache, position, 8);
                        Array.Copy(Texture[vertexData.Texture], 0, textureBuffer.Cache, position, 8);
                        Array.Copy(vertexData.Color.ToArray4( ), 0, colorBuffer.Cache, position * 2, 16);
                        position += 8;
                    }
                    for (int k = i; k < 3; k++) startPositions[k] += delta;
                }

                updateBufferAt(0);
                updateBufferAt(1);
                updateBufferAt(2);
            } else {
                int index;
                int position;
                void clearBufferAt(int i) {
                    index = FindCurrentIndex(element, i, out position);
                    if (index > -1) {
                        int verticiesToClear = indexUsage[i][index].Item2;
                        indexUsage[i].RemoveAt(index);
                        vertexCount -= verticiesToClear;
                        renderCount -= verticiesToClear * 6 / 8;
                        for (int k = i; k < 3; k++) startPositions[k] -= verticiesToClear;
                        int end = position + verticiesToClear;
                        Array.Copy(vertexBuffer.Cache, end, vertexBuffer.Cache, position, vertexBuffer.Cache.Length - end);
                        Array.Copy(textureBuffer.Cache, end, textureBuffer.Cache, position, textureBuffer.Cache.Length - end);
                        Array.Copy(colorBuffer.Cache, end * 2, colorBuffer.Cache, position * 2, colorBuffer.Cache.Length - end * 2);
                    }
                }
                clearBufferAt(0);
                clearBufferAt(1);
                clearBufferAt(2);
            }
        }

        private static int FindCurrentIndex (Element item, int depth, out int position) {
            position = (depth == 0) ? 0 : startPositions[depth - 1];
            for (int i = 0; i < indexUsage[depth].Count; i++) {
                Tuple<Element, int> entry = indexUsage[depth][i];
                if (entry.Item1 == item) {
                    return i;
                } else {
                    position += entry.Item2;
                }
            }
            return -1;
        }

        public static void Update (DeltaTime dt) {
            if (updateQueue.Count > 0) {
                while (updateQueue.Count > 0)
                    UpdateBuffer(updateQueue.Dequeue( ));
                ApplyBufferUpdates( );
            }

            foreach (Element element in elements[currentScreen])
                element.Update(dt);
        }

        private static void ApplyBufferUpdates ( ) {
            vertexBuffer.Apply( );
            textureBuffer.Apply( );
            colorBuffer.Apply( );
        }
    }
}