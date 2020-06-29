using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int Width { get { return boardWidth; } }
    public int Height { get { return boardHeight; } }

    [SerializeField]
    private int boardWidth  = 5;
    [SerializeField]
    private int boardHeight = 12;
    public Block BlockPrefab = null;
    [HideInInspector]
    public List<Block> BlockList = new List<Block>();

    public BlockMaterials BlockMaterials = null;

    private void OnValidate()
    {
        if (BlockList.Count != boardWidth * boardHeight)
        {
            
            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyBlocks();
                InstantiateBlocks();
            };
        }
    }
 
    public void DestroyBlocks()
    {
        Queue<Block> blockChildren = new Queue<Block>(this.GetComponentsInChildren<Block>());
        while(blockChildren.Count > 0)
        {
            Block block = blockChildren.Dequeue();
            StartCoroutine(DestroyBlock(block.gameObject));
        }
        BlockList.Clear();
    }
       
    private IEnumerator DestroyBlock(GameObject block)
    {
        yield return new WaitForEndOfFrame();
        DestroyImmediate(block);
    }


    public Block this[int x, int y]
    {
        get
        {
            int index = x + y * boardWidth;
            return BlockList[index];
        }

        set
        {
            int index = x + y * boardWidth;
            BlockList[index] = value;
        }
    }

    public Block this[Vector2Int pos]
    {
        get { return this[pos.x, pos.y]; }
        set { this[pos.x, pos.y] = value; }
    }

    public void InstantiateBlocks()
    {
        BlockList.Capacity = boardHeight * boardWidth;
        for (int index = 0; index < boardHeight * boardWidth; ++index)
        {
            Vector3 currentPos = this.transform.position;
            int x = index % boardWidth;
            int y = index / boardWidth;
            Vector3 blockPosition = new Vector3(x, y, 0) + currentPos;
            StartCoroutine(InstantiateBlock(blockPosition));
        }
    }

    private IEnumerator<Block> InstantiateBlock(Vector3 blockPosition)
    {
        Block newBlock = PrefabUtility.InstantiatePrefab(BlockPrefab, this.transform) as Block;
        newBlock.transform.position = blockPosition;
        BlockList.Add(newBlock);

        Array blockColorArray = Enum.GetValues(typeof(Block.BlockColor));
        newBlock.Color = (Block.BlockColor)blockColorArray.GetValue((int)(UnityEngine.Random.value * blockColorArray.Length));
        yield return newBlock;
    }

    public void Swap(Vector2Int cursorPosition)
    {
        // Swap the two blocks at cursor position and cursor position + (1, 0)
        Vector2Int cursorPositionRight = cursorPosition + Vector2Int.right;
        Vector3 tmpPos = this[cursorPosition].transform.position;
        this[cursorPosition].transform.position = this[cursorPositionRight].transform.position;
        this[cursorPositionRight].transform.position = tmpPos;
        Block tmp = this[cursorPosition];
        this[cursorPosition] = this[cursorPositionRight];
        this[cursorPositionRight] = tmp;

        // Validate the board
        Block.BlockColor leftColor = this[cursorPosition].Color;
        Block.BlockColor rightColor = this[cursorPositionRight].Color;

        const int minBlockClearCount = 3;
        if (this[cursorPosition].State == Block.BlockState.Active)
        {
            FloodSearchBlocks(cursorPosition, leftColor, minBlockClearCount);
        }
        if (this[cursorPositionRight].State == Block.BlockState.Active &&
            leftColor != rightColor)
        {
            FloodSearchBlocks(cursorPositionRight, rightColor, minBlockClearCount);
            //bool[,] visitedBlocks = new bool[boardWidth, boardHeight];
            //int rightSum = FloodSearchSum(cursorPositionRight, rightColor, ref visitedBlocks);
            //if (rightSum >= minBlockClearCount)
            //{
            //    FloodFillClear(cursorPositionRight);
            //    for (int y = 0; y < boardHeight; ++y)
            //    {
            //        for (int x = 0; x < boardWidth; ++x)
            //        {
            //            if (visitedBlocks[x, y] && this[x, y].Color == rightColor)
            //            {
            //                this[x, y].enabled = false;
            //            }
            //        }
            //    }
            //}
        }
    }

    private void FloodSearchBlocks(Vector2Int location, Block.BlockColor searchColor, int minBlockClearCount = 3)
    {
        bool[,] visitedBlocks = new bool[boardWidth, boardHeight];
        int sum = FloodSearchSum(location, searchColor, ref visitedBlocks);
        if (sum >= minBlockClearCount)
        {
            //FloodFillClear(cursorPosition);
            for (int y = 0; y < boardHeight; ++y)
            {
                for (int x = 0; x < boardWidth; ++x)
                {
                    if (visitedBlocks[x, y] && this[x, y].Color == searchColor)
                    {
                        this[x, y].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private int FloodSearchSum(Vector2Int location, Block.BlockColor searchColor, ref bool[,] visitedBlocks)
    {
        // Return the count of the similar colored cardinal blocks
        visitedBlocks[location.x, location.y] = true;

        Block.BlockColor color = this[location].Color;
        if (searchColor != color)
        {
            return 0;
        }

        // Search the cardinals
        Vector2Int locationLeft = location + Vector2Int.left;
        Vector2Int locationDown = location + Vector2Int.down;
        Vector2Int locationRight = location + Vector2Int.right;
        Vector2Int locationUp = location + Vector2Int.up;

        // Visit tiles that are in bounds and haven't been visited yet
        int sum = 1;
        if(LocationValid(locationLeft) && !visitedBlocks[locationLeft.x, locationLeft.y])
        {
            sum += FloodSearchSum(locationLeft, searchColor, ref visitedBlocks);
        }
        if (LocationValid(locationDown) && !visitedBlocks[locationDown.x, locationDown.y])
        {
            sum += FloodSearchSum(locationDown, searchColor, ref visitedBlocks);
        }
        if (LocationValid(locationRight) && !visitedBlocks[locationRight.x, locationRight.y])
        {
            sum += FloodSearchSum(locationRight, searchColor, ref visitedBlocks);
        }
        if (LocationValid(locationUp) && !visitedBlocks[locationUp.x, locationUp.y])
        {
            sum += FloodSearchSum(locationUp, searchColor, ref visitedBlocks);
        }

        return sum;
    }

    private bool LocationValid(Vector2Int location)
    {
        return location.x >= 0 &&
            location.x < boardWidth &&
            location.y >= 0 &&
            location.y < boardHeight;
    }

    private void FloodFillClear(Vector2Int location)
    {
        
    }
}
