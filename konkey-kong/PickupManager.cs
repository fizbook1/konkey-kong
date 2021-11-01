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
    class PickupManager
    {
    public List<Pickup> list = new List<Pickup>();
        public void Update(double time, Player player, ScoreManager score)
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
                    }

                    if (p.type == PickupType.PowerupEatGhost)
                    {
                        player.PowerupDuration = 12000;
                        player.state = EntityState.PowerupGhost;
                    }
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
    }
}
