using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject BlockPrefab = null;
    public List<Material> BlockMaterials = new List<Material>();

    private const int BoardWidth = 5;
    private const int BoardHeight = 12;
    private GameObject[,] BlockBoard = new GameObject[BoardWidth, BoardHeight];

    private void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = new Color(0.45f, 0.45f, 0.55f, 0.5f);
        float blockPrefabWidth = 1.0f;
        Vector3 boardPlaySize = new Vector3(BoardWidth * blockPrefabWidth, BoardHeight * blockPrefabWidth, 0.85f);
        Vector3 offset = new Vector3(boardPlaySize.x / 2, boardPlaySize.y / 2, -boardPlaySize.z / 2);
        Gizmos.DrawCube(transform.position + offset, boardPlaySize);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize and spawn the block prefabs
        for (int y = 0; y < BoardHeight; y++)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                Vector3 blockPosition = new Vector3(x, y, 0);
                BlockBoard[x, y] = Instantiate(BlockPrefab, blockPosition, Quaternion.identity, this.transform);
                Material blockMaterial = BlockMaterials[(int)(UnityEngine.Random.value * BlockMaterials.Count)];
                BlockBoard[x, y].GetComponent<MeshRenderer>().material = blockMaterial;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
