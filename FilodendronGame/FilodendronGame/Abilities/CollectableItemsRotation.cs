﻿using FilodendronGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FilodendronGame.Abilities
{
    class CollectableItemsRotation : Animation
    {
        public Matrix World { get; set; }
        public float yawSpeed;
        //public Matrix world;
        public CollectableItemsRotation(float yaw, Matrix world)
        {
            this.yawSpeed = yaw;
            this.World = world;
        }

        public Matrix UpdateAnimation(GameTime gameTime)
        {
            return Matrix.CreateRotationY(yawSpeed) * World;
        }

        public void IsAnimationStatus()
        {
            throw new NotImplementedException();
        }

        public Matrix AnimationForward()
        {
            throw new NotImplementedException();
        }

        public Matrix AnimationBackward()
        {
            throw new NotImplementedException();
        }
    }
}
