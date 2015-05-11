using FilodendronGame.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilodendronGame.Abilities
{
    class PlatformAnimation : Animation
    {
        private float startPosition;
        private float stopPosition;
        private bool animationStatus;
        private Vector3 position;

        public PlatformAnimation(Vector3 startPosition, float stopPosition)
        {
            this.startPosition = startPosition.Z;
            this.position = startPosition;
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
            if (position.Z >= startPosition)
            {
                animationStatus = true;
            }
            if (position.Z <= stopPosition)
            {
                animationStatus = false;
            }
        }

        public Matrix AnimationForward()
        {
            position += new Vector3(0, 0, 1);
            return Matrix.CreateTranslation(position);
        }

        public Matrix AnimationBackward()
        {
            position -= new Vector3(0, 0, 1);
            return Matrix.CreateTranslation(position);
        }
    }
}
