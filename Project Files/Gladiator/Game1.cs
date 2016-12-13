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
using System.IO; 
using System.Xml.Serialization;

namespace Maybe_You_will_finish_this_one
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	/// 
	public class Game1 : Game
	{
		[Serializable]
		public struct SaveData
		{
			[XmlElement("soundLevel")]
			public float soundLevel;
		}
		private delegate void gameStateDelegate(GameTime gameTime);
		private gameStateDelegate UpdateApp, DrawApp;
		private static Game1 game;
		public const int GAME_WIDTH = 900;
		public const int GAME_HEIGHT = 900;
		private static int killCount = 0;
		private Texture2D whiteTexture;
		private Button restartButton, mainMenuButton;
		public static bool Focused
		{
			get { return game.IsActive; }
		}
		public static Rectangle Arena_Bounds
		{
			get;
			private set;
		}
		private bool blackFadeDone = false, textFadeDone = false;
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private static Player player;
		private Texture2D arenaTexture;
		private static List<Projectile> projectiles, projectileRemoveList;
		private static List<Monster> monsters, monsterRemoveList;
		private static List<MeleeAttack> meleeAttacks;
		private static List<Powerup> powerups, powerupRemoveList;
		private Spawner spawner;
		private SpriteFont gameFont;
		private Texture2D hearts;
		private const int DEATH_FADE_TIME_MS = 3000, GAME_OVER_TEXT_FADE_TIME_MS = 2000, TIME_BETWEEN_WAVES_MS = 500;
		private float fadeTimer, fadeRatio, generalTimer;
		private SpriteFont gameOverFont, killCountFont, gameOverButtonFont, mainMenuButtonFont, mainMenuTitleFont, pauseMenuTitleFont, pauseMenuButtonFont, mainMenuSliderFont, tutorialOkayButtonFont;
		private Button mainMenuPlayButton, mainMenuExitButton, pResumeButton, pMainMenuButton, pExitButton, tutorialOkayButton;
		private Vector2 mainMenuTitleLoc, pauseMenuTitleLoc, menuSoundSliderLoc, pauseMenuSliderLoc;
		private SoundEffect endGameSound, endGameRain;
		private SoundEffectInstance endRainLoop;
		private static Weapon shotgun;
		private KeyboardState oldKb, currKb;
		private Slider mainMenuSoundSlider, pauseMenuSoundSlider;
		private bool waitWave, firstTime;
		private Texture2D tutPicture;
		private readonly string saveFileName = "saveData.sav";
		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = GAME_WIDTH;
			graphics.PreferredBackBufferHeight = GAME_HEIGHT;
			Content.RootDirectory = "Content";
			fadeTimer = 0;
			fadeRatio = 0;
			this.IsMouseVisible = true;
			currKb = Keyboard.GetState();
			waitWave = false;
			game = this;
			firstTime = true;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			UpdateApp = UpdateMainMenu;
			DrawApp = DrawMainMenu;
			if (File.Exists(saveFileName))
			{
				FileStream stream = File.Open(saveFileName, FileMode.OpenOrCreate,
					FileAccess.Read);
				try
				{

					XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
					SaveData data = (SaveData)serializer.Deserialize(stream);
					SoundEffect.MasterVolume = data.soundLevel;
				}
				finally
				{
					stream.Close();
				}
			}
			else
			{
				SoundEffect.MasterVolume = 1;
			}
			base.Initialize();
	
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			gameFont = Content.Load<SpriteFont>("gameFont");
			whiteTexture = Content.Load<Texture2D>("white");
			Powerup.Powerup_Sprites = Content.Load<Texture2D>("powerups");
			hearts = Content.Load<Texture2D>("heart");
			arenaTexture = Content.Load<Texture2D>("arena_background");
			gameOverFont = Content.Load<SpriteFont>("GameOverFont");
			killCountFont = Content.Load<SpriteFont>("KillCountFont");
			gameOverButtonFont = Content.Load<SpriteFont>("GameOverButtonFont");
			mainMenuButtonFont = Content.Load<SpriteFont>("MainMenuButtonFont");
			mainMenuTitleFont = Content.Load<SpriteFont>("MainMenuTitleFont");
			pauseMenuTitleFont = Content.Load<SpriteFont>("PauseMenuFont");
			pauseMenuButtonFont = Content.Load<SpriteFont>("PauseMenuButtonFont");
			endGameSound = Content.Load<SoundEffect>("endsound");
			endGameRain = Content.Load<SoundEffect>("rain");
			mainMenuSliderFont = Content.Load<SpriteFont>("MainMenuSliderFont");
			tutorialOkayButtonFont = Content.Load<SpriteFont>("TutorialOkayButtonFont");
			tutPicture = Content.Load<Texture2D>("tutpic");
			endRainLoop = endGameRain.CreateInstance();
			mainMenuButton = new Button(85, 700, 300, 100, "Main Menu", gameOverButtonFont, Color.Black, whiteTexture, Color.Gray);
			mainMenuButton.ClickSound = Content.Load<SoundEffect>("click");
			restartButton = new Button(500, 700, 300, 100, "Restart Game", gameOverButtonFont, Color.Black, whiteTexture, Color.Gray);
			restartButton.ClickSound = Content.Load<SoundEffect>("click");
			mainMenuButton.ButtonAlpha = 0;
			restartButton.ButtonAlpha = 0;
			mainMenuPlayButton = new Button(237, 350, 425, 200, "Play Game", mainMenuButtonFont, Color.Black, whiteTexture, Color.Gray);
			mainMenuPlayButton.ClickSound = Content.Load<SoundEffect>("click");
			mainMenuExitButton = new Button(237, 625, 425, 200, "Exit Game", mainMenuButtonFont, Color.Black, whiteTexture, Color.Gray);
			mainMenuExitButton.ClickSound = Content.Load<SoundEffect>("click");
			pResumeButton = new Button(300, 375, 300, 125, "Resume", pauseMenuButtonFont, Color.Black, whiteTexture, Color.Gray);
			pResumeButton.ClickSound = Content.Load<SoundEffect>("click");
			pMainMenuButton = new Button(300, 525, 300, 125, "Main Menu", pauseMenuButtonFont, Color.Black, whiteTexture, Color.Gray);
			pMainMenuButton.ClickSound = Content.Load<SoundEffect>("click");
			pExitButton = new Button(300, 675, 300, 125, "Exit", pauseMenuButtonFont, Color.Black, whiteTexture, Color.Gray);
			pExitButton.ClickSound = Content.Load<SoundEffect>("click");
			mainMenuSoundSlider = new Slider(775, 450, 40, 20, 10, 300, whiteTexture, Color.Gray, Orientation.Vertical, SoundEffect.MasterVolume);
			mainMenuSoundSlider.SliderReleased += ChangeSoundLevel;
			mainMenuSoundSlider.ReleaseSound = Content.Load<SoundEffect>("click");
			pauseMenuSoundSlider = new Slider(750, 450, 40, 20, 10, 300, whiteTexture, Color.Gray, Orientation.Vertical, SoundEffect.MasterVolume);
			pauseMenuSoundSlider.SliderReleased += ChangeSoundLevel;
			pauseMenuSoundSlider.ReleaseSound = Content.Load<SoundEffect>("click");
			restartButton.ButtonClicked += RestartGame;
			mainMenuButton.ButtonClicked += TriggerMainMenu;
			mainMenuPlayButton.ButtonClicked += RestartGame;
			mainMenuExitButton.ButtonClicked += () => this.QuitGame();
			pExitButton.ButtonClicked += () => this.QuitGame();
			pMainMenuButton.ButtonClicked += TriggerMainMenu;
			pResumeButton.ButtonClicked += ResumeGame;
			mainMenuTitleLoc = (-mainMenuTitleFont.MeasureString("Gladiator") + new Vector2(graphics.PreferredBackBufferWidth, 400)) / 2;
			pauseMenuTitleLoc = (-pauseMenuTitleFont.MeasureString("Paused") + new Vector2(graphics.PreferredBackBufferWidth, 400)) / 2;
			endRainLoop.IsLooped = true;
			endRainLoop.Play();
			endRainLoop.Volume = .6f;
			Vector2 dim = mainMenuSliderFont.MeasureString("Volume");
			menuSoundSliderLoc = new Vector2(mainMenuSoundSlider.X - dim.X / 2 + mainMenuSoundSlider.Width / 2, mainMenuSoundSlider.Y - dim.Y * 1.1f);
			dim = pauseMenuButtonFont.MeasureString("Volume");
			pauseMenuSliderLoc = new Vector2(pauseMenuSoundSlider.X - dim.X / 2 + pauseMenuSoundSlider.Width / 2, pauseMenuSoundSlider.Y - dim.Y * 1.1f);
			tutorialOkayButton = new Button(300, 700, 300, 100, "Got it!", tutorialOkayButtonFont, Color.Black, whiteTexture, Color.Gray);
			tutorialOkayButton.ClickSound = Content.Load<SoundEffect>("click");
			tutorialOkayButton.ButtonClicked += () =>
			{
				UpdateApp = UpdateGame;
				DrawApp = DrawGame;
				this.IsMouseVisible = false;
			};
			//RangedStats skelerange = new RangedStats(SkeleWeapon.BASE_R_KNOCK_BACK_AMOUNT, SkeleWeapon.BASE_R_KNOCK_BACK_SPEED, SkeleWeapon.BASE_R_RELOAD_TIME_MS, SkeleWeapon.BASE_R_RANGE, SkeleWeapon.BASE_BULLET_SPEED, SkeleWeapon.BASE_BULLET_WIDTH, SkeleWeapon.BASE_BULLET_HEIGHT);
			//Skelebro skeleBro = new Skelebro(400, 400, player, Content.Load<Texture2D>("skeleton"));
			//skeleBro.Weapon = new SkeleWeapon(skeleBro, skelerange, null, Content.Load<Texture2D>("white"), Content.Load<Texture2D>("white"));
			//monsters.Add(skeleBro);
			//MeleeStats meleeStats = new MeleeStats(ZombieWeapon.BASE_M_KNOCK_BACK_AMOUNT, ZombieWeapon.BASE_M_KNOCK_BACK_SPEED, ZombieWeapon.BASE_M_RELOAD_TIME_MS, ZombieWeapon.BASE_M_RANGE);
			//Zombie debug_Zombie = new Zombie(100, 100, player, Content.Load<Texture2D>("white"));
			//debug_Zombie.Weapon = new ZombieWeapon(debug_Zombie, null, meleeStats, Content.Load<Texture2D>("white"), Content.Load<Texture2D>("white"));
			//mobs.Add(debug_Zombie);
			//Zombie debug_Zombie2 = new Zombie(300, 100, player, Content.Load<Texture2D>("white"));
			//debug_Zombie2.Weapon = new ZombieWeapon(debug_Zombie, null, meleeStats, Content.Load<Texture2D>("white"), Content.Load<Texture2D>("white"));
			//mobs.Add(debug_Zombie2);
			//Zombie debug_Zombie3 = new Zombie(300, 300, player, Content.Load<Texture2D>("white"));
			//debug_Zombie3.Weapon = new ZombieWeapon(debug_Zombie, null, meleeStats, Content.Load<Texture2D>("white"), Content.Load<Texture2D>("white"));
			//mobs.Add(debug_Zombie3);
		}

		public void EndGame()
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			endGameSound.Play();
			endRainLoop.Volume = 0;
			endRainLoop.Play();
			fadeTimer = 0;
			fadeRatio = 0;
			restartButton.ButtonAlpha = 0;
			mainMenuButton.ButtonAlpha = 0;
			textFadeDone = false;
			blackFadeDone = false;
			DrawApp = DrawGameOver;
			UpdateApp = UpdateGameOver;
			
		}

		private void TriggerMainMenu()
		{
			endRainLoop.Play();
			UpdateApp = UpdateMainMenu;
			DrawApp = DrawMainMenu;
		}

		private void RestartGame()
		{
			endRainLoop.Stop();
			this.IsMouseVisible = false;
			fadeTimer = 0;
			fadeRatio = 0;
			KeyControls controls = new KeyControls(Keys.W, Keys.S, Keys.A, Keys.D, Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.F);
			player = new Player(400, 400, controls, Content.Load<Texture2D>("player"), Content.Load<SoundEffect>("grunt"), this);
			RangedStats rangedStats = BasicWeapon.BASE_RANGED_STATS;
			MeleeStats meleeStats = new MeleeStats(ZombieWeapon.BASE_M_KNOCK_BACK_AMOUNT, ZombieWeapon.BASE_M_KNOCK_BACK_SPEED, ZombieWeapon.BASE_M_RELOAD_TIME_MS, ZombieWeapon.BASE_M_RANGE + 25);
			player.Weapon = new BasicWeapon(player, rangedStats, meleeStats, Content.Load<Texture2D>("bullet"), whiteTexture, Content.Load<SoundEffect>("BasicGunshot"));
			//player.Weapon = new Shotgun(player, rangedStats, meleeStats, Content.Load<Texture2D>("bullet"), whiteTexture, Content.Load<SoundEffect>("BasicGunshot"));
			shotgun = new Shotgun(player, Shotgun.BASE_RANGED_STATS, null, Content.Load<Texture2D>("bullet"), whiteTexture, Content.Load<SoundEffect>("shotgun"));
			Arena_Bounds = new Rectangle(50, 50, 800, 800);
			projectiles = new List<Projectile>();
			monsters = new List<Monster>();
			projectileRemoveList = new List<Projectile>();
			monsterRemoveList = new List<Monster>();
			meleeAttacks = new List<MeleeAttack>();
			powerups = new List<Powerup>();
			powerupRemoveList = new List<Powerup>();
			spawner = new Spawner(player, Content.ServiceProvider);
			spawner.AddSpawners(new Vector2(Arena_Bounds.X + Arena_Bounds.Width / 4, Arena_Bounds.Y), new Vector2(Arena_Bounds.X + (3 * Arena_Bounds.Width) / 4, Arena_Bounds.Y),
								new Vector2(Arena_Bounds.X + Arena_Bounds.Width / 4, Arena_Bounds.Y + Arena_Bounds.Height), new Vector2(Arena_Bounds.X + (3 * Arena_Bounds.Width) / 4, Arena_Bounds.Y + Arena_Bounds.Height)
			);
			if (firstTime)
			{
				UpdateApp = UpdateTutorial;
				DrawApp = DrawTutorial;
				IsMouseVisible = true;
				firstTime = false;
			}
			else
			{
				UpdateApp = UpdateGame;
				DrawApp = DrawGame;
				spawner.StartWave();
			}
		}

		private void PauseGame()
		{
			this.IsMouseVisible = true;
			Mouse.SetPosition(450, 450);
			UpdateApp = UpdatePauseMenu;
			DrawApp = DrawPauseMenu;
		}

		private void ResumeGame()
		{
			this.IsMouseVisible = false;
			UpdateApp = UpdateGame;
			DrawApp = DrawGame;
		}

		public void QuitGame()
		{
				
			FileStream stream = File.Open(saveFileName, FileMode.Create);
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
				SaveData sav = new SaveData();
				sav.soundLevel = SoundEffect.MasterVolume;
				serializer.Serialize(stream, sav);
			}
			finally
			{
				stream.Close();
			}
			this.Exit();
}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			Content.Unload();
			Content.Dispose();
			if(spawner != null)
			{
				spawner.Dispose();
			}
			base.UnloadContent();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			UpdateApp(gameTime);
			base.Update(gameTime);
		}
		private void UpdateGame(GameTime gameTime)
		{
			if (!IsActive)
			{
				PauseGame();
				return;
			}
			oldKb = currKb;
			currKb = Keyboard.GetState();
			if (currKb.IsKeyDown(Keys.Escape) && !oldKb.IsKeyDown(Keys.Escape))
			{
				PauseGame();
				return;
			}
			player.Update(gameTime);
			spawner.Update(gameTime);
			foreach (Powerup p in powerups)
			{
				p.Update(gameTime);
				if (p.isSpent)
				{
					powerupRemoveList.Add(p);
				}
			}
			foreach (Monster m in monsters)
			{
				m.Update(gameTime);
			}
			foreach (Projectile p in projectiles)
			{
				p.Update(gameTime);
				if (!player.IsHit && !player.Equals(p.Owner) && p.ColidesWith(player.Bounds) && !p.IsInactive)
				{
					player.Hit(p.Dir, p.rangedStats.KnockBackAmount, p.rangedStats.KnockBackSpeed);
					RemoveProjectile(p);
				}
				foreach (Monster m in monsters)
				{
					if (p.Owner.Equals(player) && !m.IsHit && !p.Owner.Equals(m) && p.ColidesWith(m.Bounds) && !p.IsInactive)
					{
						m.Hit(p.Dir, p.rangedStats.KnockBackAmount, p.rangedStats.KnockBackSpeed);
						RemoveProjectile(p);
					}
				}
			}
			foreach (MeleeAttack melee in meleeAttacks)
			{
				if (!player.IsHit && !melee.Owner.Equals(player) && melee.ColidesWith(player.Bounds))
				{
					player.Hit(melee.Dir, melee.meleeStats.KnockBackAmount, melee.meleeStats.KnockBackSpeed);
				}
				/*foreach (Monster m in monsters)
				{
					if (!m.IsHit && !melee.Owner.Equals(m) && melee.ColidesWith(m.Bounds))
					{
						m.Hit(melee.Dir, melee.meleeStats.KnockBackAmount, melee.meleeStats.KnockBackSpeed);
					}
				}*/
			}
			foreach (Projectile p in projectileRemoveList)
			{
				projectiles.Remove(p);
			}
			foreach (Monster m in monsterRemoveList)
			{
				monsters.Remove(m);
			}
			foreach (Powerup p in powerupRemoveList)
			{
				powerups.Remove(p);
			}
			projectileRemoveList.Clear();
			monsterRemoveList.Clear();
			meleeAttacks.Clear();
			powerupRemoveList.Clear();
			generalTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			if (monsters.Count == 0)
			{
				if (!waitWave)
				{
					generalTimer = 0;
					waitWave = true;
				}
				if(waitWave && generalTimer >= TIME_BETWEEN_WAVES_MS)
				{
					spawner.StartWave();
					waitWave = false;
				}
			}
			
		}
		private void UpdateGameOver(GameTime gameTime)
		{
			if (fadeTimer < DEATH_FADE_TIME_MS && !blackFadeDone)
			{
				fadeTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				fadeRatio = fadeTimer / DEATH_FADE_TIME_MS;
			}
			else if (fadeTimer >= DEATH_FADE_TIME_MS && !blackFadeDone)
			{
				fadeTimer = 0;
				fadeRatio = 0;
				blackFadeDone = true;
			}
			else if (fadeTimer < GAME_OVER_TEXT_FADE_TIME_MS && blackFadeDone && !textFadeDone)
			{
				fadeTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				fadeRatio = fadeTimer / GAME_OVER_TEXT_FADE_TIME_MS;
				restartButton.ButtonAlpha = fadeRatio;
				mainMenuButton.ButtonAlpha = fadeRatio;
				endRainLoop.Volume = fadeRatio * .6f;
			}
			else if(fadeTimer >= GAME_OVER_TEXT_FADE_TIME_MS && !textFadeDone)
			{
				fadeTimer = 0;
				fadeRatio = 0;
				textFadeDone = true;
				restartButton.ButtonAlpha = 1;
				mainMenuButton.ButtonAlpha = 1;
				Mouse.SetPosition(restartButton.X + restartButton.Width / 2, restartButton.Y + restartButton.Height / 2);
				endRainLoop.Volume = .6f;
				this.IsMouseVisible = true;
			}
			if (!this.IsActive)
				return;
			restartButton.Update(gameTime);
			mainMenuButton.Update(gameTime);
		}
		private void UpdateMainMenu(GameTime gameTime)
		{
			if (!this.IsActive)
				return;
			mainMenuPlayButton.Update(gameTime);
			mainMenuExitButton.Update(gameTime);
			mainMenuSoundSlider.Update(gameTime);
		}
		private void UpdatePauseMenu(GameTime gameTime)
		{
			if (!this.IsActive)
				return;
			oldKb = currKb;
			currKb = Keyboard.GetState();
			if (currKb.IsKeyDown(Keys.Escape) && !oldKb.IsKeyDown(Keys.Escape))
			{
				ResumeGame();
				return;
			}
			pResumeButton.Update(gameTime);
			pExitButton.Update(gameTime);
			pMainMenuButton.Update(gameTime);
			pauseMenuSoundSlider.Update(gameTime);
		}
		private void UpdateTutorial(GameTime gameTime)
		{
			tutorialOkayButton.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			DrawApp(gameTime);
			base.Draw(gameTime);
		}
		private void DrawGame(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin();
			spriteBatch.Draw(arenaTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
			player.Draw(spriteBatch);
			foreach (Powerup p in powerups)
			{
				p.Draw(spriteBatch);
			}
			foreach (Projectile p in projectiles)
			{
				p.Draw(spriteBatch);
			}
			foreach (Monster m in monsters)
			{
				m.Draw(spriteBatch);
			}
			spriteBatch.DrawString(gameFont, "Wave: " + spawner.wave, new Vector2(675, -5), Color.Red);
			spriteBatch.Draw(hearts, new Rectangle(60, 5, 39, 39), Color.White);
			spriteBatch.DrawString(gameFont, "" + player.health, new Vector2(110, -5), Color.Red);
			if (player.activePowerup != null)
			{
				spriteBatch.Draw(Powerup.Powerup_Sprites, new Rectangle(530, 5, 40, 40), player.activePowerup.SourceRectangle, Color.White);
				spriteBatch.DrawString(gameFont, "Powerup: ", new Vector2(310, -5), Color.Red);
			}
			spriteBatch.End();
			// TODO: Add your drawing code here
		}
		private void DrawGameOver(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin();
			if (!blackFadeDone)
			{
				spriteBatch.Draw(arenaTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
				player.Draw(spriteBatch);
				foreach (Powerup p in powerups)
				{
					p.Draw(spriteBatch);
				}
				foreach (Projectile p in projectiles)
				{
					p.Draw(spriteBatch);
				}
				foreach (Monster m in monsters)
				{
					m.Draw(spriteBatch);
				}
				spriteBatch.DrawString(gameFont, "Wave: " + spawner.wave, new Vector2(675, -5), Color.Red);
				spriteBatch.Draw(hearts, new Rectangle(60, 5, 39, 39), Color.White);
				spriteBatch.DrawString(gameFont, "" + player.health, new Vector2(110, -5), Color.Red);
				if (player.activePowerup != null)
				{
					spriteBatch.Draw(Powerup.Powerup_Sprites, new Rectangle(530, 5, 40, 40), player.activePowerup.SourceRectangle, Color.White);
					spriteBatch.DrawString(gameFont, "Powerup: ", new Vector2(310, -5), Color.Red);
				}
				spriteBatch.Draw(whiteTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
					 Color.Black * fadeRatio);
			}
			else
			{
				spriteBatch.Draw(whiteTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
				 Color.Black);
				Vector2 youDiedDim = gameOverFont.MeasureString("You Died");
				Vector2 youDiedLoc = new Vector2((graphics.PreferredBackBufferWidth - youDiedDim.X) / 2, (graphics.PreferredBackBufferHeight - youDiedDim.Y / 2) / 8);
				Vector2 oDim = gameOverFont.MeasureString("Game Over");
				Vector2 gameOverLoc = new Vector2((graphics.PreferredBackBufferWidth  - oDim.X) / 2, youDiedLoc.Y + youDiedDim.Y - 50);
				oDim = killCountFont.MeasureString("Monsters Killed: " + killCount);
				Vector2 killCountLoc = new Vector2((graphics.PreferredBackBufferWidth - oDim.X) / 2, gameOverLoc.Y + youDiedDim.Y - 5);
				if (!textFadeDone)
				{
					spriteBatch.DrawString(gameOverFont, "You Died", youDiedLoc, Color.Red * fadeRatio);
					spriteBatch.DrawString(gameOverFont, "Game Over", gameOverLoc, Color.Red * fadeRatio);
					spriteBatch.DrawString(killCountFont, "Monsters Killed: " + killCount, killCountLoc, Color.Red * fadeRatio);
				}
				else
				{
					spriteBatch.DrawString(gameOverFont, "You Died", youDiedLoc, Color.Red);
					spriteBatch.DrawString(gameOverFont, "Game Over", gameOverLoc, Color.Red);
					spriteBatch.DrawString(killCountFont, "Monsters Killed: " + killCount, killCountLoc, Color.Red);
				}
				restartButton.Draw(spriteBatch);
				mainMenuButton.Draw(spriteBatch);
			}
			


			spriteBatch.End();
		}
		private void DrawMainMenu(GameTime gameTime)
		{
			spriteBatch.Begin();
			GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Draw(whiteTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.Tan);
			spriteBatch.DrawString(mainMenuTitleFont, "Gladiator", mainMenuTitleLoc, Color.Red);
			spriteBatch.DrawString(mainMenuSliderFont, "Volume", menuSoundSliderLoc, Color.Red);
			mainMenuPlayButton.Draw(spriteBatch);
			mainMenuExitButton.Draw(spriteBatch);
			mainMenuSoundSlider.Draw(spriteBatch);
			spriteBatch.End();
		}
		private void DrawPauseMenu(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin();
			spriteBatch.Draw(arenaTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
			player.Draw(spriteBatch);
			foreach (Projectile p in projectiles)
			{
				p.Draw(spriteBatch);
			}
			foreach (Monster m in monsters)
			{
				m.Draw(spriteBatch);
			}
			foreach (Powerup p in powerups)
			{
				p.Draw(spriteBatch);
			}
			spriteBatch.DrawString(gameFont, "Wave: " + spawner.wave, new Vector2(675, -5), Color.Red);
			spriteBatch.Draw(hearts, new Rectangle(60, 5, 39, 39), Color.White);
			spriteBatch.DrawString(gameFont, "" + player.health, new Vector2(110, -5), Color.Red);
			if (player.activePowerup != null)
			{
				spriteBatch.Draw(Powerup.Powerup_Sprites, new Rectangle(530, 5, 40, 40), player.activePowerup.SourceRectangle, Color.White);
				spriteBatch.DrawString(gameFont, "Powerup: ", new Vector2(310, -5), Color.Red);
			}
			spriteBatch.Draw(whiteTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.Black * .9f);
			spriteBatch.DrawString(pauseMenuTitleFont, "Paused", pauseMenuTitleLoc, Color.White);
			pResumeButton.Draw(spriteBatch);
			pExitButton.Draw(spriteBatch);
			pMainMenuButton.Draw(spriteBatch);
			pauseMenuSoundSlider.Draw(spriteBatch);
			spriteBatch.DrawString(pauseMenuButtonFont, "Volume", pauseMenuSliderLoc, Color.White);
			spriteBatch.End();
		}
		private void DrawTutorial(GameTime gameTime)
		{
			spriteBatch.Begin();
			spriteBatch.Draw(whiteTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.Tan);
			spriteBatch.DrawString(tutorialOkayButtonFont, " Use the WASD keys to move \nand the arrow keys to shoot.", new Vector2(200,200), Color.Black);
			spriteBatch.Draw(tutPicture, new Rectangle(60, 300, 800, 400), Color.White);
			tutorialOkayButton.Draw(spriteBatch);
			spriteBatch.End();
		}
		public static void AddProjectile(Projectile projectile)
		{
			Game1.projectiles.Add(projectile);
		}
		public static void RemoveProjectile(Projectile projectile)
		{
			Game1.projectileRemoveList.Add(projectile);
			projectile.IsInactive = true;
		}
		public static void AddMonster(Monster mob)
		{
			monsters.Add(mob);
		}
		public static void RemoveMonster(Monster mob)
		{
			monsterRemoveList.Add(mob);
			killCount++;
		}
		public static void AddMeleeAttack(MeleeAttack melee)
		{
			meleeAttacks.Add(melee);
		}
		public static Mob IsSpaceOcupied(Mob mob, Vector2 target)
		{
			Vector2 playerMiddle = new Vector2(player.Location.X + player.Width / 2, player.Location.Y + player.Height / 2);
			Vector2 mobMiddle = new Vector2(mob.Location.X + mob.Width / 2, mob.Location.Y + mob.Height / 2);
			if ((playerMiddle - mobMiddle).Length() < player.Width * .6f)
				return player;
			foreach (Monster m in monsters)
			{
				if (m != mob && m.Bounds.Intersects(new Rectangle((int)target.X, (int)target.Y, mob.Width, mob.Height)) && m.CanColide)
					return m;
			}

			return null;
		}
		public static void DropPowerup(Vector2 midLoc, int type)
		{
			switch (type)
			{
				case 1:
					powerups.Add(new Powerup((int)midLoc.X - 25, (int)midLoc.Y - 25, 50, 50, player, PowerupType.FireRateBoost, 200, Color.Blue, 10, null));
					break;
				case 2:
					powerups.Add(new Powerup((int)midLoc.X - 25, (int)midLoc.Y - 25, 50, 50, player, PowerupType.HealthBoost, 3, Color.Blue, 0, null));
					break;
				case 3:
					powerups.Add(new Powerup((int)midLoc.X - 25, (int)midLoc.Y - 25, 50, 50, player, PowerupType.SpeedBoost, 150, Color.Blue, 10, null));
					break;
				case 4:
					powerups.Add(new Powerup((int)midLoc.X - 25, (int)midLoc.Y - 25, 50, 50, player, PowerupType.Shotgun, 0, Color.Blue, 15, shotgun));
					break;
			}
		}
		public static Mob GetClosestMonsterAround(Vector2 loc, int r)
		{
			foreach (Mob m in monsters)
			{
				if (Vector2.Distance(m.Location, loc) <= r)
					return m;
			}
			return null;
		}
		private void ChangeSoundLevel(float level)
		{
			SoundEffect.MasterVolume = level;
			pauseMenuSoundSlider.SliderPercent = level;
			mainMenuSoundSlider.SliderPercent = level;
		}
	}
}
