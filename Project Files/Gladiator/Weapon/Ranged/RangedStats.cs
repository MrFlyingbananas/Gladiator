using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	public class RangedStats
	{
		public int KnockBackAmount
		{
			get;
			set;
		}
		public int KnockBackSpeed
		{
			get;
			set;
		}
		public int ReloadTimeMS
		{
			get;
			set;
		}
		public int Range
		{
			get;
			set;
		}
		public int BulletSpeed
		{
			get;
			set;
		}
		public int BulletWidth
		{
			get;
			set;
		}
		public int BulletHeight
		{
			get;
			set;
		}
		public RangedStats(int knockBackAmount, int knockBackSpeed, int reloadTimeMS, int range, int bulletSpeed, int bulletWidth, int bulletHeight)
		{
			this.KnockBackAmount = knockBackAmount;
			this.KnockBackSpeed = knockBackSpeed;
			this.ReloadTimeMS = reloadTimeMS;
			this.Range = range;
			this.BulletSpeed = bulletSpeed;
			this.BulletWidth = bulletWidth;
			this.BulletHeight = bulletHeight;
		}
	}
}
