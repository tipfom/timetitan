using System;
using System.Collections.Generic;
using System.Text;
using Core;

namespace Universal.UI.Layout {
    public class Box {
        private Vector2 _Size;
        public Vector2 Size { get { return _Size; } set { _Size = value; UpdateVerticies( ); } }

        private Vector2 _Position;
        public Vector2 Position { get { return _Position; } set { _Position = value; UpdateVerticies( ); } }

        public float Width { get { return Size.X; } }
        public float Height { get { return Size.Y; } }

        private float _Rotation;
        public float Rotation { get { return _Rotation; } set { _Rotation = value; UpdateVerticies( ); } }

        private float[ ] tempVerticies = new float[8];
        public float[ ] Verticies = new float[8];

        public Box (float x, float y, float width, float height, float rotation = 0) : this(new Vector2(x, y), new Vector2(width, height), rotation) {
        }

        public Box (Vector2 position, Vector2 size, float rotation = 0) {
            _Position = position;
            _Size = size;
            _Rotation = rotation;
            UpdateVerticies( );
        }

        public bool Collides (Vector2 point) {
            Vector2 xAxis = new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation));
            Vector2 yAxis = new Vector2(xAxis.Y, -xAxis.X);

            float pointX = xAxis.X * point.X + xAxis.Y * point.Y;
            float pointY = yAxis.X * point.X + yAxis.Y * point.Y;
            
            float thisXMin = float.PositiveInfinity;
            float thisYMin = float.PositiveInfinity;

            float thisXMax = float.NegativeInfinity;
            float thisYMax = float.NegativeInfinity;


            for (int i = 0; i < 5; i += 4) {
                float temp;

                temp = Verticies[i] * xAxis.X + Verticies[i + 1] * xAxis.Y;
                if (temp < thisXMin) thisXMin = temp;
                if (temp > thisXMax) thisXMax = temp;

                temp = Verticies[i] * yAxis.X + Verticies[i + 1] * yAxis.Y;
                if (temp < thisYMin) thisYMin = temp;
                if (temp > thisYMax) thisYMax = temp;
            }

            return !(
                pointX < thisXMin ||
                pointX > thisXMax ||
                pointY < thisYMin || 
                pointY > thisYMax);
        }

        public bool Collides (Box box) {
            return this.SAT(box) || box.SAT(this);
        }

        private bool SAT (Box box) {
            Vector2 xAxis = new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation));
            Vector2 yAxis = new Vector2(xAxis.Y, -xAxis.X);

            float thisXMin = float.PositiveInfinity;
            float boxXMin = float.PositiveInfinity;
            float thisYMin = float.PositiveInfinity;
            float boxYMin = float.PositiveInfinity;

            float thisXMax = float.NegativeInfinity;
            float boxXMax = float.NegativeInfinity;
            float thisYMax = float.NegativeInfinity;
            float boxYMax = float.NegativeInfinity;

            for (int i = 0; i < 8; i += 2) {
                float temp;

                temp = Verticies[i] * xAxis.X + Verticies[i + 1] * xAxis.Y;
                if (temp < thisXMin) thisXMin = temp;
                if (temp > thisXMax) thisXMax = temp;

                temp = Verticies[i] * yAxis.X + Verticies[i + 1] * yAxis.Y;
                if (temp < thisYMin) thisYMin = temp;
                if (temp > thisYMax) thisYMax = temp;

                temp = box.Verticies[i] * xAxis.X + box.Verticies[i + 1] * xAxis.Y;
                if (temp < boxXMin) boxXMin = temp;
                if (temp > boxXMax) boxXMax = temp;

                temp = box.Verticies[i] * yAxis.X + box.Verticies[i + 1] * yAxis.Y;
                if (temp < boxYMin) boxYMin = temp;
                if (temp > boxYMax) boxYMax = temp;
            }

            return !(
                thisXMin > boxXMax ||
                thisXMax < boxXMin ||
                thisYMin > boxYMax ||
                thisYMax < boxYMin);
        }

        private void UpdateVerticies ( ) {
            if (_Rotation != 0) {
                tempVerticies[0] = -Size.X / 2f;
                tempVerticies[1] = Size.Y / 2f;
                tempVerticies[2] = tempVerticies[0]; // Left
                tempVerticies[3] = -tempVerticies[1]; // -Top = Bottom
                tempVerticies[4] = -tempVerticies[0]; // -Left = Right
                tempVerticies[5] = tempVerticies[3]; // Bottom
                tempVerticies[6] = tempVerticies[4]; // Right
                tempVerticies[7] = tempVerticies[1]; // Top

                Mathf.TransformAtOrigin(tempVerticies, ref Verticies, Position.X + tempVerticies[4], Position.Y - tempVerticies[1], Rotation);
            } else {
                Verticies[0] = Position.X;
                Verticies[1] = Position.Y;
                Verticies[2] = Verticies[0]; // Left
                Verticies[3] = Position.Y - Size.Y;
                Verticies[4] = Position.X + Size.X;
                Verticies[5] = Verticies[3]; // Bottom
                Verticies[6] = Verticies[4]; // Right
                Verticies[7] = Verticies[1]; // Top
            }
        }

        public static float[ ] GetVerticies (float x, float y, float width, float height) {
            return new float[8] {
                x, y,
                x, y - height,
                x + width, y - height,
                x + width, y
            };
        }

        public static float[ ] GetVerticies (Vector2 position, Vector2 size) {
            return new float[8] {
                position.X, position.Y,
                position.X, position.Y - size.Y,
                position.X + size.X, position.Y - size.Y,
                position.X + size.X, position.Y
            };
        }
    }
}
