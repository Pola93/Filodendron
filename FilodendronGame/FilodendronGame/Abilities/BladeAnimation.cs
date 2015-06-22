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
        public Matrix World { get; set; }
        private float startPosition;
        public float rotation;
        private float stopPosition;
        private bool animationStatus;
        private Vector3 position;
        private char direction;
        private float speed;
        private float positionDirection;

        public BladeAnimation(Vector3 startPosition, float stopPosition, float speed, char direction)
        {
            this.direction = direction;
            if (direction == 'X') this.startPosition = startPosition.X;
            if (direction == 'Y') this.startPosition = startPosition.Y;
            if (direction == 'Z') this.startPosition = startPosition.Z;
            this.position = startPosition;
            this.stopPosition = stopPosition;
            this.speed = speed;
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
                return AnimationLeftX();
            }
            else
            {
                return AnimationRightX();
            }
        }

        public Matrix UpdateAnimationY()
        {
            if (animationStatus)
            {
                return AnimationDownY();
            }
            else
            {
                return AnimationUpY();
            }
        }

        public Matrix UpdateAnimationZ()
        {
            if (animationStatus)
            {
                return AnimationForwardZ();
            }
            else
            {
                return AnimationBackwardZ();
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

        public Matrix AnimationBackwardZ()
        {
            position += Vector3.Multiply(Vector3.Backward, speed);
            rotation += 0.03f;
            return Matrix.CreateRotationX(rotation) * Matrix.CreateTranslation(position);
        }

        public Matrix AnimationForwardZ()
        {
            position += Vector3.Multiply(Vector3.Forward, speed);
            rotation -= 0.03f;
            return Matrix.CreateRotationX(rotation) * Matrix.CreateTranslation(position);
        }

        public Matrix AnimationDownY()
        {
            position += Vector3.Multiply(Vector3.Down, speed);
            rotation += 0.03f;
            return Matrix.CreateRotationX(rotation) * Matrix.CreateTranslation(position);
        }

        public Matrix AnimationUpY()
        {
            position += Vector3.Multiply(Vector3.Up, speed);
            rotation -= 0.03f;
            return Matrix.CreateRotationX(rotation) * Matrix.CreateTranslation(position);
        }

        public Matrix AnimationRightX()
        {
            position += Vector3.Multiply(Vector3.Right, speed);
            rotation += 0.03f;
            return Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position);
        }

        public Matrix AnimationLeftX()
        {
            position += Vector3.Multiply(Vector3.Left, speed);
            rotation -= 0.03f;
            return Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(position);
        }


        public Vector3 avatarPositionChange
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool isTrap
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public float currentTime
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
