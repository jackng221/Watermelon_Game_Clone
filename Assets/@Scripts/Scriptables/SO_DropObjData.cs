using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_DropObjData", menuName = "ScriptableObjects/SO_DropObjData", order = 1)]
public class SO_DropObjData : ScriptableObject
{
    public List<SO_DropObj> dropItems;

    [Tooltip("Largest random growth for objects dropped by player. Starts at 0")]
    public int largestDropObjGrowth = 0;
}
