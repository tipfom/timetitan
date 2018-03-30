using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.World.Mobs
{
    public class Chest : Mob {
        public Chest () : base(EntitySpecies.CHEST) {
            Health = 15;
        }
    }
}
