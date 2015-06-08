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
        public Matrix World { get; set; }
        private float startPosition;
        private float stopPosition;
        private bool animationStatus;
        private Vector3 position;
        private char direction;
        private float positionDirection;

        public PlatformAnimation(Vector3 startPosition, float stopPosition, char direction)
        {
            this.direction = direction;
            if (direction == 'X') this.startPosition = startPosition.X;
            if (direction == 'Y') this.startPosition = startPosition.Y;
            if (direction == 'Z') this.startPosition = startPosition.Z;
            this.position = startPosition;
            this.stopPosition = stopPosition;
        }

        public Matrix UpdateAnimation()
        {
            IsAnimationStatus();
            if (direction == 'X') return UpdateAnimationX();
            if (direction == 'Y') return UpdateAnimationY();
            else return UpdateAnimationZ();
        }

        public Matrix UpdateAnimationX()
        {
            if (animationStatus)
            {
                //return AnimationLeftX();
                return Matrix.CreateTranslation(AnimationLeftX());
            }
            else
            {
                //return AnimationRightX();
                return Matrix.CreateTranslation(AnimationRightX());
            }
        }

        public Matrix UpdateAnimationY()
        {
            if (animationStatus)
            {
                //return AnimationDownY();
                return Matrix.CreateTranslation(AnimationDownY());
            }
            else
            {
                //return AnimationUpY();
                return Matrix.CreateTranslation(AnimationUpY());
            }
        }

        public Matrix UpdateAnimationZ()
        {
            if (animationStatus)
            {
                //return AnimationForwardZ();
                return Matrix.CreateTranslation(AnimationForwardZ());
            }
            else
            {
                //return AnimationBackwardZ();
                return Matrix.CreateTranslation(AnimationBackwardZ());
            }
        }

        public void savePositionDirection()
        {
            if (direction == 'X') positionDirection = position.X;
            if (direction == 'Y') positionDirection = position.Y;
            if (direction == 'Z') positionDirection = position.Z;
        }

        public void IsAnimationStatus()
        {
            savePositionDirection();
            if (startPosition > stopPosition)
            {
                if (positionDirection >= startPosition)
                {
                    animationStatus = true;
                }
                if (positionDirection <= stopPosition)
                {
                    animationStatus = false;
                }
            }
            if (stopPosition > startPosition)
            {
                if (positionDirection >= stopPosition)
                {
                    animationStatus = true;
                }
                if (positionDirection <= startPosition)
                {
                    animationStatus = false;
                }
            }
        }

        /* public Matrix AnimationForward()
         {
             position += new Vector3(0, 0, 1);
             return Matrix.CreateTranslation(position);
         }

         public Matrix AnimationBackward()
         {
             position -= new Vector3(0, 0, 1);
             return Matrix.CreateTranslation(position);
         }*/

        public Vector3 AnimationBackwardZ()
        {
            return position += Vector3.Backward;
        }

        public Vector3 AnimationForwardZ()
        {
            return position += Vector3.Forward;
        }

        public Vector3 AnimationDownY()
        {
            return position += Vector3.Down;
        }

        public Vector3 AnimationUpY()
        {
            return position += Vector3.Up;
        }

        public Vector3 AnimationRightX()
        {
            return position += Vector3.Right;
        }

        public Vector3 AnimationLeftX()
        {
            return position += Vector3.Left;
        }
    }
}
