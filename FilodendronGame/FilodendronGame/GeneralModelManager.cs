using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace FilodendronGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GeneralModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Model box;
        Model filodendron;
        Texture2D boxTexture;
        Texture2D avatarTexture;

        // Set the avatar position and rotation variables.
        public Vector3 avatarPosition = new Vector3(0, 0, -50);
 
        // Set rates in world units per 1/60th second 
        // (the default fixed-step interval).
        public float rotationSpeed = 1f / 500f;
        public float forwardSpeed = 200f / 60f;

        public float avatarYaw;
        public Matrix World = Matrix.Identity;

        MouseState prevMouseState;

        public GeneralModelManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            box = Game.Content.Load<Model>(@"models/box");
            filodendron = Game.Content.Load<Model>(@"models/spaceship");
            boxTexture = Game.Content.Load<Texture2D>(@"textures/boxtexture");
            avatarTexture = Game.Content.Load<Texture2D>(@"textures/avatartexture");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            UpdateAvatarPosition();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        { 
            //Game.graphics.GraphicsDevice.Clear(Color.SteelBlue);
                    
            DrawBoxes();
            World = Matrix.CreateRotationY(avatarYaw) *
                Matrix.CreateTranslation(avatarPosition);

            DrawModel(filodendron, World, avatarTexture, ((Game1)Game).camera);

            base.Draw(gameTime);
        }
          
         /// <summary>
        /// Update the position and direction of the avatar.
        /// </summary>
        void UpdateAvatarPosition()
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
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                Matrix forwardMovement = Matrix.CreateRotationY(avatarYaw);
                Vector3 v = new Vector3(0, 0, -forwardSpeed);
                v = Vector3.Transform(v, forwardMovement);
                avatarPosition.Z += v.Z;
                avatarPosition.X += v.X;
            }
            prevMouseState = Mouse.GetState();

            //if the side boundry of screen reached, set the mouse on the other side
            if (Mouse.GetState().X >= Game.Window.ClientBounds.Width)
            {
                Mouse.SetPosition(1, Mouse.GetState().Y);// jak wstawia³em 0 to mi ekran przeskakiwa³ lol
                prevMouseState = Mouse.GetState();
            }
            if (Mouse.GetState().X <= 0)
            {
                Mouse.SetPosition(Game.Window.ClientBounds.Width, Mouse.GetState().Y);
                prevMouseState = Mouse.GetState();
            }
        }
         
         void DrawBoxes()
        {
            for (int z = 0; z < 9; z++)
            {
                for (int x = 0; x < 9; x++)
                {
                    DrawModel(box,
                        Matrix.CreateTranslation(x * 60, 0, z * 60),
                        boxTexture, ((Game1)Game).camera);
                }
            }
        } 
         
        /// <summary>
        /// Draws the 3D specified model.
        /// </summary>
        /// <param name="model">The 3D model being drawn.</param>
        /// <param name="world">Transformation matrix for world coords.
        /// </param>
        /// <param name="texture">Texture used for the drawn 3D model.
        /// </param>
        void DrawModel(Model model, Matrix world, Texture2D texture, Camera camera)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.Projection = camera.proj;
                    be.View = camera.view;
                    be.World = world;
                    be.Texture = texture;
                    be.TextureEnabled = true;
                }
                mesh.Draw();
            }
        }
    }
}
