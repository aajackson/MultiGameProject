using System;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum BlockColor
    {
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
    }

    public enum BlockState
    {
        Active,
        Empty,
    }

    public BlockState State
    {
        get { return state; }
        set
        {
            state = value;
            switch (value)
            {
                case BlockState.Active:
                    SetVisible(true);
                    break;
                case BlockState.Empty:
                    SetVisible(false);
                    break;
                default:
                    break;
            }
        }
    }

    private void SetVisible(bool visible)
    {
        GetComponent<MeshRenderer>().enabled = visible;
    }

    [SerializeField]
    private BlockState state;
    public BlockColor Color
    {
        get { return color; }
        set
        {
            color = value;
            Material newMaterial = null;
            switch (value)
            {
                case BlockColor.Red:
                    newMaterial = BlockMaterials.Materials[0];
                    break;
                case BlockColor.Orange:
                    newMaterial = BlockMaterials.Materials[1];
                    break;
                case BlockColor.Yellow:
                    newMaterial = BlockMaterials.Materials[2];
                    break;
                case BlockColor.Green:
                    newMaterial = BlockMaterials.Materials[3];
                    break;
                case BlockColor.Blue:
                    newMaterial = BlockMaterials.Materials[4];
                    break;
            }
            GetComponent<MeshRenderer>().material = newMaterial;
        }
    }
    [SerializeField]
    private BlockColor color;
    
    public BlockMaterials BlockMaterials = null;
}
