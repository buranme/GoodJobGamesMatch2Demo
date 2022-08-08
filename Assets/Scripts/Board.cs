using System;
using System.Collections.Generic;
using Blocks;
using Helpers;
using UnityEngine;

/// <summary>
/// The board and the game master of the whole game.
/// Creates and destroys blocks, calculates where each block should fall to,
/// Uses BlockAnimator to animate movement, shuffles the blocks if necessary
/// </summary>
public class Board : MonoBehaviour
{
    // Where most of the values are stored
    public Lookup lookup;
    
    // Has a couple of helper functions
    private Helper _helper;
    
    // Pool to prevent needless instantiate and destroy of blocks
    public BlockPool Pool;
    
    // Class to animate block movement
    public BlockAnimator animator;
    
    // Array to hold the blocks in the board
    public Block[,] Blocks { get; private set; }
    
    // While the animations are running prevents clicks on the blocks
    public bool ClickAvailable { get; private set; }
    
    private List<Block> _blocksCache;
    private Vector3 _selfPosition;
    
    public AudioSource click;
    public AudioSource shuffle;

    private void Awake()
    {
        ClickAvailable = false;
        _selfPosition = transform.position;
        _blocksCache = new List<Block>();
        Blocks = new Block[lookup.n, lookup.m];
        _helper = new Helper(this);
        Pool = new BlockPool();
    }
    
    private void Start()
    {
        _helper.ClampLookupValues();
        Initialize();
    }

    private void Initialize()
    {
        animator.Initialize(this);
    
        var position = _selfPosition;
        for (var j = 0; j < lookup.m; j++)
        {
            for (var i = 0; i < lookup.n; i++)
            {
                var newBlock = _helper.GetStandardBlock(position + new Vector3(Lookup.OffsetX * i,0, -j));
                SetBlockOnBoard(i, j, newBlock);
            }
            position.y += Lookup.OffsetY;
        }
        AdjustSelf();
        AnimationsFinished();
    }

    // Adjusts its position so that the blocks are perfectly centered
    private void AdjustSelf()
    {
        var towardsLeft = (1 - lookup.n) * Lookup.OffsetX * 0.5f;
        var towardsDown = (1 - lookup.m) * Lookup.OffsetY * 0.5f;
        _selfPosition += new Vector3(towardsLeft, towardsDown, 0);
        transform.position = _selfPosition;
    }

    // Called by a block that has been clicked.
    // PARAM: list of blocks to be destroyed
    public void BlockClicked(List<Block> blocks)
    {
        ClickAvailable = false;
        click.Play();
        
        // Free Blocks of each block and push them to the pool
        foreach (var block in blocks)
        {
            var (x, y) = block.Indices;
            Blocks[x, y] = null;
            Pool.Push(block);
        }
        
        FallDown();
    }

    // Running for each column bottom to top, makes each block fall if there's empty spots under them
    // And pulls new blocks from the pool and places them on top of the column
    // Sends each block that should be falling to the animator, when the animator is done with
    // every block, the game loop continues from AnimationsFinished()
    private void FallDown()
    {
        // For each column
        for (var x = 0; x < lookup.n; x++)
        {
            var position = _selfPosition + Vector3.right * (Lookup.OffsetX * x);
            var emptySpots = 0;
            
            // Each block falls down if there's empty spot(s) underneath
            for (var y = 0; y < lookup.m; y++)
            {
                if (Blocks[x, y] == null)
                {
                    emptySpots++;
                    continue;
                }
                if (emptySpots > 0)
                {
                    animator.AnimateBlock(Blocks[x,y], new Vector3(position.x,position.y,emptySpots-y));
                    SetBlockOnBoard(x, y - emptySpots, Blocks[x, y]);
                    Blocks[x, y] = null;
                }
                position += Vector3.up * Lookup.OffsetY;
            }

            // New blocks are pulled from the pool for each of the empty spots on top of the column
            for (var i = 0; i < emptySpots; i++)
            {
                var blockRow = i + lookup.m - emptySpots;
                var newPosition = position + Vector3.up * Lookup.OffsetY * emptySpots;
                var createdBlock = _helper.GetStandardBlock(newPosition);
                animator.AnimateBlock(createdBlock, new Vector3(position.x,position.y,-blockRow));

                position += Vector3.up * Lookup.OffsetY;
                SetBlockOnBoard(x, blockRow, createdBlock);
            }
        }
    }
    
    // Gets called by the animator when it is done animating each falling block.
    // Shuffles the blocks around until there's at least one block that can cause an explosion
    // When there is a potential move, makes click available
    public void AnimationsFinished()
    {
        if (!_helper.CheckBlocks())
        {
            ShuffleBoard();
        }
        ClickAvailable = true;
    }

    // Shuffles all the blocks around
    // Copies the blocks into a 1D list, shuffles the list, puts the list back into the Blocks[,] and animates movement
    private void ShuffleBoard()
    {
        shuffle.Play();
        _blocksCache = new List<Block>();
        foreach (var block in Blocks)
        {
            _blocksCache.Add(block);
        }

        for (var i = 0; i < _blocksCache.Count; i++)
        {
            var temp = _blocksCache[i];
            var random = UnityEngine.Random.Range(i, _blocksCache.Count);
            _blocksCache[i] = _blocksCache[random];
            _blocksCache[random] = temp;
        }
        
        var index = 0;
        var position = _selfPosition;
        for (var j = 0; j < lookup.m; j++)
        {
            for (var i = 0; i < lookup.n; i++)
            {
                SetBlockOnBoard(i, j, _blocksCache[index++]);
                animator.AnimateBlock(Blocks[i, j], position + new Vector3(Lookup.OffsetX * i,0, -j));
            }
            position.y += Lookup.OffsetY;
        }
    }

    public void SetBlockOnBoard(int x, int y, Block block)
    {
        block.SetIndicesAndName(x, y);
        Blocks[x, y] = block;
    }

    public void PushBlock(Block pushed)
    {
        Pool.Push(pushed);
    }
}
