using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	public class MeleeStats
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

		public MeleeStats(int knockBackAmount, int knockBackSpeed, int reloadTimeMS, int range)
		{
			this.KnockBackAmount = knockBackAmount;
			this.KnockBackSpeed = knockBackSpeed;
			this.ReloadTimeMS = reloadTimeMS;
			this.Range = range;
		}
	}
}
