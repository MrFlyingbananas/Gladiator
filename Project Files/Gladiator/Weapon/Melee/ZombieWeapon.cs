using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Maybe_You_will_finish_this_one
{
	public class ZombieWeapon : Weapon
	{
		public const int BASE_M_KNOCK_BACK_AMOUNT = 30;
		public const int BASE_M_KNOCK_BACK_SPEED = 400;
		public const int BASE_M_RELOAD_TIME_MS = 2000;
		public const int BASE_M_RANGE = 38;
		public static MeleeStats BASE_MELEE_STATS = new MeleeStats(BASE_M_KNOCK_BACK_AMOUNT, BASE_M_KNOCK_BACK_SPEED, BASE_M_RELOAD_TIME_MS, BASE_M_RANGE);
		public ZombieWeapon(Mob owner, RangedStats ranged, MeleeStats melee, Texture2D pTexture, Texture2D mTexture, SoundEffect soundEffect) 
			: base(owner, ranged, melee, pTexture, mTexture, soundEffect)
		{
		}
		public override void FireProjectile(Vector2 loc, Vector2 addedVel, Direction dir){ }

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
