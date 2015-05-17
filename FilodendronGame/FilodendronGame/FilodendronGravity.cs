using FilodendronGame.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilodendronGame
{
    public class FilodendronGravity : Gravity
    {
        public float gravity = 1f;
        
        public float UpdateSpeed(GameTime time)
        {
            return gravity * (float)time.ElapsedGameTime.TotalSeconds;
        }
    }
}
