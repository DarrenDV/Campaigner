using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public SnapPointType snapPointType;
}

public enum SnapPointType
{
    None,
    Top,
    Bottom,
    Left,
    Right,
    Front,
    Back
}