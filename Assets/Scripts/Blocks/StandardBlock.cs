using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using BlockType = Lookup.StandardBlockType;
using BlockColor = Lookup.BlockColor;

namespace Blocks
{
    /// <summary>
    /// The standard block that has a color and has to form chunks of same color to be clicked.
    /// Has a reference to its SpriteRenderer for when it changes images, holds its color and type,
    /// and has a queue to use in the flood fill algorithm it uses to calculate the chunk it is in.
    /// </summary>
    public class StandardBlock : Block
    {
        [SerializeField] private SpriteRenderer blockImage;
        private BlockColor _color;
        private BlockType _type;
        
        private Queue<Tuple<int, int>> _queue;

        public override void Initialize(Board board)
        {
            Board = board;
            _type = BlockType.Plain;
        }
        
        public override BlockColor Color()
        {
            return _color;
        }

        public void SetRandomType()
        {
            var randomInt = Random.Range(0, Board.lookup.k);
            _color = (BlockColor) randomInt;
            blockImage.sprite = Board.lookup.defaultSprites[(int)_color];
        }
        
        protected override void OnMouseDown()
        {
            if (ToExplode.Count < 2 || !Board.ClickAvailable) return;
            SpawnSpecialBlockInPlace();
            Board.BlockClicked(ToExplode);
        }

        public override void SetToExplode(List<Block> list)
        {
            ToExplode.Clear();
            ToExplode.AddRange(list);
            ChangeIcon();
        }
        
        // Called after its ToExplode list has been updated. Changes the type and
        // Image into the correct one regarding values of a, b and c
        private void ChangeIcon()
        {
            if (ToExplode.Count > Board.lookup.c)
            {
                _type = BlockType.C;
                blockImage.sprite = Board.lookup.cSprites[(int)_color];
            }
            else if (ToExplode.Count > Board.lookup.b)
            {
                _type = BlockType.B;
                blockImage.sprite = Board.lookup.bSprites[(int)_color];
            }
            else if (ToExplode.Count > Board.lookup.a)
            {
                _type = BlockType.A;
                blockImage.sprite = Board.lookup.aSprites[(int)_color];
            }
            else
            {
                _type = BlockType.Plain;
                blockImage.sprite = Board.lookup.defaultSprites[(int)_color];
            }
        }

        // Uses the FloodFill algorithm to find all the neighboring blocks of same color
        // If the count is 1 (only this) returns an empty list
        public override List<Block> CalculateExplosion()
        {
            var (x, y) = Indices;
            _queue = new Queue<Tuple<int, int>>();
            ToExplode.Clear();
        
            _queue.Enqueue(new Tuple<int, int>(x,y));
            while (_queue.Count != 0)
            {
                var (i, j) = _queue.Dequeue();
                if (j < 0 || j >= Board.lookup.m || i < 0 || i >= Board.lookup.n) continue;
            
                var currentBlock = Board.Blocks[i, j];
                if(currentBlock.GetType() != typeof(StandardBlock)) continue;
                if(ToExplode.Contains(currentBlock)) continue;
                if(currentBlock.Color() != _color) continue;
            
                ToExplode.Add(Board.Blocks[i, j]);
                _queue.Enqueue(new Tuple<int, int>(i - 1, j));
                _queue.Enqueue(new Tuple<int, int>(i + 1, j));
                _queue.Enqueue(new Tuple<int, int>(i, j - 1));
                _queue.Enqueue(new Tuple<int, int>(i, j + 1));
            }

            ChangeIcon();
            return ToExplode.Count == 1 ? new List<Block>() : ToExplode;
        }

        // Method to spawn either a Rocket, Bomb or DiscoBall in place of the clicked StandardBlock,
        // If it is in a chunk that is larger than a, b or c
        private void SpawnSpecialBlockInPlace()
        {
            if (_type == BlockType.Plain) return;
            
            Block createdBlock;
            if (_type == BlockType.A)
                createdBlock = Instantiate(Board.lookup.rocketReference, transform.position, Quaternion.identity);
            else if (_type == BlockType.B)
                createdBlock = Instantiate(Board.lookup.bombReference, transform.position, Quaternion.identity);
            else
                createdBlock = Instantiate(Board.lookup.discoBallReference, transform.position, Quaternion.identity);
            
            createdBlock.Initialize(Board);
            createdBlock.transform.SetParent(Board.transform);
            createdBlock.SetColor(_color);
            Board.SetBlockOnBoard(Indices.Item1, Indices.Item2, createdBlock);
            ToExplode.Remove(this);
            Board.PushBlock(this);
        }
    }
}
