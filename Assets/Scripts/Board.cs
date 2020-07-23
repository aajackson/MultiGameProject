using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int Width { get { return boardWidth; } }
    public int Height { get { return boardHeight; } }

    public bool SpawnActive = true;
    public float SpawnDelay = 15.0f;
    [SerializeField]
    private int boardWidth  = 5;
    [SerializeField]
    private int boardHeight = 12;

    public void InitializeValidBoard()
    {
        // Set block colors and remove one block from the top
    }

    public Block BlockPrefab = null;
    [HideInInspector]
    public List<Block> BlockList = new List<Block>();
    public BlockMaterials BlockMaterials = null;
    private Coroutine spawnBlocksCoroutine;

    private void OnValidate()
    {
        if (BlockList.Count != boardWidth * boardHeight)
        {
            Debug.Log($"Validating! {BlockList.Count} != {boardWidth * boardHeight} ({boardWidth} * {boardHeight})");
            //UnityEditor.EditorApplication.delayCall += () =>
            //{
            //    DestroyBlocks();
            //    InstantiateBlocks();
            //};
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

    private void Start()
    {
        foreach (Block block in BlockList)
        {
            block.State = Block.BlockState.Empty;
        }
        spawnBlocksCoroutine = StartCoroutine(SpawnBlocks());
    }

    private IEnumerator SpawnBlocks()
    {
        // Initial spawn delay
        yield return new WaitForSeconds(SpawnDelay);

        // Stop spawning and game over if top row contains an active one
        for (int col = 0; col < Width; ++col)
        {
            if (this[col, Height - 1].State == Block.BlockState.Active)
            {
                SpawnActive = false;
            }
        }

        while (SpawnActive)
        {
            for (int col = 0; col < Width; ++col)
            {
                for (int row = Height - 1; row > 0; --row)
                {
                    ExchangeBlocks(new Vector2Int(col, row), new Vector2Int(col, row - 1));
                }

                // Spawn blocks that are different than the color to their left or above
                Block.BlockColor[] blockColors = (Block.BlockColor[])Enum.GetValues(typeof(Block.BlockColor));
                List<Block.BlockColor> validColors = new List<Block.BlockColor>(blockColors);
                if (LocationValid(col - 1, 0))
                {
                    // Remove left color from pool
                    validColors.Remove(this[col - 1, 0].Color);
                }
                if (this[col, 1].State == Block.BlockState.Active)
                {
                    // Remove up color from pool
                    validColors.Remove(this[col, 1].Color);
                }
                this[col, 0].Color = validColors.GetRandomItem();
                this[col, 0].State = Block.BlockState.Active;
            }

            yield return new WaitForSeconds(SpawnDelay);

            // Stop spawning and game over if top row contains an active one
            for (int col = 0; col < Width; ++col)
            {
                if (this[col, Height - 1].State == Block.BlockState.Active)
                {
                    SpawnActive = false;
                }
            }
        } // while (SpawnActive)

        Debug.Log("Spawning stopped! Game Over!");
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
        ExchangeBlocks(cursorPosition, cursorPositionRight);

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
        }
    }

    private void ExchangeBlocks(Vector2Int blockPos1, Vector2Int blockPos2)
    {
        // Change positions and references of two blocks
        Vector3 tmpPos = this[blockPos1].transform.position;
        this[blockPos1].transform.position = this[blockPos2].transform.position;
        this[blockPos2].transform.position = tmpPos;
        Block tmp = this[blockPos1];
        this[blockPos1] = this[blockPos2];
        this[blockPos2] = tmp;
    }

    private void FloodSearchBlocks(Vector2Int location, Block.BlockColor searchColor, int minBlockClearCount = 3)
    {
        bool[,] visitedBlocks = new bool[boardWidth, boardHeight];
        int sum = FloodSearchSum(location, searchColor, ref visitedBlocks);
        // Hide/Deactivate the cleared blocks
        if (sum >= minBlockClearCount)
        {
            for (int y = 0; y < boardHeight; ++y)
            {
                for (int x = 0; x < boardWidth; ++x)
                {
                    if (visitedBlocks[x, y] &&
                        this[x, y].Color == searchColor &&
                        this[x, y].State == Block.BlockState.Active)
                    {
                        this[x, y].State = Block.BlockState.Empty;
                    }
                }
            }
        }
        // Collapse the columns
        for (int col = 0; col < Width; ++col)
        {
            for (int row = 0; row < Height; ++row)
            {
                if (this[col, row].State == Block.BlockState.Empty)
                {
                    // Search for the first block above that is non-empty
                    for (int searchRow = row + 1; searchRow < Height; ++searchRow)
                    {
                        if (this[col, searchRow].State == Block.BlockState.Active)
                        {
                            ExchangeBlocks(new Vector2Int(col, row), new Vector2Int(col, searchRow));
                            break;
                        }
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
        bool blockActive = this[location].State == Block.BlockState.Active;
        if (searchColor != color || !blockActive)
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

    private bool LocationValid(int x, int y)
    {
        return LocationValid(new Vector2Int(x, y));
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
