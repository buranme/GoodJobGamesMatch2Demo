using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    public class DiscoBall : Block
    {
        [SerializeField] private SpriteRenderer blockImage;
        private Lookup.BlockColor _color;
        
        public override Lookup.BlockColor Color()
        {
            return _color;
        }
        
        public override void SetColor(Lookup.BlockColor color)
        {
            _color = color;
            blockImage.sprite = Board.lookup.discoBallSprites[(int)_color];
        }

        // Calculates all the StandardBlocks that share the same color
        public override List<Block> CalculateExplosion()
        {
            ToExplode.Clear();
            foreach (var block in Board.Blocks)
            {
                if(block.Color() == _color && block.GetType() != GetType())
                    ToExplode.Add(block);
            }
            return ToExplode;
        }
    }
}
