using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FilodendronGame.Interfaces;
using System.Diagnostics;

namespace FilodendronGame.Abilities
{
    class FilodendronRigidBody : RigidBody
    {
        Filodendron filodendron;
        float distance;

        public FilodendronRigidBody(Filodendron f)
        {
            this.filodendron = f;
        }
        public void UpdateRigidBody(GameTime gameTime)
        {
            foreach(BasicModel otherModel in GeneralModelManager.allModels)
            {
                if (CollidesWith(otherModel))
                {
                    // Do nothing here
                }
            }
        }

        public bool DetectCollision()
        {
            foreach (BasicModel otherModel in GeneralModelManager.allModels)
            {
                if (CollidesWith(otherModel))
                {
                    return true;
                }
            }
            return false;
        }

        private float GetDistanceBetweenCenters(ModelMesh a, ModelMesh b)
        {
            return (float)Math.Sqrt(Math.Pow(a.BoundingSphere.Center.X - b.BoundingSphere.Center.X, 2) +
                Math.Pow(a.BoundingSphere.Center.Y - b.BoundingSphere.Center.Y, 2) +
                Math.Pow(a.BoundingSphere.Center.Z - b.BoundingSphere.Center.Z, 2));
        }

        public bool CollidesWith(BasicModel model)
        {
            if (model.boundingBoxes != null)
            {
                foreach (ModelMesh a in filodendron.model.Meshes)
                {
                    foreach (BoundingBox b in model.boundingBoxes)
                    {
                        if(intersectsWith(b, a.BoundingSphere.Transform(filodendron.World)))
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }

        public static bool intersectsWith(BoundingBox boundingBox, BoundingSphere sphere)
        {
            float dmin = 0;

            Vector3 center = sphere.Center;
            Vector3 bmin = boundingBox.Min;
            Vector3 bmax = boundingBox.Max;

            if (center.X < bmin.X)
            {
                dmin += (float)Math.Pow(center.X - bmin.X, 2);
            }
            else if (center.X > bmax.X)
            {
                dmin += (float)Math.Pow(center.X - bmax.X, 2);
            }

            if (center.Y < bmin.Y)
            {
                dmin += (float)Math.Pow(center.Y - bmin.Y, 2);
            }
            else if (center.Y > bmax.Y)
            {
                dmin += (float)Math.Pow(center.Y - bmax.Y, 2);
            }

            if (center.Z < bmin.Z)
            {
                dmin += (float)Math.Pow(center.Z - bmin.Z, 2);
            }
            else if (center.Z > bmax.Z)
            {
                dmin += (float)Math.Pow(center.Z - bmax.Z, 2);
            }

            return dmin <= (float)Math.Pow(sphere.Radius, 2);
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
