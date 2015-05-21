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

            Debug.WriteLine(bulletPosition);
        }

        public void Shoot(Matrix world, Vector3 position)
        {
            visible = true;
            bulletSpeed = new Vector3(-10, 0, 10);
            bulletPosition = position;
            Debug.WriteLine(position);
            Debug.WriteLine(world);
            this.World = world;
        }

        public override void Draw(Model model, Matrix world, Texture2D texture, Camera camera, GameTime gameTime, GraphicsDeviceManager graphics)
        {
            base.Draw(model, world, texture, camera, gameTime, graphics);
        }
    }
}
