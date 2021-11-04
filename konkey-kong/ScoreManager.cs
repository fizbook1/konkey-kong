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
    public class ScoreManager
    {
        public int score = 0;
        public List<PointString> list = new List<PointString>();
        public List<string> highscores = new List<string>();
        public void Increment(int value, Vector2 pos)
        {
            score += value;
            PointString pointStringCreate = new PointString(value, pos);
            list.Add(pointStringCreate);

        }
        public void Update(double time)
        {
            int? killObject = null;
            foreach (PointString ps in list)
            {
                ps.PointStringAnim(time);
                if (ps.duration < 0)
                {
                    killObject = list.IndexOf(ps);
                }
            }
            if (killObject != null)
            {
                list.RemoveAt((int)killObject);
            }
        }

        public void UpdateDraw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            foreach (PointString ps in list)
            {
                ps.Draw(spriteBatch, spriteFont);
            }
        }
        public void CalculateHighscores()
        {
            List<int> sortingList = new List<int>();
            if (score > 0)
            {
                highscores.Add(score.ToString());
            }
            foreach (string s in highscores)
            {
                sortingList.Add(Int32.Parse(s));
            }
            sortingList.Sort();
            sortingList.Reverse();

            highscores.Clear();
            if (sortingList.Count > 10)
            {
                for (int j = 0; j < 10; j++)
                {
                    highscores.Add(sortingList[j].ToString());
                }
            }
            else
            {
                foreach (int i in sortingList)
                {
                    highscores.Add(i.ToString());
                }
            }
            File.WriteAllLines("highscores.txt", highscores, Encoding.UTF8);
        }
    }
}
