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
        private float speed;
        public Vector3 avatarPositionChange { get; set; }
        public Boolean isTrap { get; set; }

        public PlatformAnimation(Vector3 startPosition, float stopPosition, float speed, char direction)
        {
            avatarPositionChange = Vector3.Zero;
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
                return Matrix.CreateTranslation(AnimationLeftX());
            }
            else
            {
                return Matrix.CreateTranslation(AnimationRightX());
            }
        }

        public Matrix UpdateAnimationY()
        {
            if (animationStatus)
            {
                return Matrix.CreateTranslation(AnimationDownY());
            }
            else
            {
                return Matrix.CreateTranslation(AnimationUpY());
            }
        }

        public Matrix UpdateAnimationZ()
        {
            if (animationStatus)
            {
                return Matrix.CreateTranslation(AnimationForwardZ());
            }
            else
            {
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

        public Vector3 AnimationBackwardZ()
        {
            avatarPositionChange = Vector3.Multiply(Vector3.Backward, speed);
            return position += avatarPositionChange;
        }

        public Vector3 AnimationForwardZ()
        {
            avatarPositionChange = Vector3.Multiply(Vector3.Forward, speed);
            return position += avatarPositionChange;
        }

        public Vector3 AnimationDownY()
        {
            avatarPositionChange = Vector3.Multiply(Vector3.Down, speed);
            return position += avatarPositionChange;
        }

        public Vector3 AnimationUpY()
        {
            avatarPositionChange = Vector3.Multiply(Vector3.Up, speed);
            return position += avatarPositionChange;
        }

        public Vector3 AnimationRightX()
        {
            avatarPositionChange = Vector3.Multiply(Vector3.Right, speed);
            return position += avatarPositionChange;
        }

        public Vector3 AnimationLeftX()
        {
            avatarPositionChange = Vector3.Multiply(Vector3.Left, speed);
            return position += avatarPositionChange;
        }
    }
}
