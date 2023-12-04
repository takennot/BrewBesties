using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemStateMachine
{
    // Start is called before the first frame update
    public enum ItemState
    {
        None,
        IsBeingDragged,
        IsBeingHeld,
        IsBeingThrown
    }
}
