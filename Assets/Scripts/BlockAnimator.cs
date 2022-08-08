using System.Collections.Generic;
using Blocks;
using UnityEngine;

/// <summary>
/// Animates movement of the blocks
/// Each block to be moved gets sent to here by the board, the block gets added to the list
/// On each fixed update the list of blocks gets iterated and each block moved slightly
/// If a block reaches its destination it is removed from the list
/// When the list is completely empty, notifies the board so that it allows user input again
/// Creates an AnimationData object for each block
/// </summary>
public class BlockAnimator : MonoBehaviour
{
    private Board _board;
    
    // The main list that holds the blocks to be animated
    private List<AnimationData> _blocksToBeAnimated;
    
    // The list that holds the blocks that are being animated in that particular fixed update
    private List<AnimationData> _blocksBeingAnimated;

    private bool _isRunningAnimation;
    private AnimationData _cachedAnimation;
    private Vector3 _cachedVector;
    private Block _cachedBlock;
    private Transform _cachedTransform;
    private int _cachedIndex;

    public void Initialize(Board board)
    {
        _board = board;
        _blocksToBeAnimated = new List<AnimationData>();
        _blocksBeingAnimated = new List<AnimationData>();
    }

    // Moves each block slightly towards their destination on each update
    private void FixedUpdate()
    {
        if (!_isRunningAnimation) return;
        
        _blocksBeingAnimated.Clear();
        _blocksBeingAnimated.AddRange(_blocksToBeAnimated);
        
        foreach (var elem in _blocksBeingAnimated)
        {
            _cachedTransform = elem.Block.transform;
            var position = _cachedTransform.position;
            _cachedVector = elem.Position - position;
            position += _cachedVector.normalized * Lookup.AnimationSpeed;
            _cachedTransform.position = position;

            // Remove block from the list if it has reached the destination
            if (_cachedVector.magnitude < Lookup.AnimationSpeed * 2)
            {
                _cachedTransform.position = elem.Position;
                _blocksToBeAnimated.Remove(elem);
            }
        }

        // If the list is empty notify the board
        if (_blocksToBeAnimated.Count >= 1) return;
        _isRunningAnimation = false;
        _board.AnimationsFinished();
    }

    // Gets called by the board to add a block for animation
    // PARAM: Block to be moved and its destination
    public void AnimateBlock(Block block, Vector3 endPosition)
    {
        if (!_isRunningAnimation)
        {
            _isRunningAnimation = true;
        }

        // Z values of each block is used to make sure the higher blocks stay on top of the lower ones
        // since the given images for blocks are fake 3d and have upper "sides" that should tuck under the block above
        // Since this animator moves each Block by a fixed amount, it's important that they don't move
        // in the z dimension to ensure they are not slowed down in x and y, so their z values are
        // immediately set to their destination before starting the animation
        var blockTransform = block.transform;
        var blockPosition = blockTransform.position;
        blockTransform.position = new Vector3(blockPosition.x, blockPosition.y, endPosition.z);
        
        _cachedAnimation = new AnimationData(block, endPosition);
        _blocksToBeAnimated.Add(_cachedAnimation);
    }
}
