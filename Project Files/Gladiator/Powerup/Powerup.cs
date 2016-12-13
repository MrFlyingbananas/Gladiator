using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	public class Powerup
	{
		public Rectangle SourceRectangle
		{
			get { return srcRect; }
		}
		public static Texture2D Powerup_Sprites;
		private Rectangle bounds;
		private Player player;
		private bool pickedUp = false;
		private PowerupType effectType;
		private int effectAmount, effectDuration;
		private Color effectColor;
		private float timer;
		public bool isSpent = false;
		private Rectangle srcRect;
		private Weapon savedWeapon;
		private Weapon weapon;
		public Powerup(int x, int y, int width, int height, Player player, PowerupType effectType, int effectAmount, Color effectColor, int effectDuration, Weapon weapon)
		{
			this.bounds = new Rectangle(x, y, width, height);
			this.player = player;
			this.effectAmount = effectAmount;
			this.effectColor = effectColor;
			this.effectType = effectType;
			this.effectDuration = effectDuration;
			switch (effectType)
			{
				case PowerupType.FireRateBoost:
					srcRect = new Rectangle(0, 0, 16, 16);
					break;
				case PowerupType.HealthBoost:
					srcRect = new Rectangle(32, 0, 16, 16);
					break;
				case PowerupType.SpeedBoost:
					srcRect = new Rectangle(16, 0, 16, 16);
					break;
				case PowerupType.Shotgun:
					srcRect = new Rectangle(48, 0, 16, 16);
					break;
			}
			this.weapon = weapon;
		}
		public void Update(GameTime gameTime)
		{
			if (!pickedUp && player.Bounds.Intersects(bounds))
			{
				TriggerPickup();
			}
			if (pickedUp && !isSpent)
			{
				timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (timer >= effectDuration)
				{
					RemovePowerup();
				}
					
			}
		}
		public void Draw(SpriteBatch sb)
		{
			if (!pickedUp)
			{
				sb.Draw(Powerup_Sprites, bounds, srcRect, Color.White);
			}
			
		}
		private void RemovePowerup()
		{
			isSpent = true;
			switch (effectType)
			{
				case PowerupType.FireRateBoost:
					player.Weapon.rangedStats.ReloadTimeMS += effectAmount;
					break;
				case PowerupType.SpeedBoost:
					player.speed -= effectAmount;
					player.Weapon.rangedStats.BulletSpeed -= effectAmount;
					break;
				case PowerupType.Shotgun:
					player.Weapon = savedWeapon;
					break;
			}
			player.activePowerup = null;
		}
		protected void TriggerPickup()
		{
			pickedUp = true;
			if (player.activePowerup != null && effectType != PowerupType.HealthBoost)
			{
				player.activePowerup.RemovePowerup();
				player.activePowerup = this;
			}else if(player.activePowerup == null && effectType != PowerupType.HealthBoost)
			{
				player.activePowerup = this;
			}
			else
			{
				isSpent = true;
			}
			switch (effectType)
			{
				case PowerupType.FireRateBoost:
					player.Weapon.rangedStats.ReloadTimeMS -= effectAmount;
					break;
				case PowerupType.HealthBoost:
					player.health += effectAmount;
					break;
				case PowerupType.SpeedBoost:
					player.speed += effectAmount;
					player.Weapon.rangedStats.BulletSpeed += effectAmount;
					break;
				case PowerupType.Shotgun:
					savedWeapon = player.Weapon;
					player.Weapon = weapon;
					break;
			}
		}
	}
}
