using FilodendronGame.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilodendronGame
{
    public class FilodendronGravity
    {
        public float gravity = 9.8f;
        public bool allowGravity = true;
        
        public float UpdateSpeed(GameTime time)
        {
            return (allowGravity) ? gravity * (float)time.ElapsedGameTime.TotalSeconds : 0;
        }
    }
}
