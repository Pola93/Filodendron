using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FilodendronGame.Interfaces;
using FilodendronGame.Abilities;
using System.Diagnostics;

namespace FilodendronGame
{
    public class BasicModel
    {
        public bool boundingBox = false;
        public Model model { get; protected set; }
        public Matrix World { get; protected set; }

        public List<BoundingBox> boundingBoxes;

        public RigidBody rigidBody;
        public Animation animation;
        public Audio audio;

        public List<String> names;

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
                World = animation.UpdateAnimation(gameTime);
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
            graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
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
            //DrawBoundingBox(camera, graphics);

            if (this.boundingBox == true)
            {
                CreateBoundingBox(camera, graphics);
            }
        }

        private BoundingBox BuildBoundingBox(ModelMesh mesh, Matrix meshTransform)
        {
            // Create initial variables to hold min and max xyz values for the mesh
            Vector3 meshMax = new Vector3(float.MinValue);
            Vector3 meshMin = new Vector3(float.MaxValue);

            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                // The stride is how big, in bytes, one vertex is in the vertex buffer
                // We have to use this as we do not know the make up of the vertex
                int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[part.NumVertices];
                part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, stride);

                // Find minimum and maximum xyz values for this mesh part
                Vector3 vertPosition = new Vector3();
 
                for (int i = 0; i < vertexData.Length; i++)
                {
                    vertPosition = vertexData[i].Position;

                    // update our values from this vertex
                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                }
            }


            // transform by mesh bone matrix
            meshMin = Vector3.Transform(meshMin, meshTransform);
            meshMax = Vector3.Transform(meshMax, meshTransform);

            // Create the bounding box
            BoundingBox box = new BoundingBox(meshMin, meshMax);
            //Debug.WriteLine(box);
            return box;
        }

        public void CreateBoundingBox(Camera camera, GraphicsDeviceManager manager)
        {
            short[] bBoxIndices = {
                0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
            };

            if (boundingBoxes == null)
            {
                boundingBoxes = new List<BoundingBox>();
                names = new List<string>();

                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in model.Meshes)
                {
                    //if (mesh.Name.Equals("floor") || mesh.Name.Equals("Box01"))
                    {
                        Matrix meshTransform = transforms[mesh.ParentBone.Index];
                        boundingBoxes.Add(BuildBoundingBox(mesh, meshTransform));
                        names.Add(mesh.Name);
                    }
                }
            }

            //foreach (BoundingBox box in boundingBoxes)
            //{
            //    Vector3[] corners = box.GetCorners();
            //    VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

            //    // Assign the 8 box vertices
            //    for (int i = 0; i < corners.Length; i++)
            //    {
            //        primitiveList[i] = new VertexPositionColor(corners[i], Color.White);
            //    }

            //    /* Set your own effect parameters here */
            //    BasicEffect be = new BasicEffect(manager.GraphicsDevice);

            //    be.World = World;
            //    be.View = camera.view;
            //    be.Projection = camera.proj;
            //    be.TextureEnabled = false;

            //    // Draw the box with a LineList
            //    foreach (EffectPass pass in be.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        manager.GraphicsDevice.DrawUserIndexedPrimitives(
            //            PrimitiveType.LineList, primitiveList, 0, 8,
            //            bBoxIndices, 0, 12);
            //    }
            //}
        }

        public void DrawBoundingBox(Camera camera, GraphicsDeviceManager graphics)
        {
            short[] bBoxIndices = {
                0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
            };
            List<BoundingBox> boundingBoxes = new List<BoundingBox>();
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix meshTransform;
            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                meshTransform = transforms[myModelMeshes.ParentBone.Index];
                boundingBoxes.Add(BuildBoundingBox(myModelMeshes, meshTransform));
            }

            foreach (BoundingBox box in boundingBoxes)
            {
                Vector3[] corners = box.GetCorners();
                VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

                // Assign the 8 box vertices
                for (int i = 0; i < corners.Length; i++)
                {
                    primitiveList[i] = new VertexPositionColor(corners[i], Color.White);
                }

                /* Set your own effect parameters here */
                BasicEffect be = new BasicEffect(graphics.GraphicsDevice);

                be.World = World;
                be.View = camera.view;
                be.Projection = camera.proj;
                be.TextureEnabled = false;

                // Draw the box with a LineList
                foreach (EffectPass pass in be.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.LineList, primitiveList, 0, 8,
                        bBoxIndices, 0, 12);
                }
            }
        }
    }
}
