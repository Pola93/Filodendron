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

        public BasicModel(Model m, Matrix world)
        {
            this.model = m;
            this.World = world;
        }

        public virtual void Update(GameTime gameTime)
        {

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
                    be.Texture = texture;
                    be.TextureEnabled = true;
                }
                mesh.Draw();
            }
        }
        public bool CollidesWith(Model otherModel, Matrix otherWorld)
        {  
            // Loop through each ModelMesh in both objects and compare 
            // all bounding spheres for collisions  
            foreach (ModelMesh myModelMeshes in model.Meshes) 
            {  
                foreach (ModelMesh hisModelMeshes in otherModel.Meshes)    
                {      
                    if (myModelMeshes.BoundingSphere.Transform(World).Intersects(hisModelMeshes.BoundingSphere.Transform(otherWorld)))  
                        return true;   
                }
            }  
            return false; 
        } 
    }
}
