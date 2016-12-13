using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	class Skelebro : Monster
	{
		private const int BASE_WIDTH = 36;
		private const int BASE_HEIGHT = 36;
		private const int BASE_HEALTH = 3;
		private const int BASE_SPEED = 100;
		private const int BASE_HIT_DURATION_MS = 100;
		private const int WALK_ANIM_DURATION_MS = 175;
		private const int STOP_RANGE = 300;
		private const int FIRE_ANIM_DURATION_MS = 300;
		public Skelebro(int x, int y, Player player, Texture2D texture, SoundEffect hitSound) : base(x, y, player, texture, hitSound)
		{
			Width = BASE_WIDTH;
			Height = BASE_HEIGHT;
			hitDurationMS = BASE_HIT_DURATION_MS;
			speed = BASE_SPEED;
			health = BASE_HEALTH;
			animTimer = 0;
			bounds = new Rectangle(x, y, BASE_WIDTH, BASE_HEIGHT);
			currAnim = AnimationState.Walking;
			srcRect = new Rectangle(0, Height, Width, Height);
			currAnim = AnimationState.Idle;
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			Color color = Color.White;
			if (isHit)
				color = Color.Red;
			spriteBatch.Draw(texture, bounds, srcRect, color);
		}

		public override void Update(GameTime gameTime)
		{
			oldAnim = currAnim;
			boostedMobs.Clear();
			float updateSpeed = tempSpeedInc + speed;
			Vector2 dir = (player.Location + new Vector2(player.Width / 2, player.Height / 2) - loc - new Vector2(Width / 2, Height / 2));
			float dist = dir.Length();
			tempSpeedInc = 0;
			if (!knockBacked)
			{
				dir.Normalize();
				if (dist > STOP_RANGE)
				{
					Vector2 tempLoc = new Vector2(loc.X + dir.X * updateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds,
					loc.Y + dir.Y * updateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
					Mob mob = Game1.IsSpaceOcupied(this, tempLoc);
					if (mob == null)
					{
						loc += dir * updateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					}
					else
					{
						if (mob.GetType() != typeof(Player))
						{

							loc += dir * updateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

							if (!mob.boostedMobs.Contains(this))
							{
								mob.AddSpeedForUpdate(speed / 2);
								this.boostedMobs.Add(mob);
							}
						}
					}
				}
				else if (dist < STOP_RANGE)
				{
					if (dist > STOP_RANGE)
					{
						Vector2 tempLoc = new Vector2(loc.X + dir.X * updateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds,
						loc.Y + dir.Y * updateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
						Mob mob = Game1.IsSpaceOcupied(this, tempLoc);
						if (mob == null)
						{
							loc -= dir * updateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
						}
						else
						{
							if (mob.GetType() != typeof(Player))
							{

								loc -= dir * updateSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

								if (!mob.boostedMobs.Contains(this))
								{
									mob.AddSpeedForUpdate(speed / 2);
									this.boostedMobs.Add(mob);
								}
							}
						}
					}
				}
				Vector2 vec = player.Location - loc;
				float angle = (float)Math.Atan2(vec.Y, vec.X);
				angle = MathHelper.ToDegrees(angle);
				angle += 180;
				vec.Normalize();
				if (angle >= 0 && angle < 45 || angle > 315 && angle <= 360)
				{
					bodyDirecton = Direction.Left;
				}
				else if (angle >= 45 && angle < 135)
				{
					bodyDirecton = Direction.Up;
				}
				else if (angle >= 135 && angle < 225)
				{
					bodyDirecton = Direction.Right;
				}
				else if (angle >= 225 && angle < 315)
				{
					bodyDirecton = Direction.Down;
				}
				if (Weapon.canFire)
				{
					currAnim = AnimationState.Firing;
					Weapon.FireProjectile(loc, vec, bodyDirecton);
					animTimer = 0;
				}
			}
			else
			{
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

			base.Update(gameTime);
			if (currAnim == AnimationState.Firing && animTimer >= FIRE_ANIM_DURATION_MS)
			{
				if (dist > STOP_RANGE)
					currAnim = AnimationState.Idle;
				else
				{
					currAnim = AnimationState.Walking;
				}
			}
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
		}
	}
}
