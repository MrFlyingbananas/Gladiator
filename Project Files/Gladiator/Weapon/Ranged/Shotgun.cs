using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Maybe_You_will_finish_this_one
{
	public class Shotgun : Weapon
	{
		public static int BASE_R_KNOCK_BACK_AMOUNT = 20;
		public static int BASE_R_KNOCK_BACK_SPEED = 200;
		public static int BASE_R_RANGE = 0;
		public static int BASE_R_RELOAD_TIME_MS = 1000;
		public static int BASE_BULLET_SPEED = 500;
		public static int BASE_BULLET_WIDTH = 8;
		public static int BASE_BULLET_HEIGHT = 8;
		public static RangedStats BASE_RANGED_STATS = new RangedStats(BASE_R_KNOCK_BACK_AMOUNT, BASE_R_KNOCK_BACK_SPEED, BASE_R_RELOAD_TIME_MS, BASE_R_RANGE, BASE_BULLET_SPEED, BASE_BULLET_WIDTH, BASE_BULLET_HEIGHT);

		public Shotgun(Mob owner, RangedStats ranged, MeleeStats melee, Texture2D pTexture, Texture2D mTexture, SoundEffect soundEffect)
			: base(owner, ranged, melee, pTexture, mTexture, soundEffect)
		{
		}
		public override void FireProjectile(Vector2 loc, Vector2 addedVel, Direction dir)
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
				Vector2 vDir1 = Vector2.Zero, vDir2 = Vector2.Zero, vDir3 = Vector2.Zero;
				double tan = Math.Tan(.15d);
				switch (dir)
				{
					case Direction.Up:
						vDir1 = new Vector2(addedVel.X, -rangedStats.BulletSpeed);
						vDir2 = new Vector2((float)tan * vDir1.Y, vDir1.Y);
						vDir3 = new Vector2(-(float)tan * vDir1.Y, vDir1.Y);
						break;
					case Direction.Down:
						vDir1 = new Vector2(addedVel.X, rangedStats.BulletSpeed);
						vDir2 = new Vector2((float)tan * vDir1.Y, vDir1.Y);
						vDir3 = new Vector2(-(float)tan * vDir1.Y, vDir1.Y);
						break;
					case Direction.Left:
						vDir1 = new Vector2(-rangedStats.BulletSpeed, addedVel.Y);
						vDir2 = new Vector2(vDir1.X, (float)tan * vDir1.X);
						vDir3 = new Vector2(vDir1.X, -(float)tan * vDir1.X);
						break;
					case Direction.Right:
						vDir1 = new Vector2(rangedStats.BulletSpeed, addedVel.Y);
						vDir2 = new Vector2(vDir1.X, (float)tan * vDir1.X);
						vDir3 = new Vector2(vDir1.X, -(float)tan * vDir1.X);
						break;
				}
				vDir1.Normalize();
				vDir2.Normalize();
				vDir3.Normalize();
				Game1.AddProjectile(new Projectile(Owner, fireOffset, vDir1, rangedStats, pTexture, Color.White));
				Game1.AddProjectile(new Projectile(Owner, fireOffset, vDir2, rangedStats, pTexture, Color.White));
				Game1.AddProjectile(new Projectile(Owner, fireOffset, vDir3, rangedStats, pTexture, Color.White));
				soundEffect.Play(.3f, .5f, 0f);
				canFire = false;
				reloadTimer = 0;
			}
		}


		public override void FireMelee(Vector2 loc, Vector2 dir)
		{
			if (canMelee)
			{
				Game1.AddMeleeAttack(new MeleeAttack(Owner, loc, dir, meleeStats, mTexture));
				canMelee = false;
				meleeTimer = 0;
			}
		}
	}
}
