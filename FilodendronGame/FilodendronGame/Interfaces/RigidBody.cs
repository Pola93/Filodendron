using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FilodendronGame.Interfaces
{
    public interface RigidBody
    {
        void UpdateRigidBody(GameTime gameTime);
        bool CollidesWith(Model otherModel, Matrix otherWorld);
    }
}
