using Assets.Scripts.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardManager : MonoBehaviour
{
    public GameObject BlockPrefab = null;
    public List<Material> BlockMaterials = new List<Material>();
    public GameObject Cursor = null;
    public Vector2 CursorPosition
    {
        get { return cursorPosition; }
        set
        {
            if (value.x < 0)
                value.x = 0;
            if (value.x > boardWidth - 2)
                value.x = boardWidth - 2;
            if (value.y < 0)
                value.y = 0;
            if (value.y > boardHeight - 1)
                value.y = boardHeight - 1;

            cursorPosition = value;

            if (Cursor != null)
            {
                Cursor.transform.localPosition = cursorPosition;
            }
        }
    }

    private const int boardWidth = 5;
    private const int boardHeight = 12;
    private GameObject[,] blockBoard = new GameObject[boardWidth, boardHeight];
    private Vector2 cursorPosition = new Vector2(0,0); // Max (boardWidth - 1, boardHeight - 1)



    private void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = new Color(0.45f, 0.45f, 0.55f, 0.5f);
        float blockPrefabWidth = 1.0f;
        Vector3 boardPlaySize = new Vector3(boardWidth * blockPrefabWidth, boardHeight * blockPrefabWidth, 0.85f);
        Vector3 offset = new Vector3(boardPlaySize.x / 2, boardPlaySize.y / 2, -boardPlaySize.z / 2);
        Gizmos.DrawCube(this.transform.position + offset, boardPlaySize);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize and spawn the block prefabs
        Vector3 currentPos = this.transform.position;
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Vector3 blockPosition = new Vector3(x, y, 0) + currentPos;
                //blockBoard[x, y] = Instantiate(BlockPrefab, blockPosition, Quaternion.identity, this.transform);
                //blockBoard[x, y].GetComponent<MeshRenderer>().material = BlockMaterials.GetRandomItem();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveCursor(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Move Cursor Called" + context.ReadValue<Vector2>());
        }
    }

    public void MoveCursorX(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float value = context.ReadValue<float>();
            Debug.Log("Move Cursor X Called" + value);
            CursorPosition += new Vector2(value, 0);
        }
    }

    public void MoveCursorY(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float value = context.ReadValue<float>();
            Debug.Log("Move Cursor Y Called" + value);
            CursorPosition += new Vector2(0, value);
        }
    }

    public void Swap(InputAction.CallbackContext context)
    {
        //if((context.control as UnityEngine.InputSystem.Controls.ButtonControl).wasPressedThisFrame)
        if(context.performed)
        {
            Debug.Log("Swap Called");
        }
    }
}
