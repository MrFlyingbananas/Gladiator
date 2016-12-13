using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Maybe_You_will_finish_this_one
{
	public class Player : Mob
	{
		private const int BASE_WIDTH = 36;
		private const int BASE_HEIGHT = 36;
		private const int BASE_HEALTH = 6;
		private const int BASE_SPEED = 200;
		private const int BASE_HIT_DURATION_MS = 1000;
		private const int FIRE_ANIM_DURATION_MS = 200;
		private KeyControls controls;
		private KeyboardState oldKb;
		public Powerup activePowerup;
		private Game1 game;
		public Player(int x, int y, KeyControls controls, Texture2D texture, SoundEffect hitSound, Game1 game) : base(x, y, texture, hitSound)
		{
			this.controls = controls;
			oldKb = Keyboard.GetState();
			bodyDirecton = Direction.Up;
			health = BASE_HEALTH;
			speed = BASE_SPEED;
			Width = BASE_WIDTH;
			Height = BASE_HEIGHT;
			hitDurationMS = BASE_HIT_DURATION_MS;
			bounds = new Rectangle(x, y, Width, Height);
			srcRect = new Rectangle(0, Height, Width, Height);
			this.game = game;
		}

		public override void Update(GameTime gameTime)
		{
			oldAnim = currAnim;
			Vector2 vel = Vector2.Zero;
			KeyboardState currKb = Keyboard.GetState();
			if (!knockBacked)
			{
				//input
				Vector2 dir = Vector2.Zero;
				if (currKb.IsKeyDown(controls.keyUp))
					dir.Y += 1;
				if (currKb.IsKeyDown(controls.keyDown))
					dir.Y -= 1;
				if (currKb.IsKeyDown(controls.keyLeft))
					dir.X -= 1;
				if (currKb.IsKeyDown(controls.keyRight))
					dir.X += 1;
				vel = new Vector2(dir.X, -dir.Y) * speed;				
				if(vel == Vector2.Zero)
					currAnim = AnimationState.Idle;
				else{
					currAnim =AnimationState.Walking;
				}
				if (currKb.IsKeyDown(controls.keyFireUp))
				{
					Weapon.FireProjectile(loc, Vector2.Zero, Direction.Up);
					bodyDirecton = Direction.Up;
					currAnim = AnimationState.Firing;
				}
				else if (currKb.IsKeyDown(controls.keyFireDown))
				{
					Weapon.FireProjectile(loc, Vector2.Zero, Direction.Down);
					bodyDirecton = Direction.Down;
					currAnim = AnimationState.Firing;
				}
				else if (currKb.IsKeyDown(controls.keyFireLeft))
				{
					Weapon.FireProjectile(loc, Vector2.Zero / 2, Direction.Left);
					bodyDirecton = Direction.Left;
					currAnim = AnimationState.Firing;
				}
				else if (currKb.IsKeyDown(controls.keyFireRight))
				{
					Weapon.FireProjectile(loc, Vector2.Zero / 2, Direction.Right);
					bodyDirecton = Direction.Right;
					currAnim = AnimationState.Firing;
				}
				if (currKb.IsKeyDown(controls.melee))
				{
					Mob mob = Game1.GetClosestMonsterAround(this.loc, Weapon.meleeStats.Range);
					if (mob != null)
					{
						//Weapon.FireMelee(loc, dir);
					}
				}

				//staying in arena
				Vector2 tempLoc = loc + vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
				Rectangle arena = Game1.Arena_Bounds;

				if (tempLoc.X < arena.X)
					loc.X = arena.X;
				else if (tempLoc.X + Width > arena.Width + arena.X)
					loc.X = arena.Width + arena.X - Width;
				else
					loc.X = tempLoc.X;
				if (tempLoc.Y < arena.Y)
					loc.Y = arena.Y;
				else if (tempLoc.Y + Height > arena.Height + arena.Y)
					loc.Y = arena.Height + arena.Y - Height;
				else
					loc.Y = tempLoc.Y;
				
			}
			else
			{
				//knockBacked
				Vector2 tempLoc = loc + knockBackDir * (float)gameTime.ElapsedGameTime.TotalSeconds * knockBackSpeed;
				Rectangle arena = Game1.Arena_Bounds;

				if (tempLoc.X < arena.X)
				{
					loc.X = arena.X;
					knockBacked = false;
				}
				else if (tempLoc.X + Width > arena.Width + arena.X)
				{
					loc.X = arena.Width + arena.X - Width;
					knockBacked = false;
				}
				else
					loc.X = tempLoc.X;
				if (tempLoc.Y < arena.Y)
				{
					loc.Y = arena.Y;
					knockBacked = false;
				}
				else if (tempLoc.Y + Height > arena.Height + arena.Y)
				{
					loc.Y = arena.Height + arena.Y - Height;
					knockBacked = false;
				}
				else
					loc.Y = tempLoc.Y;
			}
			oldKb = currKb;
			if (oldAnim != currAnim)
			{
				animTimer = 0;
				switch (currAnim)
				{
					case AnimationState.Firing:
						switch (bodyDirecton)
						{
							case Direction.Up:
								srcRect = new Rectangle(Width * 2, 0, Width, Height);
								break;
							case Direction.Down:
								srcRect = new Rectangle(0, 0, Width, Height);
								break;
							case Direction.Left:
								srcRect = new Rectangle(Width * 3, 0, Width, Height);
								break;
							case Direction.Right:
								srcRect = new Rectangle(Width, 0, Width, Height);
								break;
						}
						break;
					case AnimationState.Walking:
						srcRect = new Rectangle(0, Height, Width, Height);
						break;
					case AnimationState.Idle:
						srcRect = new Rectangle(0, Height, Width, Height);
						break;
				}
			}
			base.Update(gameTime);
			if(currAnim == AnimationState.Firing && animTimer >= FIRE_ANIM_DURATION_MS)
				if (vel == Vector2.Zero)
					currAnim = AnimationState.Idle;
				else
				{
					currAnim = AnimationState.Walking;
				}
		}

		public override void Hit(Vector2 hitDir, int knockBackAmount, int knockBackSpeed)
		{
			if (--health <= 0)
			{
				game.EndGame();
			}
			else
			{
				if (hitSound != null)
					hitSound.Play();
			}
			base.Hit(hitDir, knockBackAmount, knockBackSpeed);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Color color = Color.White;
			if (isHit)
				color = Color.Pink;
			//TODO WALKING ANIMATION
			
			spriteBatch.Draw(texture, bounds, srcRect, color);
			/*spriteBatch.Draw(texture, new Rectangle(600, 600, 100, 100), srcRect, color);
			switch (bodyDirecton)
			{
				case Direction.Up:
					spriteBatch.Draw(texture, new Rectangle((int)loc.X + Width / 2 - 5, (int)loc.Y, 10, 10), Color.Blue);
					break;
				case Direction.Down:
					spriteBatch.Draw(texture, new Rectangle((int)loc.X + Width / 2 - 5, (int)loc.Y + Height - 10, 10, 10), Color.Blue);
					break;
				case Direction.Left:
					spriteBatch.Draw(texture, new Rectangle((int)loc.X, (int)loc.Y + Height / 2 - 5, 10, 10), Color.Blue);
					break;
				case Direction.Right:
					spriteBatch.Draw(texture, new Rectangle((int)loc.X + Width - 10, (int)loc.Y + Height / 2 - 5, 10, 10), Color.Blue);
					break;
			}
			*/
		}
	}
}
