﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FilodendronGame.Interfaces
{
    public interface Animation
    {
        Matrix UpdateAnimation(GameTime gameTime);
    }
}
