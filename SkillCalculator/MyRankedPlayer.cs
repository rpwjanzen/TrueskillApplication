using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkillCalculator
{
    public sealed class MyRankedPlayer : MyPlayer
    {
        public int Rank { get; set; }

        public override string ToString()
        {
            return base.ToString() + string.Format(", Rank={0}", Rank);
        }
    }
}
