using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum C_SOUNDS
{
   Steel = 0,
   SteelB = 1,
    Ball = 2,
    Chair = 3,
    BigObject = 4
};
public class CollisionSound : UnityEvent<Vector3, C_SOUNDS> {}