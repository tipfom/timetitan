﻿using System;
using System.Collections.Generic;
using System.Text;
using Core.Graphics;

namespace Universal.World.Mobs {
    public class Chest : Mob {
        public Chest (long stage) : base(MobType.CHEST, 15, 250, stage) {
        }

        public override IEnumerable<RenderableObject> Draw ( ) {
            float top = 1.5f + offset;
            yield return new RenderableObject(new float[ ] { -0.75f, top, -0.75f, offset, 0.75f, offset, 0.75f, top }, "body", GetColor( ));
        }
    }
}
