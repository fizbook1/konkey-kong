﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace pakeman
{
    public class EnemyManager
    {
        TextureManager textures;
        public List<Enemy> enemies = new List<Enemy>();
        public static WorldState world = new WorldState();
        public static Enemy chasingEnemy;
        double currentTimer = 3000;
        const double CURRENTTIMER = 3000;
        public EnemyManager(TextureManager textures)
        {
            this.textures = textures;
        }
        public void Update(double time, Player player, ScoreManager score, TileManager tiles)
        {
            if(player.PowerupDuration > 0)
            {
                world.isPoweredUp = true;
            }
            else
            {
                world.isPoweredUp = false;
            }
            currentTimer -= time;
            if(currentTimer < 0)
            {
                currentTimer = CURRENTTIMER;
                chasingEnemy = enemies[new Random().Next(0, 3)];
            }
            
            foreach (Enemy e in enemies)
            {
                e.NeighborTiles(tiles.NeighborTiles(e.tilePosX, e.tilePosY));
                e.EntityMove();
                if(player.state == EntityState.PowerupGhost && e.state != EntityState.Death)
                {
                    e.state = EntityState.Scared;
                //    e.Scared(player);
                } else { e.ArtificialIntelligence(player); }
                if(player.state != EntityState.PowerupGhost && e.state != EntityState.Death)
                {
                    e.state = EntityState.Default;
                }
                e.Anim(time);
                e.ToggleState();

                Collision(e, player, score);
            }
        }

        public void UpdateDraw(SpriteBatch spriteBatch)
        {
            foreach (Enemy e in enemies)
            {
                e.Draw(spriteBatch);
            }
        }
        private void Collision(Enemy e, Player p, ScoreManager score)
        {
            if(e.size.Intersects(p.size) && e.state == EntityState.Default && p.state != EntityState.PowerupGhost && p.state != EntityState.Death)
            {
                p.Death();
            }
            else if (e.size.Intersects(p.size) && e.state != EntityState.Death && p.state == EntityState.PowerupGhost)
            {
                e.Death();
                score.Increment(200, e.pos);
            }
        }
        public void Reset()
        {
            enemies.Clear();
        }

    }
}
