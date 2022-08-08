using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using RocketType = Lookup.RocketType;

namespace Blocks
{
    public class Rocket : Block
    {
        [SerializeField] private SpriteRenderer blockImage;
        private RocketType _type;
    
        public override void Initialize(Board board)
        {
            Board = board;
            SetRandomType();
        }
        
        private void SetRandomType()
        {
            var randomInt = Random.Range(0, 2);
            _type = (RocketType) randomInt;
            blockImage.sprite = Board.lookup.rocketSprites[(int)_type];
        }

        // Calculate all the Blocks on a vertical or a horizontal line
        public override List<Block> CalculateExplosion()
        {
            ToExplode.Clear();
            switch (_type)
            {
                case RocketType.X:
                {
                    for (var i = 0; i < Board.lookup.n; i++)
                    {
                        ToExplode.Add(Board.Blocks[i, Indices.Item2]);
                    }
                    break;
                }
                case RocketType.Y:
                {
                    for (var j = 0; j < Board.lookup.m; j++)
                    {
                        ToExplode.Add(Board.Blocks[Indices.Item1, j]);
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return ToExplode;
        }
    }
}
