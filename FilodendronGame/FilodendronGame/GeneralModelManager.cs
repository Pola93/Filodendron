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
        // Everything with comment "for boxes" will be deleted later
        Model box;
        Texture2D boxTexture;
        Texture2D avatarTexture; // ##########czy textury trzymac w modelmanager czy w klasach poszczegolnych modeli?

        public Filodendron avatar;
        public Matrix World = Matrix.Identity; // for boxes

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            box = Game.Content.Load<Model>(@"models/box");
            avatar = new Filodendron(Game.Content.Load<Model>(@"models\spaceship"));
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
            avatar.Update();
            //if the side boundry of screen reached, set the mouse on the other side
            if (Mouse.GetState().X >= Game.Window.ClientBounds.Width)
            {
                Mouse.SetPosition(1, Mouse.GetState().Y);// jak wstawia³em 0 to mi ekran przeskakiwa³ lol
                avatar.prevMouseState = Mouse.GetState();
            }
            if (Mouse.GetState().X <= 0)
            {
                Mouse.SetPosition(Game.Window.ClientBounds.Width, Mouse.GetState().Y);
                avatar.prevMouseState = Mouse.GetState();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {                     
            DrawBoxes();
            avatar.Draw(avatar.model, avatar.World, avatarTexture, ((Game1)Game).camera);

            base.Draw(gameTime);
        }
          
                
        void DrawBoxes() // for boxes
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
        void DrawModel(Model model, Matrix world, Texture2D texture, Camera camera) // for boxes
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
