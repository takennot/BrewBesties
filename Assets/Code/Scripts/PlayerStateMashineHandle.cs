using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStateMashineHandle
{
    public enum PlayerState
    {
        None,
        Dragging,
        IsBeingDragged,
        IsBeingHeld,
        Interacting,
        Emoting,
        IsBingThrown
    }
    
    public enum HoldingState
    {
        HoldingNothing,
        HoldingItem,
        HoldingPlayer
    }

}
