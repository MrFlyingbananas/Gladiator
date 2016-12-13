using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	public abstract class DamageObject
	{
		public Vector2 Loc
		{
			get;
			protected set;
		}
		public Mob Owner
		{
			get;
			protected set;
		}
		public Vector2 Dir
		{
			get;
			protected set;
		}
		protected Texture2D texture;
		public DamageObject(Mob owner, Vector2 loc, Vector2 dir, Texture2D texture)
		{
			this.Loc = loc;
			this.texture = texture;
			this.Owner = owner;
			this.Dir = dir;
		}
	}
}
