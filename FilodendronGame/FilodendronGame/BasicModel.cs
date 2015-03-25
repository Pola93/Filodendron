using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FilodendronGame
{
    public class BasicModel
    {
        public Model model { get; protected set; }
        public Matrix World { get; protected set; }

        public BasicModel(Model m)
        {
            model = m;
            World = Matrix.Identity;
        }

        public virtual void Update()
        {

        }

        /*public void Draw(Model model, Matrix world, Texture2D texture, Camera camera)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.Projection = camera.proj;
                    be.View = camera.view;
                    be.World = world;
                    be.Texture = texture;
                    be.TextureEnabled = true;
                }
                mesh.Draw();
            }
        }*/
        public void Draw(Model model, Matrix world, Texture2D texture, Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();//
                    be.Projection = camera.proj;
                    be.View = camera.view;
                    be.World = world * transforms[mesh.ParentBone.Index] /*mesh.ParentBone.Transform*/; //######pytanie czym to sie rozni i o co chodzi
                    be.Texture = texture;
                    be.TextureEnabled = true;
                }
                mesh.Draw();
            }
        }
    }
}
