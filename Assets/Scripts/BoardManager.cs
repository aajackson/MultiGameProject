using UnityEngine;
using UnityEngine.InputSystem;

public class BoardManager : MonoBehaviour
{
    public GameObject Cursor = null;
    public Vector2Int CursorPosition
    {
        get { return cursorPosition; }
        set
        {
            if (value.x < 0)
                value.x = 0;
            if (value.x > GameBoard.Width - 2)
                value.x = GameBoard.Width - 2;
            if (value.y < 0)
                value.y = 0;
            if (value.y > GameBoard.Height - 1)
                value.y = GameBoard.Height - 1;

            cursorPosition = value;

            if (Cursor != null)
            {
                Cursor.transform.localPosition = new Vector3(cursorPosition.x, cursorPosition.y, Cursor.transform.localPosition.z);
            }
        }
    }
    private Vector2Int cursorPosition = new Vector2Int(0, 0); // Max (boardWidth - 1, boardHeight - 1)

    public Board GameBoard = null;



    private void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = new Color(0.45f, 0.45f, 0.55f, 0.5f);
        float blockPrefabWidth = 1.0f;
        Vector3 boardPlaySize = new Vector3(GameBoard.Width * blockPrefabWidth, GameBoard.Height * blockPrefabWidth, 0.85f);
        Vector3 offset = new Vector3(boardPlaySize.x / 2, boardPlaySize.y / 2, -boardPlaySize.z / 2);
        Gizmos.DrawCube(this.transform.position + offset, boardPlaySize);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize and spawn the block prefabs
        //StartCoroutine(SpawnInitialBlocks());
    }

    //IEnumerator SpawnInitialBlocks()
    //{
    //    Vector3 currentPos = this.transform.position;
    //    for (int y = 0; y < boardHeight; y++)
    //    {
    //        for (int x = 0; x < boardWidth; x++)
    //        {
    //            Vector3 blockPosition = new Vector3(x, y, 0) + currentPos;
    //            blockBoard[x, y] = Instantiate(BlockPrefab, blockPosition, Quaternion.identity, this.transform);
    //            blockBoard[x, y].GetComponent<MeshRenderer>().material = BlockMaterials.GetRandomItem();
    //            yield return null;
    //        }
    //    }
    //}

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveCursor(InputAction.CallbackContext context)
    {
    }

    public void MoveCursorX(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int value = (int)context.ReadValue<float>();
            CursorPosition += Vector2Int.right * value;
        }
    }

    public void MoveCursorY(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int value = (int)context.ReadValue<float>();
            CursorPosition += Vector2Int.up * value;
        }
    }

    public void Swap(InputAction.CallbackContext context)
    {
        //if((context.control as UnityEngine.InputSystem.Controls.ButtonControl).wasPressedThisFrame)
        if (context.performed)
        {
            GameBoard.Swap(CursorPosition);
        }
    }
}
