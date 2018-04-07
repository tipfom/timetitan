using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Universal.Graphics.Animations {
    public class SpriteAnimation {
        public delegate void AnimationFinishedCallback (bool finished);

        public string Texture { get { return Animations[currentAnimation].Frame.Texture; } }

        private Animation[ ] Animations;
        private int idleAnimation = 0;

        private int currentAnimation;
        private int queuedAnimation;
        private AnimationFinishedCallback currentCallback;
        private AnimationFinishedCallback queuedCallback;

        public SpriteAnimation (string source) {
            using (StreamReader reader = new StreamReader(Assets.GetStream(source))) {
                int count = int.Parse(reader.ReadLine( ));
                Animations = new Animation[count];
                int index = 0;
                string line;
                while ((line = reader.ReadLine( )) != null) {
                    string[ ] meta = line.Split(':');
                    string[ ] variations = meta[1].Split('|');
                    Frame[ ][ ] frames = new Frame[variations.Length][ ];
                    for (int i = 0; i < variations.Length; i++) {
                        string[ ] content = variations[i].Split(';');
                        frames[i] = new Frame[content.Length];
                        for (int k = 0; k < content.Length; k++) {
                            string[ ] data = content[k].Split(',');
                            frames[i][k] = new Frame( ) { Texture = data[0], Time = int.Parse(data[1]) };
                        }

                    }
                    Animations[index] = new Animation( ) { Name = meta[0], Frames = frames };
                    index++;
                }
            }

            idleAnimation = GetIndex("idle");
            currentAnimation = idleAnimation;
        }

        public void Update (DeltaTime dt) {
            if (!Animations[currentAnimation].Update(dt)) {
                currentCallback?.Invoke(true);
                if (queuedAnimation != -1) {
                    currentAnimation = queuedAnimation;
                    currentCallback = queuedCallback;
                    queuedAnimation = -1;
                } else {
                    currentAnimation = idleAnimation;
                    currentCallback = null;
                }
                Animations[currentAnimation].Start( );
            }
        }

        public void Start (string name, bool force = true, AnimationFinishedCallback animationFinishedCallback = null) {
            if (force) {
                currentAnimation = GetIndex(name);
                currentCallback = animationFinishedCallback;
                queuedAnimation = -1;

                Animations[currentAnimation].Start( );
            } else {
                queuedAnimation = GetIndex(name);
                queuedCallback = animationFinishedCallback;
            }
        }

        public void Reset ( ) {
            currentAnimation = idleAnimation;
            Animations[currentAnimation].Start( );
        }

        private int GetIndex (string name) {
            for (int i = 0; i < Animations.Length; i++) {
                if (Animations[i].Name == name) {
                    return i;
                }
            }
            return idleAnimation;
        }

        private class Animation {
            public string Name;
            public Frame[ ][ ] Frames;
            public Frame Frame { get { return Frames[variation][currentFrame]; } }

            private int variation;
            private int currentFrame;
            private float timeLeft;

            public bool Update (DeltaTime dt) {
                timeLeft -= dt.Milliseconds;

                if (timeLeft < 0) {
                    if (currentFrame == Frames[variation].Length - 1) {
                        return false;
                    }
                    currentFrame++;
                    timeLeft = Frame.Time;
                }
                return true;
            }

            public void Start ( ) {
                variation = Mathi.Random(0, Frames.Length);
                currentFrame = 0;
                timeLeft = Frame.Time;
            }
        }

        private struct Frame {
            public int Time;
            public string Texture;
        }
    }
}
