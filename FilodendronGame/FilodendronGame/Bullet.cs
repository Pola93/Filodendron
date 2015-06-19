using FilodendronGame.Abilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FilodendronGame
{
    public class Bullet : BasicModel
    {
        public bool visible = false;
        public Vector3 bulletSpeed;
        public Vector3 bulletPosition;
        public BulletRigidBody rb;
        public bool hit = false;

        private Vector3 bulletShift = new Vector3(0, 45, 0);

        public Bullet(Model model, Matrix world) : base(model, world)
        {
            bulletPosition = World.Translation;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateBulletPosition(gameTime);

            World = Matrix.CreateTranslation(bulletPosition);
        }

        public void UpdateBulletPosition(GameTime gameTime)
        {
            bulletPosition.X += bulletSpeed.X;
            bulletPosition.Y += bulletSpeed.Y;
            bulletPosition.Z += bulletSpeed.Z;

            //Debug.WriteLine(bulletPosition);
        }

        public void Shoot(Filodendron filodendron)
        {
            Matrix movement = Matrix.CreateRotationY(filodendron.avatarYaw + 0.8f);
            World = filodendron.World * Matrix.CreateRotationY(filodendron.avatarYaw);

            bulletSpeed = new Vector3(-50, 0, 50);
            bulletSpeed = Vector3.Transform(bulletSpeed, movement);
            bulletPosition = filodendron.avatarPosition + bulletShift;

            visible = true;
            Debug.WriteLine(filodendron.avatarPosition);
            Debug.WriteLine(filodendron.avatarYaw);
        }

        public override void Draw(Model model, Matrix world, Texture2D texture, Camera camera, GameTime gameTime, GraphicsDeviceManager graphics)
        {
            base.Draw(model, world, texture, camera, gameTime, graphics);
            this.rb.UpdateRigidBody(gameTime);
        }
    }
}
