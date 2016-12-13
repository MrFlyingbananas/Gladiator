using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Maybe_You_will_finish_this_one
{
	class Zombie : Monster
	{
		private const int BASE_WIDTH = 36;
		private const int BASE_HEIGHT = 36;
		private const int BASE_HEALTH = 4;
		private const int BASE_SPEED = 100;
		private const int BASE_HIT_DURATION_MS = 100;
		private const int WALK_ANIM_DURATION_MS = 175;
		private const int walkAnimFrames = 2;
		private int animFrame;
		public Zombie(int x, int y, Player player, Texture2D texture, SoundEffect hitSound) : base(x, y, player, texture, hitSound)
		{
			Width = BASE_WIDTH;
			Height = BASE_HEIGHT;
			hitDurationMS = BASE_HIT_DURATION_MS;
			speed = BASE_SPEED;
			health = BASE_HEALTH;
			animFrame = 0;
			bounds = new Rectangle(x, y, BASE_WIDTH, BASE_HEIGHT);
			currAnim = AnimationState.Walking;
			srcRect = new Rectangle(0, 0, Width, Height);
			
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
			boostedMobs.Clear();
			float updateSpeed = tempSpeedInc + speed;
			tempSpeedInc = 0;
			if (!knockBacked)
			{
				Vector2 dir = (player.Location + new Vector2(player.Width / 2, player.Height / 2) - loc - new Vector2(Width / 2, Height / 2));
				float dist = dir.Length();
				dir.Normalize();
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
				if (dist < Weapon.meleeStats.Range)
				{
					Weapon.FireMelee(loc, dir);
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
			if (currAnim == AnimationState.Walking && animTimer >= WALK_ANIM_DURATION_MS)
			{
				if (animFrame >= walkAnimFrames - 1)
				{
					animFrame = 0;
				}
				else
				{
					animFrame++;
				}
				srcRect = new Rectangle(Width * animFrame, 0, Width, Height);
				animTimer = 0;
			}
		}
	}
}
