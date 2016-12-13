using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maybe_You_will_finish_this_one
{
	class Spawner : IDisposable
	{
		private ContentManager content;
		public int wave;
		private float spawnTimer;
		private float spawnSpeedMS;
		private float skeleSpawnCount, zombieSpawnCount;
		private float zombieSpawnCap, skeleSpawnCap;
		private Player player;
		private List<Vector2> spawnLocs;
		private int waveLimiter;
		private Random random;
		private SoundEffect endWaveSound;
		public Spawner(Player player, IServiceProvider serviceProvider)
		{
			content = new ContentManager(serviceProvider, "Content");
			this.player = player;
			wave = 0;
			skeleSpawnCount = 0;
			zombieSpawnCount = 0;
			spawnTimer = 0;
			spawnLocs = new List<Vector2>();
			waveLimiter = 4;
			random = new Random();
			endWaveSound = content.Load<SoundEffect>("endwavetone");
		}
		public void AddSpawners(params Vector2[] locs)
		{
			foreach(Vector2 v in locs)
			{
				spawnLocs.Add(v);
			}
		}
		public void StartWave()
		{
			if (++wave == 6)
				waveLimiter = 2;
			if (wave == 12)
				waveLimiter = 1;
			skeleSpawnCount = 0;
			zombieSpawnCount = 0;
			zombieSpawnCap = wave * (8 / waveLimiter);
			spawnSpeedMS = 2000;
			if (wave >= 3)
			{
				skeleSpawnCap = (wave - 2) * (4 / waveLimiter);
			}
			endWaveSound.Play();
			spawnMonster();
		}

		private void spawnMonster()
		{
			foreach(Vector2 loc in spawnLocs)
			{
				if (skeleSpawnCount < skeleSpawnCap && zombieSpawnCount < zombieSpawnCap)
				{
					switch (random.Next(0, 2))
					{
						case 0:
							Zombie z = new Zombie((int)loc.X, (int)loc.Y, player, content.Load<Texture2D>("zombie"), content.Load<SoundEffect>("zombiehit"));
							z.Weapon = new ZombieWeapon(z, null, ZombieWeapon.BASE_MELEE_STATS, content.Load<Texture2D>("white"), content.Load<Texture2D>("white"), null);
							Game1.AddMonster(z);
							zombieSpawnCount++;
							break;
						case 1:
							Skelebro s = new Skelebro((int)loc.X, (int)loc.Y, player, content.Load<Texture2D>("skeleton"), content.Load<SoundEffect>("bone"));
							s.Weapon = new SkeleWeapon(s, SkeleWeapon.BASE_RANGED_STATS, null, content.Load<Texture2D>("bullet"), 
								content.Load<Texture2D>("white"), content.Load<SoundEffect>("BowShot"));
							Game1.AddMonster(s);
							skeleSpawnCount++;
							break;
					}
				}
				else if (skeleSpawnCount >= skeleSpawnCap)
				{
					Zombie z = new Zombie((int)loc.X, (int)loc.Y, player, content.Load<Texture2D>("zombie"), content.Load<SoundEffect>("zombiehit"));
					z.Weapon = new ZombieWeapon(z, null, ZombieWeapon.BASE_MELEE_STATS, content.Load<Texture2D>("white"), content.Load<Texture2D>("white"), null);
					Game1.AddMonster(z);
					zombieSpawnCount++;
				}
				else if (zombieSpawnCount >= zombieSpawnCap)
				{
					Skelebro s = new Skelebro((int)loc.X, (int)loc.Y, player, content.Load<Texture2D>("skeleton"), content.Load<SoundEffect>("bone"));
					s.Weapon = new SkeleWeapon(s, SkeleWeapon.BASE_RANGED_STATS, null, content.Load<Texture2D>("bullet"), 
						content.Load<Texture2D>("white"), content.Load<SoundEffect>("BowShot"));
					Game1.AddMonster(s);
					skeleSpawnCount++;
				}
				if (skeleSpawnCount >= skeleSpawnCap && zombieSpawnCount >= zombieSpawnCap)
					return;
			}
		}

		public void Update(GameTime gameTime)
		{
			if(skeleSpawnCount < skeleSpawnCap || zombieSpawnCount < zombieSpawnCap)
			{
				spawnTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				if (spawnTimer > spawnSpeedMS)
				{
					spawnMonster();
					spawnTimer -= spawnSpeedMS;
				}
			}
		}

		public void Dispose()
		{
			content.Unload();
			content.Dispose();
		}
	}
}
