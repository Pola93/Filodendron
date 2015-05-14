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
        float distance;
        // My suggestion
        // Vector3 t;

        public FilodendronRigidBody(Filodendron f)
        {
            this.filodendron = f;
        }
        public void UpdateRigidBody(GameTime gameTime)
        {
            foreach(BasicModel otherModel in GeneralModelManager.allModels)
            {
                if (CollidesWith(otherModel.model, otherModel.World))
                {
                    StopPosition(otherModel);
                }
            }
        }

        /*
         * Also my suggestion
        private Vector3 GetDistanceBetweenCenters(ModelMesh a, ModelMesh b)
        {
            return new Vector3((float)Math.Pow(a.BoundingSphere.Center.X - b.BoundingSphere.Center.X, 2), 
                (float)Math.Pow(a.BoundingSphere.Center.X - b.BoundingSphere.Center.X, 2), 
                (float)Math.Pow(a.BoundingSphere.Center.X - b.BoundingSphere.Center.X, 2));
        }
         */

        private float GetDistanceBetweenCenters(ModelMesh a, ModelMesh b)
        {
            return (float)Math.Sqrt(Math.Pow(a.BoundingSphere.Center.X - b.BoundingSphere.Center.X, 2) +
                Math.Pow(a.BoundingSphere.Center.Y - b.BoundingSphere.Center.Y, 2) +
                Math.Pow(a.BoundingSphere.Center.Z - b.BoundingSphere.Center.Z, 2));
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
                    {
                        // My suggestion
                        // t = new Vector3(hisModelMeshes.BoundingSphere.Radius + myModelMeshes.BoundingSphere.Radius,
                        //     hisModelMeshes.BoundingSphere.Radius + myModelMeshes.BoundingSphere.Radius,
                        //     hisModelMeshes.BoundingSphere.Radius + myModelMeshes.BoundingSphere.Radius) - test(myModelMeshes, hisModelMeshes);
                        // End suggestion
                        distance = hisModelMeshes.BoundingSphere.Radius + myModelMeshes.BoundingSphere.Radius - GetDistanceBetweenCenters(myModelMeshes, hisModelMeshes);
                        filodendron.animationPlayer.Update(new TimeSpan(0, 0, 0), true, Matrix.Identity);
                        return true;
                    }
                }
            }
            return false; 
        }

        public void StopPosition(BasicModel otherModel)
        {
            if (filodendron.avatarPosition.X != 0)
            {
                filodendron.avatarPosition.X += Math.Sign(filodendron.avatarPosition.X) * distance;
            }

            if (filodendron.avatarPosition.Z != 0)
            {
                filodendron.avatarPosition.Z += Math.Sign(filodendron.avatarPosition.Z) * distance;
            }

            // Position for vertical axis
            if (filodendron.avatarPosition.Y != 0)
            {
                filodendron.avatarPosition.Y += Math.Sign(filodendron.avatarPosition.Y) * distance;
            }

            // Stop falling
            if (filodendron.avatarPosition.Y > 0) filodendron.verticalSpeed = 0;

            // My suggestion
            // filodendron.avatarPosition = filodendron.avatarPosition + t;
            // end suggestion
            filodendron.avatarPosition = 
                 new Vector3(filodendron.avatarPosition.X + -(float)Math.Sin(distance),
                     filodendron.avatarPosition.Y, filodendron.avatarPosition.Z + -(float)Math.Cos(distance));
            //filodendron.avatarPosition = Vector3.Reflect(filodendron.avatarPosition, new Vector3(1, 0, 0));
        }
    }
}
