﻿using System;
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
                        distance = hisModelMeshes.BoundingSphere.Radius; //+ myModelMeshes.BoundingSphere.Radius;
                        return true;
                    }
                }
            }
            return false; 
        }

        public void StopPosition(BasicModel otherModel)
        {
            if (filodendron.avatarPosition.X < 0) filodendron.avatarPosition.X -= distance;
            if (filodendron.avatarPosition.X > 0) filodendron.avatarPosition.X += distance;
           // if (filodendron.avatarPosition.Y < 0) filodendron.avatarPosition.Y -= otherModel.World.Translation.Y;
            //if (filodendron.avatarPosition.Y > 0) filodendron.avatarPosition.Y += otherModel.World.Translation.Y;
            if (filodendron.avatarPosition.Z < 0) filodendron.avatarPosition.Z -= distance;
            if (filodendron.avatarPosition.Z > 0) filodendron.avatarPosition.Z += distance; 
           //filodendron.avatarPosition.X = otherModel.World.Translation.X - distance;
            //filodendron.avatarPosition.Y += otherModel.World.Translation.Y - distance;
            //filodendron.avatarPosition.Z = otherModel.World.Translation.Z - distance;
            //filodendron.avatarPosition= 
                new Vector3(filodendron.avatarPosition.X+ -(float)Math.Sin(distance),
                    filodendron.avatarPosition.Y, filodendron.avatarPosition.Z + -(float)Math.Cos(distance));
            //filodendron.avatarPosition = Vector3.Reflect(filodendron.avatarPosition, new Vector3(1, 0, 0));
        }
    }
}
