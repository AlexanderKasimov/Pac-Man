using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Uint")]
public class UintVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public uint initialValue;

    [NonSerialized]
    public uint runtimeValue;

    public void OnAfterDeserialize()
    {
        runtimeValue = initialValue;
    }

    public void OnBeforeSerialize()
    {

    }
}