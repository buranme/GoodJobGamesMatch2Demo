using System.Collections.Generic;
using Blocks;
using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// Pool that holds unused StandardBlocks rather than instantiating and destroying them needlessly
    /// Has the queue of StandardBlocks, and an oblivion position to send the Blocks to
    /// </summary>
    public class BlockPool
    {
        private readonly Queue<StandardBlock> _blockQueue;
        private StandardBlock _cachedBlock;
        private readonly Vector3 _oblivion = new(10000,0,0);
    
        public BlockPool()
        {
            _blockQueue = new Queue<StandardBlock>();
        }

        public StandardBlock Pull(Vector3 unitPosition)
        {
            if (_blockQueue.Count < 1) return null;
            _cachedBlock = _blockQueue.Dequeue();
            _cachedBlock.transform.position = unitPosition;
            return _cachedBlock;
        }

        // If the pushed block is not a standard block destroys it
        public void Push(Block pushed)
        {
            if (pushed.GetType() != typeof(StandardBlock))
            {
                pushed.DestroySelf();
                return;
            }
            pushed.transform.position = _oblivion;
            _blockQueue.Enqueue((StandardBlock) pushed);
        }
    }
}
