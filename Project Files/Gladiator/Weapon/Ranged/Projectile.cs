using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	public class Projectile : DamageObject
	{
		public bool IsInactive
		{
			get;
			set;
		}
		private Vector2 vel;
		private Rectangle bounds;
		public RangedStats rangedStats;
		public Color color;
		public Projectile(Mob owner, Vector2 fireLoc, Vector2 shotDir, RangedStats rangedStats, Texture2D texture, Color color) : base(owner, fireLoc, shotDir, texture)
		{
			vel = shotDir * rangedStats.BulletSpeed;
			this.texture = texture;
			bounds = new Rectangle((int)fireLoc.X, (int)fireLoc.Y, rangedStats.BulletWidth, rangedStats.BulletHeight);
			IsInactive = false;
			this.rangedStats = rangedStats;
			this.color = color;
		}
		public void Update(GameTime gameTime)
		{
			if (!IsInactive)
			{
				Loc += vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (Loc.X + bounds.Width < 0 || Loc.X > Game1.GAME_WIDTH || Loc.Y + bounds.Height < 0 || Loc.Y > Game1.GAME_HEIGHT)
				{
					IsInactive = true;
				}
				else
				{
					bounds = new Rectangle((int)Loc.X, (int)Loc.Y, bounds.Width, bounds.Height);
				}
			}
		}
		public void Draw(SpriteBatch spriteBatch)
		{
			if (!IsInactive)
			{
				spriteBatch.Draw(texture, bounds, color);
			}	
		}
		public bool ColidesWith(Rectangle rect)
		{
			if (rect.Intersects(bounds))
			{
				return true;
			}
			else
				return false;
		}
	}
}
