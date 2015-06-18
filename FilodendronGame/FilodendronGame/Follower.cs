using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FilodendronGame
{
    public class Follower : BasicModel
    {
        public bool visible = true;
        public Vector3 followerSpeed;
        public Vector3 followerPosition;
        public Filodendron master;
        private bool draw = true;
        private bool check = false;

        public Follower(Model model, Matrix world) : base(model, world)
        {
            followerPosition = World.Translation;
        }

        public static Quaternion GetRotation(Vector3 source, Vector3 dest, Vector3 up)
        {
            float dot = Vector3.Dot(source, dest);

            if (Math.Abs(dot + 1.0f) < 0.000001f)
            {
                // vector a and b point exactly in the opposite direction, 
                // so it is a 180 degrees turn around the up-axis
                return new Quaternion(up, MathHelper.ToRadians(180.0f));
            }
            if (Math.Abs(dot - 1.0f) < 0.000001f)
            {
                // vector a and b point exactly in the same direction
                // so we return the identity quaternion
                return Quaternion.Identity;
            }

            float rotAngle = (float)Math.Acos(dot);
            Vector3 rotAxis = Vector3.Cross(source, dest);
            rotAxis = Vector3.Normalize(rotAxis);
            return Quaternion.CreateFromAxisAngle(rotAxis, rotAngle);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float distance = (followerPosition - master.avatarPosition).Length();
            World = master.World;

            Vector3 newF = Vector3.Normalize(followerPosition - master.avatarPosition);
            Quaternion rotation = GetRotation(Vector3.Forward, newF, Vector3.Up);
            rotation.Y = 0;

            if (distance > 300)
            {
                check = true;
            
                followerSpeed.X = master.avatarSpeed.X * (float)Math.Cos(rotation.X) - master.avatarSpeed.Z * (float)Math.Sin(rotation.Z) * - 1f;
                followerSpeed.Z = master.avatarSpeed.X * (float)Math.Sin(rotation.X) - master.avatarSpeed.Z * (float)Math.Cos(rotation.Z) * - 1f;
                followerSpeed.Y = 0;

            }

            if (distance < 180 || distance > 600)
            {
                check = false;
                followerSpeed = new Vector3(0, 0, 0);
            }

            followerSpeed.X = followerSpeed.X * (float)Math.Cos(rotation.X) - master.avatarSpeed.Z * (float)Math.Sin(rotation.Z) * -1f;
            followerSpeed.Z = followerSpeed.X * (float)Math.Sin(rotation.X) - master.avatarSpeed.Z * (float)Math.Cos(rotation.Z) * -1f;
            followerSpeed.Y = 0;
            
            UpdateBulletPosition(gameTime);

            World = Matrix.CreateTranslation(followerPosition);
        }

        public void UpdateBulletPosition(GameTime gameTime)
        {
            followerPosition.X += followerSpeed.X;
            followerPosition.Y += followerSpeed.Y;
            followerPosition.Z += followerSpeed.Z;
        }

        public override void Draw(Model model, Matrix world, Texture2D texture, Camera camera, GameTime gameTime, GraphicsDeviceManager graphics)
        {
            if (master.avatarPosition.Y < 500 && draw) base.Draw(model, world, texture, camera, gameTime, graphics);
            else draw = false;
        }
    }
}
