using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	public abstract class Mob
	{
		public int Width
		{
			get;
			protected set;
		}
		public int Height
		{
			get;
			protected set;
		}
		public Vector2 Location
		{
			get { return loc; }
		}
		public Rectangle Bounds
		{
			get { return bounds; }
		}
		public bool IsHit
		{
			get { return isHit; }
		}
		public Weapon Weapon
		{
			get;
			set;
		}
		public bool CanColide
		{
			get;
			set;
		}
		public int speed;
		protected Texture2D texture;
		protected Vector2 loc;
		public int health;
		protected bool isHit = false;
		protected Rectangle bounds;
		protected float hitTimer;
		protected int hitDurationMS;
		protected bool knockBacked = false;
		protected int knockBackSpeed;
		protected int knockBackDistanceMax;
		protected float knockBackDistance = 0;
		protected Vector2 knockBackDir;
		protected Direction bodyDirecton;
		protected float tempSpeedInc;
		public List<Mob> boostedMobs;
		protected AnimationState currAnim;
		protected AnimationState oldAnim;
		protected float animTimer;
		protected Rectangle srcRect;
		protected SoundEffect hitSound;
		public Mob(int x, int y, Texture2D texture, SoundEffect hitSound)
		{
			hitTimer = 0;
			loc = new Vector2(x, y);
			this.texture = texture;
			CanColide = true;
			bodyDirecton = Direction.Up;
			tempSpeedInc = 0;
			boostedMobs = new List<Mob>();
			currAnim = AnimationState.Idle;
			oldAnim = AnimationState.Idle;
			this.hitSound = hitSound;
		}
		public virtual void Update(GameTime gameTime)
		{
			Weapon.Update(gameTime);
			if (isHit)
			{
				hitTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				if (hitTimer > hitDurationMS)
					isHit = false;
			}
			if (knockBacked)
			{
				knockBackDistance += (knockBackDir * (float)gameTime.ElapsedGameTime.TotalSeconds * knockBackSpeed).Length();
				if (knockBackDistance >= knockBackDistanceMax)
					knockBacked = false;
			}
			animTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			bounds = new Rectangle((int)loc.X, (int)loc.Y, Bounds.Width, Bounds.Height);
		}
		public abstract void Draw(SpriteBatch spriteBatch);
		public virtual void Hit(Vector2 hitDir, int knockBackAmount, int knockBackSpeed)
		{
			isHit = true;
			hitTimer = 0;
			if (knockBackAmount != 0)
			{
				KnockBack(hitDir, knockBackAmount, knockBackSpeed);
			}
			
		}
		public void AddSpeedForUpdate(float speed)
		{
			tempSpeedInc += speed;
		}
		protected void KnockBack(Vector2 hitDir, int knockBackAmount, int knockBackSpeed)
		{
			this.knockBackSpeed = knockBackSpeed;
			knockBackDistanceMax = knockBackAmount;
			knockBackDir = hitDir;
			knockBackDistance = 0;
			knockBacked = true;
		}
		
	}
}
