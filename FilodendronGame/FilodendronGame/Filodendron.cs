using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FilodendronGame.Interfaces;
using FilodendronGame.Abilities;
using SkinnedModel;
using System.Diagnostics;

namespace FilodendronGame
{
    public class Filodendron : BasicModel
    {
        public AnimationPlayer animationPlayer;
        public SkinningData skinningData;
        public AnimationClip clip;
        public Texture2D avatarTexture;

        public Vector3 avatarOldPosition;
        public Vector3 avatarPosition;
        public Vector3 avatarResp = new Vector3(-4000, 40, 4000);
        public Vector3 avatarSpeed;
        public Bullet bullet;
        public Follower slave;

        public FilodendronGravity gravity;

        public MouseState prevMouseState;
        public Effect CustomShader;

        public float rotationSpeed = 1f / 500f;
        public float forwardSpeed;
        public float sideSpeed;
        public float verticalSpeed = 0;
        
        public float avatarYaw;
        public float rotation = 0;
        private float multiplier = 1f;

        // Flag for jump key
        // private bool spacePressed = false;
        // Flag for gravity switch
        private bool gravityPressed = false;
        private bool shootPressed = false;
        public bool stopPosition { get; set; }
        public bool avatarFell = false;

        public bool isModelVisible = true;
        public bool hasAvatarJustDied = false;
        private double nextBlinkTime = 0;
        private const int maxBlinksAmount = 5;
        private float blinksDone = 0;
        private int blinkToggleTime = 200;
        private float heightOfDeath = -18;

        //Vector3 viewVector; // for specular light

        public Filodendron(Model m, Matrix world) : base(m, world)
        {
            rigidBody = new FilodendronRigidBody(this);
            avatarPosition = World.Translation;
            gravity = new FilodendronGravity();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateAvatarPosition(gameTime);
            World = Matrix.CreateRotationY(avatarYaw) * Matrix.CreateTranslation(avatarPosition); //potem przerzuc nizej do metody

            if (hasAvatarJustDied && gameTime.TotalGameTime.TotalMilliseconds >= nextBlinkTime)
            {
                isModelVisible = !isModelVisible;
                this.blinksDone += 0.5f;
                nextBlinkTime = gameTime.TotalGameTime.TotalMilliseconds + blinkToggleTime;
                if (blinksDone >= maxBlinksAmount)
                {
                    hasAvatarJustDied = false;
                    blinksDone = 0;
                }
            }
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

            if (keyboardState.IsKeyDown(Keys.E))
            {
                if (!shootPressed)
                {
                    shootPressed = true;
                    Shoot();
                }
            }
            else
            {
                shootPressed = false;
            }

            // Switching gravity
            if (keyboardState.IsKeyDown(Keys.G))
            {
                if (!gravityPressed)
                {
                    gravity.allowGravity = !gravity.allowGravity;
                    gravityPressed = true;

                    // Just in case (for now)
                    if (!gravity.allowGravity) {
                        verticalSpeed = 0;
                    }
                }
            }
            else
            {
                gravityPressed = false;
            }

            forwardSpeed = keyboardState.IsKeyDown(Keys.W) ? 400f / 60f * multiplier :
                ((keyboardState.IsKeyDown(Keys.S)) ? -(100f / 60f) * multiplier : 0);

            sideSpeed = keyboardState.IsKeyDown(Keys.A) ? 150f / 60f * multiplier :
                ((keyboardState.IsKeyDown(Keys.D)) ? -(150f / 60f) * multiplier : 0);

            verticalSpeed = (gravity.allowGravity && !rigidBody.DetectCollision()) ? verticalSpeed - gravity.UpdateSpeed(gameTime) : 
                ((keyboardState.IsKeyDown(Keys.Space) && (verticalSpeed > -0.0001f && verticalSpeed < 0.0001f)) ? 20f / 2f * multiplier : 0);

            Matrix movement = Matrix.CreateRotationY(avatarYaw);
            Vector3 moveVector = new Vector3(sideSpeed, verticalSpeed, forwardSpeed);
            Vector3 oldPosition = this.avatarPosition;
            moveVector = Vector3.Transform(moveVector, movement);
            this.avatarSpeed = moveVector;
            UpdatePosition(moveVector);
            if (verticalSpeed<=heightOfDeath)
            {
                this.avatarFell= true;
            }
            if (forwardSpeed != 0 || sideSpeed != 0)
            {
                animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);  
            }
            else
            {
                animationPlayer.Update(new TimeSpan(0, 0, 0), true, Matrix.Identity);
            }
            prevMouseState = Mouse.GetState();
        }

        private void Shoot()
        {
            bullet.Shoot(this);
            bullet.hit = false;
        }

        public void UpdatePosition(Vector3 move)
        {
            avatarPosition.X += move.X;
            avatarPosition.Y += move.Y;
            avatarPosition.Z += move.Z;

            pressKeyToTeleportFilon();
        }

        private void pressKeyToTeleportFilon()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.D1))
	        {
                avatarPosition = new Vector3(-4000, 40, 4000);//start
	        }
            if (keyboardState.IsKeyDown(Keys.D2))
            {
                avatarPosition = new Vector3(1000, 1550, 3600);//brazowa skrzynia
            }
            if (keyboardState.IsKeyDown(Keys.D3))
            {
                avatarPosition = new Vector3(5000, 2130, 3650);//fioletowa polka1
            }
            if (keyboardState.IsKeyDown(Keys.D4))
            {
                avatarPosition = new Vector3(5000, 2130, -1550);//fioletowa polka 2
            }
            if (keyboardState.IsKeyDown(Keys.D5))
            {
                avatarPosition = new Vector3(-5449, 20, -3058);//koniec
            }
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
                    effect.Texture = avatarTexture;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }
            //DrawBoundingBox(camera, graphics);
        }

        public void Die(Game Game)
        {
            if (((Game1)Game).numberOfLifes > 0)
                ((Game1)Game).numberOfLifes--;
            avatarPosition = avatarResp;
            this.hasAvatarJustDied = true;
        }
    }
}
