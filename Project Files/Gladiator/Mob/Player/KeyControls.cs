using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	public class KeyControls
	{
		public Keys keyUp, keyLeft, keyRight, keyDown, keyFireUp, keyFireDown, keyFireLeft, keyFireRight, melee;
		public KeyControls(Keys keyUp, Keys keyDown, Keys keyLeft, Keys keyRight, Keys keyFireUp, Keys keyFireDown, Keys keyFireLeft, Keys keyFireRight, Keys melee)
		{
			this.keyUp = keyUp;
			this.keyLeft = keyLeft;
			this.keyRight = keyRight;
			this.keyDown = keyDown;
			this.keyFireUp = keyFireUp;
			this.keyFireDown = keyFireDown;
			this.keyFireLeft = keyFireLeft;
			this.keyFireRight = keyFireRight;
			this.melee = melee;
		}

	}
}
