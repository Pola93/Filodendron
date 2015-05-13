using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FilodendronGame.Interfaces;
using FilodendronGame.Abilities;

namespace FilodendronGame
{
    public class BasicModel
    {
        public Model model { get; protected set; }
        public Matrix World { get; protected set; }

        public RigidBody rigidBody;
        public Gravity gravity;
        public Animation animation;
        public Audio audio;

        public BasicModel(Model m, Matrix world)
        {
            this.model = m;
            this.World = world;
        }

        public BasicModel(Model m, Matrix world, float scale)
        {
            this.model = m;
            this.World = world;
            World = Matrix.CreateScale(scale) * World;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (animation != null) 
            {
                World=animation.UpdateAnimation(gameTime);
            }
            if (rigidBody != null) 
            {
                rigidBody.UpdateRigidBody(gameTime);
            }

        }

        public Matrix ScaleModel(float scale) 
        {
            return Matrix.CreateScale(scale) * World;
        }

        public virtual void Draw(Model model, Matrix world, Texture2D texture, Camera camera, GameTime gameTime, GraphicsDeviceManager graphics)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = camera.proj;
                    be.View = camera.view;
                    be.World = world * transforms[mesh.ParentBone.Index];
                    //be.World = world * mesh.ParentBone.Transform;  ######pytanie czym to sie rozni i o co chodzi
                    //be.Texture = texture;
                    //be.TextureEnabled = true;
                }
                mesh.Draw();
            }
        }
    }
}
