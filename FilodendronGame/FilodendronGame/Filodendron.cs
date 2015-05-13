using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FilodendronGame.Interfaces;
using FilodendronGame.Abilities;
using SkinnedModel;

namespace FilodendronGame
{
    public class Filodendron : BasicModel
    {
        public AnimationPlayer animationPlayer;
        public SkinningData skinningData;
        public AnimationClip clip;
        public Texture2D avatarTexture;
        public Vector3 avatarPosition = new Vector3(0, 0, -50);
        public float rotationSpeed = 1f / 500f;
        public float forwardSpeed = 200f / 60f;
        public float backwardSpeed = -(100f / 60f);
        public float sideSpeed = 150f / 60f;
        public float avatarYaw;
        public float rotation=0;
        public MouseState prevMouseState;
        public Effect CustomShader;
        public bool stopPosition { get; set; }

        Vector3 viewVector; // for specular light

        public Filodendron(Model m, Matrix world)
            : base(m, world)
        {
            rigidBody = new FilodendronRigidBody(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateAvatarPosition(gameTime);
            World = Matrix.CreateRotationY(avatarYaw) *
                Matrix.CreateTranslation(avatarPosition); //potem przerzuc nizej do metody
        }
        /// <summary>
        /// Update the position and direction of the avatar.
        /// </summary>

        void UpdateAvatarPosition(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            if (Mouse.GetState().X != prevMouseState.X)
            {
                avatarYaw -= (Mouse.GetState().X - prevMouseState.X) * rotationSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                Matrix forwardMovement = Matrix.CreateRotationY(avatarYaw);
                Vector3 v = new Vector3(0, 0, forwardSpeed);
                v = Vector3.Transform(v, forwardMovement);
                avatarPosition.Z += v.Z;
                avatarPosition.X += v.X;
                animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                Matrix backwardMovement = Matrix.CreateRotationY(avatarYaw);
                Vector3 v = new Vector3(0, 0, backwardSpeed);
                v = Vector3.Transform(v, backwardMovement);
                avatarPosition.Z += v.Z;
                avatarPosition.X += v.X;
                animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                Matrix sideMovement = Matrix.CreateRotationY(avatarYaw);
                Vector3 v = new Vector3(sideSpeed, 0, 0);
                v = Vector3.Transform(v, sideMovement);
                avatarPosition.Z += v.Z;
                avatarPosition.X += v.X;
                animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                Matrix sideMovement = Matrix.CreateRotationY(avatarYaw);
                Vector3 v = new Vector3(-sideSpeed, 0, 0);
                v = Vector3.Transform(v, sideMovement);
                avatarPosition.Z += v.Z;
                avatarPosition.X += v.X;
                animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            }
            if (!keyboardState.IsKeyDown(Keys.W) && !keyboardState.IsKeyDown(Keys.A)
                && !keyboardState.IsKeyDown(Keys.D) && !keyboardState.IsKeyDown(Keys.S))
            {
                //Stop animation for player walking
                animationPlayer.Update(new TimeSpan(0, 0, 0), true, Matrix.Identity);
            }
            prevMouseState = Mouse.GetState();
        }

        public override void Draw(Model model, Matrix world, Texture2D texture, Camera camera, GameTime gameTime, GraphicsDeviceManager graphics)
        {
            //nie usuwać, jeszcze może się przydać!!!
            /*viewVector = Vector3.Transform(avatarPosition - camera.cameraPosition, Matrix.CreateRotationY(avatarYaw));
            viewVector.Normalize();

            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = CustomShader;
                    CustomShader.CurrentTechnique = CustomShader.Techniques["Specular"];
                    CustomShader.Parameters["xWorldViewProjection"].SetValue(
                        world * mesh.ParentBone.Transform * camera.view * camera.proj);
                    CustomShader.Parameters["ViewVector"].SetValue(viewVector);
                    CustomShader.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    CustomShader.Parameters["ModelTexture"].SetValue(texture);

                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    CustomShader.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                }
                mesh.Draw();
            }
            DrawBoundingBox(camera, graphics);*/

            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Matrix[] bones = animationPlayer.GetSkinTransforms();

            // Compute camera matrices.
            Matrix view = camera.view;

            Matrix projection = camera.proj;

            // Render the skinned mesh.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    effect.World = world * mesh.ParentBone.Transform;
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }
            DrawBoundingBox(camera, graphics);

        }
        public void DrawBoundingBox(Camera camera, GraphicsDeviceManager graphics)
        {
            short[] bBoxIndices = {
                0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
            };
            List<BoundingBox> boundingBoxes = new List<BoundingBox>();
            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                boundingBoxes.Add(BoundingBox.CreateFromSphere(myModelMeshes.BoundingSphere));
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
