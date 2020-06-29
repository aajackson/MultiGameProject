using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "BlockMaterials", menuName = "Scriptable Objects/Block Materials")]
public class BlockMaterials : ScriptableObject
{
    public List<Material> Materials = new List<Material>();

    public Material GetRandomItem()
    {
        return Materials.GetRandomItem();
    }
}
