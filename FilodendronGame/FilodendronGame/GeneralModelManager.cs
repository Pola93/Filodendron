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
using SkinnedModel;
using FilodendronGame.Abilities;


namespace FilodendronGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GeneralModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // Everything with comment "for boxes" will be deleted later
        BasicModel box;
        Texture2D boxTexture;
		MachineSector sektorMaszyn;
        public static List<BasicModel> allModels = new List<BasicModel>();
        List<CollectableItem> collectableItems = new List<CollectableItem>();
        public static List<BasicModel> platforms = new List<BasicModel>();
        public List<BasicModel> blades = new List<BasicModel>();

        public Filodendron avatar;
        public Matrix World = Matrix.Identity; // for boxes

        //for explosion
        List<ParticleExplosion> explosions = new List<ParticleExplosion>();
        ParticleExplosionSettings particleExplosionSettings = new ParticleExplosionSettings();
        ParticleSettings particleSettings = new ParticleSettings();
        Texture2D explosionTexture;
        Texture2D explosionColorsTexture;
        Effect explosionEffect;

        //for ocean
        Ocean ocean;


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
            this.setCollectableItems();
            this.setPlatforms();
            this.setBlades();

            box = new BasicModel(Game.Content.Load<Model>(@"models\box"), Matrix.Identity);//Matrix.CreateTranslation(2000, 0, 185));
            boxTexture = Game.Content.Load<Texture2D>(@"textures/boxtexture");
            box.animation = new PlatformAnimation(new Vector3(20, 0, 185), 100, 1,'Z', box);

            sektorMaszyn = new MachineSector(Game.Content.Load<Model>(@"models\pokoj1-24"), Matrix.Identity);
            sektorMaszyn.texture = Game.Content.Load<Texture2D>(@"textures/texture01-12");
            sektorMaszyn.boundingBox = true;

            avatar = new Filodendron(Game.Content.Load<Model>(@"models\test7_textures1"), Matrix.Identity);//Matrix.CreateTranslation(2500, 2200, 3500));
            avatar.skinningData = avatar.model.Tag as SkinningData;
            avatar.boundingBox = true;
            avatar.bullet = new Bullet(Game.Content.Load<Model>(@"models\box"), Matrix.Identity);
            avatar.slave = new Follower(Game.Content.Load<Model>(@"models\box"), Matrix.Identity);
            avatar.slave.master = avatar;
            avatar.slave.followerPosition = avatar.avatarPosition;

            allModels.Add(box);
            allModels.Add(sektorMaszyn);
            allModels.Add(avatar.bullet);

            if (avatar.skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            avatar.animationPlayer = new AnimationPlayer(avatar.skinningData);
            avatar.clip = avatar.skinningData.AnimationClips["Take 001"];
            avatar.avatarTexture = Game.Content.Load<Texture2D>(@"textures/texture_roslina");
            avatar.CustomShader = Game.Content.Load<Effect>(@"effects/lightening");
            avatar.animationPlayer.StartClip(avatar.clip);
            avatar.avatarPosition = new Vector3(-4000, 40, 4000);
            Vector3 slaveShift =  new Vector3(200, 3, 200);
            avatar.slave.followerPosition = avatar.avatarPosition - slaveShift;
            avatar.bullet.rb = new BulletRigidBody(avatar.bullet);
            // Load explosion textures and effect  
            explosionTexture = Game.Content.Load<Texture2D>(@"Textures\Particle");
            explosionColorsTexture = Game.Content.Load<Texture2D>(@"Textures\ParticleColors");
            explosionEffect = Game.Content.Load<Effect>(@"effects\particle");

            // Set effect parameters that don't change per particle  
            explosionEffect.CurrentTechnique = explosionEffect.Techniques["Technique1"];
            explosionEffect.Parameters["theTexture"].SetValue(explosionTexture);

            //ocean
            ocean = new Ocean(Game.Content.Load<Model>(@"models\ocean"), Matrix.Identity);
            ocean.oceanEffect = Game.Content.Load<Effect>(@"effects\OceanShader");
            ocean.diffuseOceanTexture = Game.Content.Load<Texture2D>(@"textures\water");
            ocean.normalOceanTexture = Game.Content.Load<Texture2D>(@"textures\wavesbump");
            ocean.SetupOceanShaderParameters();

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            if (((Game1)Game).currentGameState == FilodendronGame.Game1.GameState.Playing)
            {
                avatar.Update(gameTime);
                avatar.bullet.Update(gameTime);
                //avatar.bullet.rb.UpdateRigidBody(gameTime);
                avatar.slave.Update(gameTime);
                sektorMaszyn.Update(gameTime);
                
                foreach (CollectableItem item in collectableItems)
                {
                    item.Update(gameTime);
                }

                foreach (BasicModel item in platforms)
                {
                    item.Update(gameTime);
                }

                foreach (BasicModel item in blades)
                {
                    item.Update(gameTime);
                }

                if (box != null)
                {
                    box.Update(gameTime);
                }
                ocean.Update(gameTime);
                //if the side boundry of screen reached, set the mouse on the other side
                if (Mouse.GetState().X >= Game.Window.ClientBounds.Width -2)
                {
                    Mouse.SetPosition(1, Mouse.GetState().Y);// jak wstawia³em 0 to mi ekran przeskakiwa³ lol
                    avatar.prevMouseState = Mouse.GetState();
                }
                if (Mouse.GetState().X <= 0)
                {
                    Mouse.SetPosition(Game.Window.ClientBounds.Width, Mouse.GetState().Y);
                    avatar.prevMouseState = Mouse.GetState();
                }

                // Collision checkout
                foreach (CollectableItem item in collectableItems)
                {
                    if (avatar.rigidBody.CollidesWith(item.model, item.World))
                    {
                        // Collision! add an explosion. 
                        addNewExplosion(item.Position);
                        // delete the box
                        item.isCollected = true;
                        ((Game1)Game).numberOfCoins++;
                    }
                }
                collectableItems.RemoveAll(item => item.isCollected);
                /////////////////////////////// ten box bedzie do wywalenia na koncu bo byl tylko do testów
                if (box != null)
                {
                    if (avatar.rigidBody.CollidesWith(box.model, box.World))
                    {
                        // Collision! add an explosion. 
                        addNewExplosion(box.World);
                        // delete the box
                        box = null;
                        ((Game1)Game).numberOfCoins++;
                        avatar.Die(Game);
                    }

                }

                for (int i = 0; i < platforms.Count; i++)
                {
                    if (avatar.rigidBody.CollidesWith(platforms[i].model, platforms[i].World))
                    {
                        if (platforms[i].animation.isTrap)
                        {
                            platforms[i].animation.currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (platforms[i].animation.currentTime >= 2)
                            {
                                addNewExplosion(platforms[i].World);
                                platforms[i].boundingBoxes = null;
                                platforms.RemoveAt(i);
                                --i;
                                platforms[i].animation.currentTime = 0;
                            }
                        }
                        else
                        {
                            avatar.UpdatePosition(platforms[i].animation.avatarPositionChange);
                        }
                    }
                }

                foreach (BasicModel blade in blades)
                {
                    if (avatar.rigidBody.CollidesWith(blade.model, blade.World))
                    {
                        avatar.Die(Game);
                    }
                }

                // woda zabija, chcesz zeby nie zabijala to zakomentuj ifa ponizej
                if (avatar.avatarPosition.X > -6253 && avatar.avatarPosition.X < 5466 
                    && avatar.avatarPosition.Y > -5 && avatar.avatarPosition.Y < 50 
                    && avatar.avatarPosition.Z > -254 && avatar.avatarPosition.Z < 2468)
                {
                    avatar.Die(Game);
                }

                //dotknij drzwi => wygrana
                System.Diagnostics.Debug.WriteLine(avatar.avatarPosition);
                if (avatar.avatarPosition.X > -6138 && avatar.avatarPosition.X < -6135 
                    && avatar.avatarPosition.Y > 15 && avatar.avatarPosition.Y < 50 
                    && avatar.avatarPosition.Z > -3409 && avatar.avatarPosition.Z < -2171)
                {
                    System.Diagnostics.Debug.WriteLine("ibdfgukiedbgyueirg" + ((Game1)Game).win);
                    ((Game1)Game).win = true;
                }
                // Update explosions
                UpdateExplosions(gameTime);

                base.Update(gameTime);
            }
        }

        protected void addNewExplosion(Matrix modelWorld)
        {
            explosions.Add(new ParticleExplosion(GraphicsDevice, avatar,
                modelWorld,
                ((Game1)Game).rnd.Next(
                    particleExplosionSettings.minLife,
                    particleExplosionSettings.maxLife),
                ((Game1)Game).rnd.Next(
                    particleExplosionSettings.minRoundTime,
                    particleExplosionSettings.maxRoundTime),
                ((Game1)Game).rnd.Next(
                    particleExplosionSettings.minParticlesPerRound,
                    particleExplosionSettings.maxParticlesPerRound),
                ((Game1)Game).rnd.Next(
                    particleExplosionSettings.minParticles,
                    particleExplosionSettings.maxParticles),
                    explosionColorsTexture, particleSettings, explosionEffect));
        }

        protected void UpdateExplosions(GameTime gameTime)
        {
            // Loop through and update explosions  
            for (int i = 0; i < explosions.Count; ++i)
            {
                explosions[i].Update(gameTime);
                // If explosion is finished, remove it   
                if (explosions[i].IsDead)
                {
                    explosions.RemoveAt(i);
                    --i;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (((Game1)Game).currentGameState == FilodendronGame.Game1.GameState.Playing)
            {
                if (avatar.isModelVisible)
                {
                    avatar.Draw(avatar.model, avatar.World, avatar.avatarTexture, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);   
                }
                ocean.Draw(ocean.model, ocean.World, ocean.diffuseOceanTexture, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);
                sektorMaszyn.Draw(sektorMaszyn.model, sektorMaszyn.World, sektorMaszyn.texture, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);
                if (!avatar.bullet.hit)
                {
                    avatar.bullet.Draw(avatar.bullet.model, avatar.bullet.World, null, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);    
                }
                
                avatar.slave.Draw(avatar.slave.model, avatar.slave.World, null, ((Game1)Game).camera, gameTime, ((Game1)Game).graphics);
                
                // Loop through and draw each particle explosion
                foreach (ParticleExplosion pe in explosions)
                {
                    pe.Draw(((Game1)Game).camera);
                }

                // for boxes
                if (box != null)
                {
                    box.Draw(box.model,
                             box.World,
                             boxTexture, 
                             ((Game1)Game).camera, 
                             gameTime, 
                             ((Game1)Game).graphics);
                }

                foreach (BasicModel item in platforms)
                {
                    item.Draw(item.model,
                              item.World,
                              boxTexture, 
                              ((Game1)Game).camera, 
                              gameTime, 
                              ((Game1)Game).graphics);   
                }

                foreach (CollectableItem item in collectableItems)
                {
                    if (!item.isCollected)
                    {
                        item.Draw(item.model,
                                  item.World,
                                  boxTexture, 
                                  ((Game1)Game).camera, gameTime, 
                                  ((Game1)Game).graphics); 
                    }
                }

                foreach (BasicModel blade in blades)
                {
                    blade.Draw(blade.model,
                              blade.World,
                              boxTexture,
                              ((Game1)Game).camera,
                              gameTime,
                              ((Game1)Game).graphics);
                }

                base.Draw(gameTime);
            }
        }

        private void setCollectableItems()
        {
            collectableItems.Add(new CollectableItem(Game.Content.Load<Model>(@"models\box"), Matrix.CreateTranslation(-3000, 30, 5000)));
            collectableItems.Add(new CollectableItem(Game.Content.Load<Model>(@"models\box"), Matrix.CreateTranslation(-3000, 30, 5500)));
            collectableItems.Add(new CollectableItem(Game.Content.Load<Model>(@"models\box"), Matrix.CreateTranslation(-2000, 30, 5500)));
            collectableItems.Add(new CollectableItem(Game.Content.Load<Model>(@"models\box"), Matrix.CreateTranslation(-2000, 30, 5000)));
            collectableItems.Add(new CollectableItem(Game.Content.Load<Model>(@"models\box"), Matrix.CreateTranslation(-2000, 750, 4500)));//na bialej skrzyni
            collectableItems.Add(new CollectableItem(Game.Content.Load<Model>(@"models\box"), Matrix.CreateTranslation(1000, 1550, 3600)));//na brazowej skrzyni
            collectableItems.Add(new CollectableItem(Game.Content.Load<Model>(@"models\box"), Matrix.CreateTranslation(3500, 2000, 3750)));//na fioletowym podescie 1
            collectableItems.Add(new CollectableItem(Game.Content.Load<Model>(@"models\box"), Matrix.CreateTranslation(5000, 2130, 3650)));//na fioletowym podescie 2
            collectableItems.Add(new CollectableItem(Game.Content.Load<Model>(@"models\box"), Matrix.CreateTranslation(5000, 2130, -1550)));//na fioletowym podescie 3

            foreach (CollectableItem item in collectableItems)
            {
                item.animation = new CollectableItemsRotation(0.05f, item.World);
            }
        }

        private void setBlades() 
        {
            addNewBlade(new Vector3(4000, 800, -1000), 5, -1700, 2, 'Z');
            addNewBlade(new Vector3(3500, 800, -1000), 5, -1700, 2, 'Z');
            addNewBlade(new Vector3(3000, 800, -1000), 5, -1700, 2, 'Z');
            addNewBlade(new Vector3(2500, 800, -1000), 5, -1700, 2, 'Z');
            addNewBlade(new Vector3(2000, 800, -1000), 5, -1700, 2, 'Z');
        }

        private void addNewBlade(Vector3 startPosition, float size, float distance, float speed, char direction)
        {
            BasicModel ToAdd = new BasicModel(Game.Content.Load<Model>(@"models\box"), startPosition, size);
            ToAdd.animation = new BladeAnimation(startPosition, distance, speed, direction);
            blades.Add(ToAdd);
        }

        private void setPlatforms()
        {
            //addNewPlatform(new Vector3(-1200, 30, 2355), 6, 300, 5, 'Z',false);
            addNewPlatform(new Vector3(-1800, 30, 5000), 3, 800, 2, 'Y',false);//pod srebrnym boxem
            addNewPlatform(new Vector3(4700, 2130, 800), 6, 3650, 6, 'Z',false);//z fioletowej polki
            addNewPlatform(new Vector3(4700, 2200, -1400), 6, 2000, 6, 'Z', false);//z fioletowej polki (2)
            addNewPlatform(new Vector3(-1400, 750, 3800), 6, 1600, 4, 'Y', false);//na srebrnym boxie
            addNewPlatform(new Vector3(-1200, 1700, 3800), 6, 800, 6, 'X', false);//nad srebrnym boxem do brazowego boxa
            addNewPlatform(new Vector3(2650, 1700, 3800), 6, 1701, 6, 'Y', true);//miedzy brazowym boxem a fioletowym


            // most na drugi przeb pod sciana
            addNewPlatform(new Vector3(4700, 30, 2300), 6, 2350, 6, 'Z', true);
            addNewPlatform(new Vector3(4700, 30, 1800), 6, 1850, 6, 'Z', true);
            addNewPlatform(new Vector3(4700, 30, 1300), 6, 1350, 6, 'Z', true);
            addNewPlatform(new Vector3(4700, 30, 800), 6, 850, 6, 'Z', true);
            addNewPlatform(new Vector3(4700, 30, 300), 6, 350, 6, 'Z', true);

            foreach (BasicModel item in platforms)
            {
                item.boundingBox = true;
                allModels.Add(item);
            }
        }

        private void addNewPlatform(Vector3 startPosition, float size, float distance, float speed, char direction, bool trap)
        {
            BasicModel ToAdd;

            ToAdd = new BasicModel(Game.Content.Load<Model>(@"models\box"), startPosition, size);
            ToAdd.animation = new PlatformAnimation(startPosition, distance, speed, direction, ToAdd);
            ToAdd.animation.isTrap = trap;
            platforms.Add(ToAdd);
        }
    }
}