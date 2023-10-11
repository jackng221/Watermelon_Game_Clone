using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_DropObj", menuName = "ScriptableObjects/SO_DropObj", order = 1)]
public class SO_DropObj : ScriptableObject
{
    public Color color;
    public float sizeMultiplier;
    public float growthOrder;
}
