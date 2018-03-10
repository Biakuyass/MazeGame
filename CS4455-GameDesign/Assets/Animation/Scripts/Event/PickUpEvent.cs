using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GrabableType
{
    Ball,
    smallBox,
    smalllog
};
public class PickUpEvent : UnityEvent<GrabableType> {

}
