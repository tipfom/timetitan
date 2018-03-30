using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.World.Mobs
{
    public class Octopus : Mob {
        public Octopus () : base(EntitySpecies.OCTOPUS) {
            Health = 5;
        }
    }
}
