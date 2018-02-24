using Core;
using Core.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.Graphics.Renderer;

namespace Universal.World {
    public abstract class Entity {

        public const int PLAYER = 0;
        public const int PLUGGER = 1;


        public readonly int Species;

      

        public Entity (int species) {
            Species = species;
        }
        
        public abstract IEnumerable<RenderableObject> Draw ( );

        public virtual void Update (DeltaTime deltaTime) {
        }
    }
}
