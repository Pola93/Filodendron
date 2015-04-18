using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FilodendronGame.Interfaces;

namespace FilodendronGame.Abilities
{
    class FilodendronRigidBody : RigidBody
    {
        Filodendron filodendron;

        public FilodendronRigidBody(Filodendron f)
        {
            this.filodendron = f;
        }
        public void UpdateRigidBody(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public bool CollidesWith(Model otherModel, Matrix otherWorld)
        {
            // Loop through each ModelMesh in both objects and compare 
            // all bounding spheres for collisions  
            foreach (ModelMesh myModelMeshes in filodendron.model.Meshes)
            {
                foreach (ModelMesh hisModelMeshes in otherModel.Meshes)
                {
                    if (myModelMeshes.BoundingSphere.Transform(filodendron.World).Intersects(hisModelMeshes.BoundingSphere.Transform(otherWorld)))
                        return true;
                }
            }
            return false; 
        }
    }
}
