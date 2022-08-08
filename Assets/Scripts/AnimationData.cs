using Blocks;
using UnityEngine;

/// <summary>
/// Class to make objects for each block being animated to hold the data about the animation
/// </summary>
public class AnimationData
{
    public Block Block { get; }
    public Vector3 Position { get; }

    public AnimationData(Block givenBlock, Vector3 givenPosition)
    {
        Block = givenBlock;
        Position = givenPosition;
    }
}
