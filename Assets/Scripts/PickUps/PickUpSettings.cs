using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickUpSettings",menuName = "PickUps/Settings")]
public class PickUpSettings : ScriptableObject
{
    public float scoreValue;
    public Mesh mesh;   
}
