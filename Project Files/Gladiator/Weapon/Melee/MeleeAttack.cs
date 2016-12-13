using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maybe_You_will_finish_this_one
{
	public class MeleeAttack : DamageObject
	{
		
		private Vector2 meleeLoc;
		private Vector2 meleeDir;
		public MeleeStats meleeStats;
		public MeleeAttack(Mob owner, Vector2 loc, Vector2 dir, MeleeStats meleeStats, Texture2D texture) : base(owner, loc, dir, texture)
		{
			meleeDir = dir;
			meleeLoc = loc + dir * meleeStats.Range;
			this.meleeStats = meleeStats;
		}

		public bool ColidesWith(Rectangle rect)
		{
			if (meleeDir.X >= 0 && meleeDir.Y >= 0 && meleeLoc.X >= rect.X && meleeLoc.Y >= rect.Y && (meleeLoc - new Vector2(rect.X, rect.Y)).Length() <= meleeStats.Range) 
			{
				return true;
			}
			else if (meleeDir.X >= 0 && meleeDir.Y <= 0 && meleeLoc.X >= rect.X && meleeLoc.Y <= rect.Y + rect.Height && (meleeLoc - new Vector2(rect.X, rect.Y)).Length() <= meleeStats.Range)
			{
				return true;
			}
			else if (meleeDir.X <= 0 && meleeDir.Y >= 0 && meleeLoc.X <= rect.X + rect.Width && meleeLoc.Y >= rect.Y && (meleeLoc - new Vector2(rect.X, rect.Y)).Length() <= meleeStats.Range) 
			{
				return true;
			}
			else if (meleeDir.X <= 0 && meleeDir.Y <= 0 && meleeLoc.X <= rect.X + rect.Width && meleeLoc.Y <= rect.Y + rect.Height && (meleeLoc - new Vector2(rect.X, rect.Y)).Length() <= meleeStats.Range)
			{
				return true;
			}
			return false;
		}
	}
}
