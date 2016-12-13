using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	public abstract class Weapon
	{
		public Mob Owner
		{
			get;
			protected set;
		}
		protected Texture2D pTexture;
		protected Texture2D mTexture;
		protected float reloadTimer, meleeTimer;
		public bool canFire = true, canMelee = true;
		public MeleeStats meleeStats;
		public RangedStats rangedStats;
		protected SoundEffect soundEffect;
		public Weapon(Mob owner, RangedStats ranged, MeleeStats melee, Texture2D pTexture, Texture2D mTexture, SoundEffect soundEffect)
		{
			this.Owner = owner;
			this.pTexture = pTexture;
			this.mTexture = mTexture;
			this.meleeStats = melee;
			this.rangedStats = ranged;
			this.soundEffect = soundEffect;
		}
		public abstract void FireProjectile(Vector2 loc, Vector2 vel, Direction dir);
		public abstract void FireMelee(Vector2 loc, Vector2 dir);
		public void Update(GameTime gameTime)
		{
			if (!canFire)
			{
				reloadTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				if (reloadTimer > rangedStats.ReloadTimeMS)
				{
					canFire = true;
				}
			}
			if (!canMelee)
			{
				meleeTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				if (meleeTimer > meleeStats.ReloadTimeMS)
				{
					canMelee = true;
				}
			}
		}
	}
}
