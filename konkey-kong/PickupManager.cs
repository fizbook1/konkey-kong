using Microsoft.Xna.Framework;
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
    public class PickupManager
    {
        TextureManager textures;
        SoundManager sound;
        double bigPickupTimer = 20000;
        const double BIGPICKUPTIMER = 20000;
        int bigPickupsCreated = 0;
        bool bigPickupCreated = false;
        public PickupManager(TextureManager textures, SoundManager sound)
        {
            this.textures = textures;
            this.sound = sound;
        }

        public List<Pickup> list = new List<Pickup>();
        public void Update(double time, Player player, ScoreManager score, TileManager tiles)
        {
            int? killPickup = null;
            foreach (Pickup p in list)
            {
                if(Collision(p, player))
                {
                    score.Increment(p.value, p.pos);
                    if (p.type == PickupType.PowerupEatWall)
                    {
                        player.PowerupDuration = 12000;
                        player.state = EntityState.PowerupWall;
                        sound.powerupInst.Play();
                    }

                    if (p.type == PickupType.PowerupEatGhost)
                    {
                        player.PowerupDuration = 12000;
                        player.state = EntityState.PowerupGhost;
                        sound.powerupInst.Play();
                    }

                    if (p.type == PickupType.Points) { sound.pickupInst.Play(); }
                    killPickup = list.IndexOf(p);
                }
            }
            if (killPickup != null)
            {
                list.RemoveAt((int)killPickup);
            }
            foreach (Powerup p in list.OfType<Powerup>())
            {
                p.Anim(time);
            }
            bigPickupTimer -= time;
            if( bigPickupTimer < 0) { CreateBigPickup(tiles); }
            

        }
        public void UpdateDraw(SpriteBatch spriteBatch)
        {
            foreach(Pickup p in list)
            {
                p.Draw(spriteBatch);
            }
        }
        private bool Collision(Pickup p, Player player)
        {
            if(p.size.Intersects(player.size))
            {
                return true;
            }
            else { return false; }
        }
        private void CreateBigPickup(TileManager tiles)
        {
            Random rnd = new Random();
            int pickupRandX = rnd.Next(4, tiles.currentMap.GetLength(0));
            int pickupRandY = rnd.Next(4, tiles.currentMap.GetLength(1));

            if (tiles.currentMap[pickupRandX, pickupRandY].type == TileType.Standard)
            {
                if (bigPickupsCreated == 0)
                {
                    BigPickup pickupCreator = new BigPickup(tiles.currentMap[pickupRandX, pickupRandY].pos, textures.cherry, tiles.currentMap[pickupRandX, pickupRandY].size);
                    pickupCreator.value = 200;
                    pickupCreator.BigPickupCreation();
                    list.Add(pickupCreator);
                }
                if (bigPickupsCreated == 1)
                {
                    BigPickup pickupCreator = new BigPickup(tiles.currentMap[pickupRandX, pickupRandY].pos, textures.strawberry, tiles.currentMap[pickupRandX, pickupRandY].size);
                    pickupCreator.value = 350;
                    pickupCreator.BigPickupCreation();
                    list.Add(pickupCreator);
                }
                if (bigPickupsCreated == 2)
                {
                    BigPickup pickupCreator = new BigPickup(tiles.currentMap[pickupRandX, pickupRandY].pos, textures.orange, tiles.currentMap[pickupRandX, pickupRandY].size);
                    pickupCreator.value = 500;
                    pickupCreator.BigPickupCreation();
                    list.Add(pickupCreator);
                }

                bigPickupCreated = true;
            }
            if (bigPickupCreated)
            {
                bigPickupsCreated++;
                bigPickupTimer = BIGPICKUPTIMER;
                bigPickupCreated = false;
            }
        }
        public void Reset()
        {
            bigPickupTimer = BIGPICKUPTIMER;
            bigPickupsCreated = 0;
            bigPickupCreated = false;
            list.Clear();
        }
    }
}
