namespace Core.Graphics {
    public class RenderableObject {
        public Color Color { get; set; }
        public float[ ] Verticies { get; set; }
        public string Texture { get; set; }

        public RenderableObject(float[ ] verticies, string texture) : this(verticies, texture, Color.White) {

        }

        public RenderableObject(float[ ] verticies, string texture, Color color) {
            Verticies = verticies;
            Texture = texture;
            Color = color;
        }
    }
}
