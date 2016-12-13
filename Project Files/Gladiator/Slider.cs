using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	public enum Orientation
	{
		Vertical,
		Horizontal
	}
	public class Slider
	{
		public delegate void SliderHandler(float sliderPercent);
		public event SliderHandler SliderReleased;
		public int X
		{
			get
			{
				return bounds.X;
			}
			set
			{
				bounds = new Rectangle(value, bounds.Y, bounds.Width, bounds.Height);
			}
		}
		public int Y
		{
			get
			{
				return bounds.Y;
			}
			set
			{
				bounds = new Rectangle(bounds.X, value, bounds.Width, bounds.Height);
			}
		}
		public int Width
		{
			get
			{
				return bounds.Width;
			}
			set
			{
				bounds = new Rectangle(bounds.X, bounds.Y, value, bounds.Height);
			}
		}
		public int Height
		{
			get
			{
				return bounds.Height;
			}
			set
			{
				bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, value);
			}
		}
		public Color SliderColor
		{
			get;
			set;
		}
		public float SliderAlpha
		{
			get;
			set;
		}
		public SoundEffect ReleaseSound
		{
			get;
			set;
		}
		private Rectangle bounds;
		private Rectangle sliderButton;
		private Texture2D texture;
		private bool sliding;
		private Orientation orientation;
		private MouseState oldMouse, currMouse;
		private int maxLoc, minLoc;
		public float SliderPercent
		{
			set
			{
				sliderButton = new Rectangle(bounds.X - sliderButton.Width / 2 + bounds.Width / 2, 
					(int)((maxLoc - minLoc) * (1 - value) + bounds.Y - sliderButton.Height / 2), sliderButton.Width, sliderButton.Height);
				if (value == 1)
					sliderButton = new Rectangle(sliderButton.X, minLoc, sliderButton.Width, sliderButton.Height);
			}
		}
		public Slider(int x, int y, int sliderButtonWidth, int sliderButtonHeight, int sliderLineWidth, int sliderLineHeight, Texture2D sliderTexture, Color sliderColor, 
			Orientation or, float slidePercent)
		{
			texture = sliderTexture;
			this.SliderColor = sliderColor;
			SliderAlpha = 1f;
			sliding = false;
			this.orientation = or;
			switch (orientation)
			{
				case Orientation.Vertical:
					bounds = new Rectangle(x, y, sliderLineWidth, sliderLineHeight);
					minLoc = y - sliderButtonHeight / 2;
					maxLoc = bounds.Y + bounds.Height - sliderButton.Height / 2;
					sliderButton = new Rectangle(x - sliderButtonWidth / 2 + sliderLineWidth / 2, (int)((maxLoc - minLoc) *  (1 - slidePercent)) + bounds.Y, sliderButtonWidth, sliderButtonHeight);
					if (slidePercent == 1)
						sliderButton = new Rectangle(sliderButton.X, minLoc, sliderButton.Width, sliderButton.Height);
					break;
				case Orientation.Horizontal:
					bounds = new Rectangle(x, y, sliderLineWidth, sliderLineHeight);
					sliderButton = new Rectangle(x - sliderButtonWidth / 2, y - sliderButtonHeight / 2 + sliderLineHeight / 2, sliderButtonWidth, sliderButtonHeight);
					minLoc = sliderButton.X;
					maxLoc = bounds.X + bounds.Height - sliderButton.Height / X;
					break;
			}
			currMouse = Mouse.GetState();
		}
		public void Draw(SpriteBatch sb)
		{
			sb.Draw(texture, bounds, SliderColor * SliderAlpha);
			sb.Draw(texture, sliderButton, SliderColor * SliderAlpha);
		}
		public void Update(GameTime gameTime)
		{
			oldMouse = currMouse;
			currMouse = Mouse.GetState();
			if (currMouse.LeftButton == ButtonState.Pressed && sliderButton.Contains(new Point(currMouse.X, currMouse.Y)) && !sliding && oldMouse.LeftButton == ButtonState.Released)
			{
				sliding = true;
			}else if(currMouse.LeftButton == ButtonState.Pressed && bounds.Contains(new Point(currMouse.X, currMouse.Y)) && !sliding && oldMouse.LeftButton == ButtonState.Released)
			{
				switch (orientation)
				{
					case Orientation.Vertical:
						sliderButton.Y = currMouse.Y;
						break;
					case Orientation.Horizontal:
						sliderButton.X = currMouse.X;
						break;
				}
				sliding = true;
			}
			if (sliding)
			{
				switch (orientation)
				{
					case Orientation.Vertical:
						int addAmount = currMouse.Y - oldMouse.Y;
						if (sliderButton.Y + addAmount >= minLoc && sliderButton.Y + addAmount <= maxLoc)
							sliderButton.Y += (int)addAmount;
						break;
					case Orientation.Horizontal:
						addAmount = currMouse.X - oldMouse.X;
						if (sliderButton.X + addAmount >= minLoc && sliderButton.X + addAmount <= maxLoc)
							sliderButton.X += (int)addAmount;
						break;
				}
			}
			if (currMouse.LeftButton == ButtonState.Released && sliding)
			{
				if (ReleaseSound != null)
				{
					ReleaseSound.Play();
				}
				switch (orientation)
				{
					case Orientation.Vertical:
						int sliderButtonLoc = sliderButton.Y - bounds.Y + sliderButton.Height / 2;
						float sliderPercent = (float)sliderButtonLoc  / (float)(maxLoc - minLoc);
						sliderPercent = 1 - sliderPercent;
						SliderReleased(sliderPercent);
						break;
					case Orientation.Horizontal:
						sliderButtonLoc = sliderButton.X - bounds.X + sliderButton.Width / 2;
						sliderPercent = (float)sliderButtonLoc / (float)(maxLoc - minLoc);
						sliderPercent = 1 - sliderPercent;
						SliderReleased(sliderPercent);
						break;
				}
				sliding = false;
			}
		}
	}
}
