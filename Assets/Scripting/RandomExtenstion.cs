using System.Collections.Generic;
using UnityEngine;

namespace MilitaryDemo
{
    public static class RandomExtenstion
    {
        public static T GetRandom<T>(this List<T> list)
        {
            return list[RandomTool.NextInt(list.Count)];
        }
    }
}
