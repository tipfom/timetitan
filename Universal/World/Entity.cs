using Core;
using Core.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Universal.Graphics.Renderer;

namespace Universal.World {
    public abstract class Entity {
        public readonly EntitySpecies Species;

        public Entity (EntitySpecies species) {
            Species = species;
        }
        
        public abstract IEnumerable<RenderableObject> Draw ( );

        public virtual void Update (DeltaTime deltaTime) {
        }
    }
}
