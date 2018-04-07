using OpenTK;
using Vector2 = Core.Vector2;

namespace Universal.Graphics {
    public class Matrix {
        private Matrix4 _View;
        public Matrix4 View { get { return _View; } set { _View = value; } }

        private Matrix4 _Projection;
        public Matrix4 Projection { get { return _Projection; } }

        private Matrix4 _MVP;
        public Matrix4 MVP { get { return _MVP; } }

        public Matrix (Vector2 projectionsize, float zNear = -1, float zFar = 3f) {
            ResetView( );
            UpdateProjection(projectionsize, zNear, zFar);
            CalculateMVP( );
        }

        public void UpdateProjection (Vector2 projectionsize, float zNear = -1, float zFar = 3f) {
            _Projection = Matrix4.CreateOrthographicOffCenter(-projectionsize.X, projectionsize.X, -projectionsize.Y, projectionsize.Y, zNear, zFar);
        }

        public void CalculateMVP ( ) {
            _MVP = Matrix4.Mult(_View, _Projection);
        }

        public void ResetView ( ) {
            _View = Matrix4.LookAt(0, 0, 3, 0f, 0f, 0f, 0f, 1f, 0f);
        }

        public void ResetView (float x, float y) {
            _View = Matrix4.LookAt(0, 0, 3, -x, y, 0f, 0f, 1f, 0f);
        }

        public void TranslateView (float x, float y, float z) {
            _View = Matrix4.Mult(_View, Matrix4.CreateTranslation(x, y, z));
        }

        public static Matrix Default { get; set; } = new Matrix(new Vector2(Window.Ratio, 1));
    }
}
