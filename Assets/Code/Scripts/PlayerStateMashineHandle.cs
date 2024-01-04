using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStateMashineHandle
{
    public enum PlayerState
    {
        None, //0
        Dragging, //1
        IsBeingDragged, //2
        IsBeingHeld, //3
        Interacting, //4
        Emoting, //5
        IsBeingThrown, //6
        Dead //7
    }
    
    public enum HoldingState
    {
        HoldingNothing, //A
        HoldingItem, //B
        HoldingPlayer //C
    }

}
