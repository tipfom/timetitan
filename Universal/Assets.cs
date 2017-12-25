﻿using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.ES20;
using Path = System.IO.Path;
using Newtonsoft.Json;
using Core.Graphics;
using Core;

#if __ANDROID__
using Android.Content;
using Android.Graphics;
using Android.Opengl;
using Android.Gestures;
#endif

namespace Universal {
    public static class Assets {
#if __ANDROID__
        public static Context Context { get; set; }
#endif

        public static void Destroy ( ) {
            foreach (Texture2D texture in textureCache.Values)
                texture.Dispose( );
            textureCache.Clear( );
            foreach (Spritebatch2D sprite in spriteCache.Values)
                sprite.Dispose( );
            spriteCache.Clear( );

            foreach (int shader in loadedVertexShader.Values)
                GL.DeleteShader(shader);
            loadedVertexShader.Clear( );
            foreach (int shader in loadedFragmentShader.Values)
                GL.DeleteShader(shader);
            loadedFragmentShader.Clear( );
        }

        public static Texture2D GetTexture (params string[ ] path) {
            return GetTexture(InterpolationMode.PixelPerfect, path);
        }

        static Dictionary<int, Texture2D> textureCache = new Dictionary<int, Texture2D>( );
        public static Texture2D GetTexture (InterpolationMode interpolation, params string[ ] path) {
            int pathhash = path.GetHashCode( );
            if (!textureCache.ContainsKey(pathhash) || textureCache[pathhash].Disposed) {
                int width, height, id = GL.GenTexture( );
                GL.BindTexture(TextureTarget.Texture2D, id);
#if __ANDROID__
                using (BitmapFactory.Options options = new BitmapFactory.Options( ) { InScaled = false })
                using (Stream stream = GetStream(path))
                using (Bitmap bitmap = BitmapFactory.DecodeStream(stream, null, options)) {
                    GLUtils.TexImage2D((int)TextureTarget.Texture2D, 0, bitmap, 0);
                    width = bitmap.Width;
                    height = bitmap.Height;
                }
#endif

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)interpolation);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)interpolation);

                if (!textureCache.ContainsKey(pathhash)) {
                    textureCache.Add(pathhash, new Texture2D(id, new Size(width, height), path[path.Length - 1]));
                } else {
                    textureCache[pathhash] = new Texture2D(id, new Size(width, height), path[path.Length - 1]);
                }
#if DEBUG
                if (id == 0 || Debug.CheckGL(typeof(Assets)))
                    Debug.Print(typeof(Assets), "failed loading image " + path[path.Length - 1]);
                else
                    Debug.Print(typeof(Assets), "loaded image " + path[path.Length - 1]);
#endif
            }
            return textureCache[pathhash];
        }

        static Dictionary<int, Spritebatch2D> spriteCache = new Dictionary<int, Spritebatch2D>( );
        public static Spritebatch2D GetSprite (string name) {
            int namehash = name.GetHashCode( );
            if (!spriteCache.ContainsKey(namehash)) {
                Texture2D texture = GetTexture("textures", name + ".png");
                Dictionary<string, int[ ]> spriteContent = new Dictionary<string, int[ ]>( );
                JsonConvert.PopulateObject(GetText("textures", name + ".json"), spriteContent);
                spriteCache.Add(namehash, new Spritebatch2D(spriteContent, texture));
            }
            return spriteCache[namehash];
        }

        private static Dictionary<string, int> loadedVertexShader = new Dictionary<string, int>( );
        public static int GetVertexShader (string name) {
            if (loadedVertexShader.ContainsKey(name))
                return loadedVertexShader[name];
            int shader = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(shader, GetText("shader", "vertex", name));
            GL.CompileShader(shader);

#if DEBUG
            string log = GL.GetShaderInfoLog(shader);
            Debug.Print(typeof(Assets), $"vertexshader {shader} loaded from {name}");
            if (!string.IsNullOrWhiteSpace(log))
                Debug.Print(typeof(Assets), "log: " + log);
            Debug.CheckGL(typeof(Assets));
#endif

            return shader;
        }

        private static Dictionary<string, int> loadedFragmentShader = new Dictionary<string, int>( );
        public static int GetFragmentShader (string name) {
            if (loadedFragmentShader.ContainsKey(name))
                return loadedFragmentShader[name];
            int shader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(shader, GetText("shader", "fragment", name));
            GL.CompileShader(shader);

#if DEBUG
            string log = GL.GetShaderInfoLog(shader);
            Debug.Print(typeof(Assets), $"fragmentshader {shader} loaded from {name}");
            if (!string.IsNullOrWhiteSpace(log))
                Debug.Print(typeof(Assets), "log: " + log);
            Debug.CheckGL(typeof(Assets));
#endif

            return shader;
        }

        public static string GetText (params string[ ] path) {
            using (StreamReader reader = new StreamReader(GetStream(path))) {
                return reader.ReadToEnd( );
            }
        }

        public static Stream GetStream (params string[ ] path) {
#if __ANDROID__
            return Context.Assets.Open(Path.Combine(path));
#endif
        }
    }
}
