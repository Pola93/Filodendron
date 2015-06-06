using FilodendronGame.Abilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilodendronGame
{
    class CollectableItem : BasicModel
    {
        public bool isCollected;
        public Matrix Position { get; private set; }
        public CollectableItem(Model m, Matrix world) : base(m, world)
        {
            this.isCollected = false;
            this.Position = world;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            animation.World = this.World;
        }
        public override void Draw(Model model, Matrix world, Texture2D texture, Camera camera, GameTime gameTime, GraphicsDeviceManager graphics)
        {
            base.Draw(model, world, texture, camera, gameTime, graphics);
        }
    }
}
