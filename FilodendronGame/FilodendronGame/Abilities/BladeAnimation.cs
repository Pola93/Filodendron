using FilodendronGame.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilodendronGame.Abilities
{
    public class BladeAnimation : Animation
    {
        private float startPosition;
        public float rotation;
        private float stopPosition;
        private bool animationStatus;
        private Vector3 position;

        public BladeAnimation(Vector3 startPosition, float rotation, float stopPosition)
        {
            this.startPosition = startPosition.X;
            this.position = startPosition;
            this.rotation = rotation;
            this.stopPosition = stopPosition;
        }

        public Matrix UpdateAnimation(GameTime gameTime)
        {
            IsAnimationStatus();
            if (animationStatus)
            {
                return AnimationBackward();
            }
            else
            {
                return AnimationForward();
            }
        }

        public void IsAnimationStatus()
        {
            if (position.X >= startPosition)
            {
                animationStatus = true;
            }
            if (position.X <= stopPosition)
            {
                animationStatus = false;
            }
        }

        public Matrix AnimationForward()
        {
            position += new Vector3(1, 0, 0);
            rotation += 0.03f;
            return Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position);
        }

        public Matrix AnimationBackward()
        {
            position -= new Vector3(1, 0, 0);
            rotation -= 0.03f;
            return Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position);
        }
    }
}
