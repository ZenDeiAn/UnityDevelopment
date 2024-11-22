using System;
using System.Collections;
using System.Collections.Generic;
using RaindowStudio.Utility;
using UnityEngine;

public class TEST : MonoBehaviour
{
    public EnumPairList<TestType, GameObject> testPairList = new EnumPairList<TestType, GameObject>();

}

[Serializable]
public enum TestType
{
    A = 0,
    B = 1,
    J = 3,
    C = 2,
    D
}
