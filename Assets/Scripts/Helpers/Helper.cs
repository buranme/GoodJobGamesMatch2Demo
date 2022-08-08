using System;
using System.Collections.Generic;
using Blocks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Helpers
{
    /// <summary>
    /// Has a couple of helper functions that don't manipulate the values of Blocks[,]
    /// </summary>
    public class Helper
    {
        private readonly Board _board;
        private List<Block> _checkedBlocks;
        private List<Block> _cachedBlocks;
    
        public Helper(Board board)
        {
            _board = board;
        }

        // Clamps all the values given in lookup scriptable object
        public void ClampLookupValues()
        {
            _board.lookup.m = Math.Clamp(_board.lookup.m, 2, 10);
            _board.lookup.n = Math.Clamp(_board.lookup.n, 2, 10);
            
            _board.lookup.k = Math.Clamp(_board.lookup.k, 1, 6);
            _board.lookup.k = Math.Clamp(_board.lookup.k, 1, _board.lookup.m * _board.lookup.n - 1);
            
            _board.lookup.c = Math.Clamp(_board.lookup.c, 3, 100);
            _board.lookup.b = Math.Clamp(_board.lookup.b, 2, _board.lookup.c - 1);
            _board.lookup.a = Math.Clamp(_board.lookup.a, 1, _board.lookup.b - 1);
        }

        // Called by the board when it needs a new StandardBlock.
        // If it can not pull from the pool, instantiates a new StandardBlock
        public Block GetStandardBlock(Vector3 blockPosition)
        {
            var block = _board.Pool.Pull(blockPosition);
            if (!block)
            {
                block = Object.Instantiate(_board.lookup.standardBlockReference, blockPosition, Quaternion.identity);
                block.Initialize(_board);
            }
        
            block.transform.SetParent(_board.transform);
            block.SetRandomType();
            return block;
        }

        // Called after the animations for falling/shuffling are over
        // For each block on the board makes them calculate the blocks they would destroy if
        // they were to be clicked.
        // Holds a reference to all the blocks that have been checked, if at the end of the loop not
        // a single possible move has been found, returns false so that the board shuffles itself
        public bool CheckBlocks()
        {
            _checkedBlocks = new List<Block>();
            foreach (var block in _board.Blocks)
            {
                _cachedBlocks = new List<Block>();
                if (_checkedBlocks.Contains(block)) continue;
                
                _cachedBlocks.AddRange(block.CalculateExplosion());

                if (block.GetType() != typeof(StandardBlock)) continue;
                
                _checkedBlocks.AddRange(_cachedBlocks);
                foreach (var elem in _cachedBlocks)
                {
                    elem.SetToExplode(_cachedBlocks);
                }
            }
            return _checkedBlocks.Count > 0;
        }
    }
}