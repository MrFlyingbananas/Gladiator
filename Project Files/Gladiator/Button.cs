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

	public enum ClickButton
	{
		Left,
		Right,
		Middle
	}
	public class Button
	{
		public delegate void ButtonHandler();
		public event ButtonHandler ButtonClicked;
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
		public Color ButtonColor
		{
			get;
			set;
		}
		public Color StringColor
		{
			get;
			set;
		}
		public string ButtonString
		{
			get;
			set;
		}
		public Vector2 ButtonClickOffset
		{
			get;
			set;
		}
		public float ButtonAlpha
		{
			get;
			set;
		}
		public SoundEffect ClickSound
		{
			get;
			set;
		}
		private Vector2 currOffset;
		private Rectangle bounds;
		private Texture2D texture;
		private SpriteFont stringFont;
		private Vector2 stringLoc;
		private MouseState currMouse, oldMouse;
		private bool pressed;
		public Button(int x, int y, int width, int height, string buttonString, SpriteFont stringFont, Color stringColor, Texture2D texture, Color buttonColor)
		{
			bounds = new Rectangle(x, y, width, height);
			this.texture = texture;
			this.stringFont = stringFont;
			this.ButtonColor = buttonColor;
			this.StringColor = stringColor;
			this.ButtonString = buttonString;
			currMouse = Mouse.GetState();
			if(buttonString != null)
				stringLoc = (new Vector2(width, height) - stringFont.MeasureString(buttonString)) / 2 + new Vector2(x, y);
			ButtonClickOffset = new Vector2(5, 5);
			ButtonAlpha = 1;
			pressed = false;
		}
		public void Draw(SpriteBatch sb)
		{
			sb.Draw(texture, new Rectangle(bounds.X + (int)currOffset.X, bounds.Y + (int)currOffset.Y, bounds.Width, bounds.Height), ButtonColor * ButtonAlpha);
			if (ButtonString != null)
			{
				sb.DrawString(stringFont, ButtonString, stringLoc + currOffset / 2, StringColor * ButtonAlpha);
			}
		}
		public void Update(GameTime gameTime)
		{
			oldMouse = currMouse;
			currMouse = Mouse.GetState();
			if(currMouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released && bounds.Contains(new Point(currMouse.X, currMouse.Y)) && !pressed)
			{
				if(ClickSound != null)
				{
					ClickSound.Play();
				}
				currOffset = ButtonClickOffset;
				pressed = true;
			}
			if (currMouse.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed && pressed)
			{
				currOffset = Vector2.Zero;
				if (bounds.Contains(new Point(currMouse.X, currMouse.Y)))
				{
					if (ClickSound != null)
					{
						ClickSound.Play(1, -.75f, 0);
					}
					ButtonClicked();
				}
				pressed = false;
			}
		}
	}
}
