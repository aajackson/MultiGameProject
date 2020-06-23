using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Block BlockPrefab = null;
    public List<Block> BlockList = new List<Block>();

    public readonly int boardWidth = 5;
    public readonly int boardHeight = 12;

    public List<Material> BlockMaterials = new List<Material>();

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

    public void Start()
    {
        BlockList.Capacity = boardHeight * boardWidth;
        while (BlockList.Count < boardHeight * boardWidth)
        {
            int index = BlockList.Count;
            Vector3 currentPos = this.transform.position;
            int x = index % boardWidth;
            int y = index / boardWidth;
            Vector3 blockPosition = new Vector3(x, y, 0) + currentPos;
            Block newBlock = Instantiate<Block>(BlockPrefab, blockPosition, Quaternion.identity, this.transform);
            BlockList.Add(newBlock);
            newBlock.GetComponent<MeshRenderer>().material = BlockMaterials.GetRandomItem();
        }
    }

    public void Swap(Vector2Int cursorPosition)
    {
        // Swap the two blocks at cursor position and cursor position + (1, 0)
        Vector3 tmpPos = this[cursorPosition.x, cursorPosition.y].transform.position;
        this[cursorPosition.x, cursorPosition.y].transform.position = this[cursorPosition.x + 1, cursorPosition.y].transform.position;
        this[cursorPosition.x + 1, cursorPosition.y].transform.position = tmpPos;
        Block tmp = this[cursorPosition.x, cursorPosition.y];
        this[cursorPosition.x, cursorPosition.y] = this[cursorPosition.x + 1, cursorPosition.y];
        this[cursorPosition.x + 1, cursorPosition.y] = tmp;
    }
}
