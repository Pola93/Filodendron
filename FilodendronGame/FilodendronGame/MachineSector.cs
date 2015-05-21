using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Text;

namespace FilodendronGame
{
    class MachineSector : BasicModel
    {

        public MachineSector(Model m, Matrix world)
            : base(m, world)
        {

        }
        public override void Update(GameTime gameTime)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                //System.Diagnostics.Debug.WriteLine(mesh.Name); //wazne odkrycie odwolanie po nazwie
                if (mesh.Name.Equals("karton03"))
                {
                    //mesh.ParentBone.Transform = Matrix.CreateTranslation(new Vector3(0,-50,0));
                }
            }

            base.Update(gameTime);
        }
        public override void Draw(Model model, Matrix world, Texture2D texture, Camera camera, GameTime gameTime, GraphicsDeviceManager graphics)
        {
            base.Draw(model, world, texture, camera, gameTime, graphics);
            //CreateBoundingBox(camera, graphics);
        }
    }
}
