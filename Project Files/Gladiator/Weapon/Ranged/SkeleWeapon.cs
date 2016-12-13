using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	class SkeleWeapon : Weapon
	{
		public static int BASE_R_KNOCK_BACK_AMOUNT = 20;
		public static int BASE_R_KNOCK_BACK_SPEED = 200;
		public static int BASE_R_RANGE = 0;
		public static int BASE_R_RELOAD_TIME_MS = 1600;
		public static int BASE_BULLET_SPEED = 450;
		public static int BASE_BULLET_WIDTH = 8;
		public static int BASE_BULLET_HEIGHT = 10;
		public static RangedStats BASE_RANGED_STATS = new RangedStats(BASE_R_KNOCK_BACK_AMOUNT, BASE_R_KNOCK_BACK_SPEED, BASE_R_RELOAD_TIME_MS, BASE_R_RANGE, BASE_BULLET_SPEED, BASE_BULLET_WIDTH, BASE_BULLET_HEIGHT);
		public SkeleWeapon(Mob owner, RangedStats ranged, MeleeStats melee, Texture2D pTexture, Texture2D mTexture, SoundEffect soundEffect)
			: base(owner, ranged, melee, pTexture, mTexture, soundEffect)
		{
		}
		public override void FireProjectile(Vector2 loc, Vector2 vec, Direction dir)
		{
			if (canFire)
			{
				Vector2 fireOffset = Vector2.Zero;
				switch (dir)
				{
					case Direction.Up:
						fireOffset = new Vector2(loc.X + Owner.Width / 2 - rangedStats.BulletWidth / 2, loc.Y);
						break;
					case Direction.Down:
						fireOffset = new Vector2(loc.X + Owner.Width / 2 - 5, (int)loc.Y + Owner.Height - rangedStats.BulletHeight);
						break;
					case Direction.Left:
						fireOffset = new Vector2(loc.X, (int)loc.Y + Owner.Height / 2 - rangedStats.BulletHeight / 2);
						break;
					case Direction.Right:
						fireOffset = new Vector2(loc.X + Owner.Width - rangedStats.BulletWidth, (int)loc.Y + Owner.Height / 2 - rangedStats.BulletWidth / 2);
						break;
				}
				Vector2 vDir = Vector2.One * vec;
				
				vDir.Normalize();
				Game1.AddProjectile(new Projectile(Owner, fireOffset, vDir, rangedStats, pTexture, Color.OrangeRed));
				soundEffect.Play();
				canFire = false;
				reloadTimer = 0;
			}
		}


		public override void FireMelee(Vector2 loc, Vector2 dir) { }
	}
}
