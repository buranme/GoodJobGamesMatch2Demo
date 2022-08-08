using System.Collections.Generic;

namespace Blocks
{
    public class Bomb : Block
    {
        // Calculates all the Blocks in a 3x3 square
        public override List<Block> CalculateExplosion()
        {
            ToExplode.Clear();
            for (var i = Indices.Item1 - 1; i < Indices.Item1 + 2; i++)
            {
                if (i < 0 || i >= Board.lookup.n) continue;
                for (var j = Indices.Item2 - 1; j < Indices.Item2 + 2; j++)
                {
                    if (j < 0 || j >= Board.lookup.m) continue;
                    ToExplode.Add(Board.Blocks[i, j]);
                }
            }
            return ToExplode;
        }
    }
}
