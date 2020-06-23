using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public static class ListExtensions
{
    public static T GetRandomItem<T>(this List<T> list)
    {
        int randomIndex = (int)(UnityEngine.Random.value * list.Count);
        return list[randomIndex];
    }
}
