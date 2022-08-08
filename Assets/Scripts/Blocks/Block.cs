using System;
using System.Collections.Generic;
using UnityEngine;
using BlockColor = Lookup.BlockColor;

namespace Blocks
{
    /// <summary>
    /// Each one of the blocks in the board.
    /// Has a reference to the board, the blocks it would destroy if it was to be clicked,
    /// and its indices in the Blocks[,] array of the board.
    /// Has 4 children: StandardBlock, Rocket, Bomb and DiscoBall
    /// </summary>
    public class Block : MonoBehaviour
    {
        protected Board Board;
        protected List<Block> ToExplode;
        public Tuple<int,int> Indices { get; private set; }
        
        private void Awake()
        {
            ToExplode = new List<Block>();
        }

        // Name has been used for dubegging purposes, decided to leave it in
        public void SetIndicesAndName(int x, int y)
        {
            Indices = new Tuple<int, int>(x, y);
            gameObject.name = $"({x},{y})";
        }
        
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
        
        /////////////VIRTUALS///////////////
        
        // Saves the given list in ToExplode
        // Used when the block has been calculated to be in a chunk of same colored Blocks
        // All the blocks in the chunk are given the List to avoid using CalculateExplosion for each one of them
        public virtual void SetToExplode(List<Block> list){}
        
        // Only StandardBlock and DiscoBall uses Color
        public virtual BlockColor Color(){ return BlockColor.None; }
        
        // Sets the color into given one.
        // Only used by DiscoBall on creation to set the color to that of the clicked StandardBlock
        public virtual void SetColor(BlockColor color){}
        
        // Each children of Block uses their own algorithm to calculate the Blocks they will destroy if clicked
        public virtual List<Block> CalculateExplosion(){ return new List<Block>(); }
        
        public virtual void Initialize(Board board)
        {
            Board = board;
        }

        // When clicked it sends ToExplode list to the board, since after each run of animations
        // The board calls CalculateExplosion on every block, ToExplode is always up to date
        protected virtual void OnMouseDown()
        {
            if (!Board.ClickAvailable) return;
            Board.BlockClicked(ToExplode);
        }
    }
}
