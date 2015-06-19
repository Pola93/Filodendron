using FilodendronGame.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FilodendronGame.Abilities
{
    public class BulletRigidBody : RigidBody
    {
        Bullet bullet;

        public BulletRigidBody(Bullet bullet)
        {
            this.bullet = bullet;
        }

        public bool CollidesWith(Model otherModel, Matrix otherWorld)
        {
            return false;
        }
        public bool DetectCollision()
        {
            return false;
        }

        public void UpdateRigidBody(GameTime gameTime)
        {
            foreach (BasicModel model in GeneralModelManager.platforms)
            {
                CollidesWith(model);
            }
        }
        
        public void CollidesWith(BasicModel model)
        {
            if (model.boundingBoxes != null)
            {
                foreach (ModelMesh a in bullet.model.Meshes)
                {
                    foreach (BoundingBox b in model.boundingBoxes)
                    {
                        if (intersectsWith(b, a.BoundingSphere.Transform(bullet.World)))
                        {
                            bullet.hit = true;
                            if (model.isActivated())
                            {
                                model.disactivate();
                            }
                            else
	                        {
                                model.activate();
	                        }
                        }
                    }
                }
            }
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
    }
}
