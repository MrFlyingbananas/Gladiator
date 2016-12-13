using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	public abstract class Monster : Mob
	{
		
		protected bool isDead = false;
		protected Player player;
		protected static Random random;
		protected const int Powerup_Drop_Rate = 13;
		public Monster(int x, int y, Player player, Texture2D texture, SoundEffect hitSound) : base(x, y, texture, hitSound)
		{
			loc = new Vector2(x, y);
			this.player = player;
			random = new Random();
		}
		public override void Hit(Vector2 hitDir, int knockBackAmount, int knockBackSpeed)
		{
			if (--health <= 0)
			{
				isDead = true;
				Game1.RemoveMonster(this);
				int chance = random.Next(1, 101);
				if (chance >= (100 - Powerup_Drop_Rate))
				{
					int type = random.Next(1, 5);
					Game1.DropPowerup(new Vector2(this.loc.X + Width/2, this.loc.Y + Height/2), type);
				}
			}
			if (hitSound != null)
				hitSound.Play(.8f, 0, 0);
			base.Hit(hitDir, knockBackAmount, knockBackSpeed);
		}
	}
}

