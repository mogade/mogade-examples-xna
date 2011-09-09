using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogade;

namespace Mogade_Xna_Demo
{
    public class ScoreboardEntry
    {
        public string username;
        public string level;
        public int points;

        public ScoreboardEntry(Score score)
        {
            username = score.UserName;
            level = score.Data;
            points = score.Points;
        }
    }
}
