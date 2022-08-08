using System.Collections.Generic;
using Blocks;
using UnityEngine;

/// <summary>
/// Scriptable object to hold the variables and be used as a lookup table
/// </summary>
[CreateAssetMenu(fileName = "LookupScriptableObject", menuName = "ScriptableObjects/Lookup")]
public class Lookup : ScriptableObject
{
    // Possible colors for StandardBlocks and DiscoBalls
    public enum BlockColor
    {
        Yellow,
        Blue,
        Green,
        Pink,
        Purple,
        Red,
        None
    }
    
    // Determines which image the StandardBlocks will have
    // determined by the size of the chunk they are in
    public enum StandardBlockType
    {
        Plain,
        A,
        B,
        C
    }
    
    // Type of the rocket
    public enum RocketType
    {
        X,
        Y
    }
    
    // Size of each Block in x and y
    public const float OffsetX = 2.2f;
    public const float OffsetY = 2.1f;
    public const float AnimationSpeed = 0.25f;

    // All the variables that can be changed, use the created scriptable object to change them
    public int m = 6; //rows
    public int n = 6; //columns
    public int k = 5; //colors
    public int a = 5; //rockets
    public int b = 7; //bombs
    public int c = 9; //disco balls
    
    // References for the prefabs
    public StandardBlock standardBlockReference;
    public Rocket rocketReference;
    public Bomb bombReference;
    public DiscoBall discoBallReference;
    
    // The Block sprites
    public List<Sprite> defaultSprites;
    public List<Sprite> aSprites;
    public List<Sprite> bSprites;
    public List<Sprite> cSprites;
    public List<Sprite> rocketSprites;
    public List<Sprite> discoBallSprites;
}
