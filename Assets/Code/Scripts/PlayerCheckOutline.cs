using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static PlayerStateMashineHandle;

public class PlayerCheckOutline : MonoBehaviour
{
    private PlayerScript player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckOutLine();
    }

    private void CheckOutLine()
    {
        bool foundOutline = false;

        if (Physics.BoxCast(player.castingPosition.transform.position, transform.localScale / 2, player.castingPosition.transform.forward, out player.outLinehit, Quaternion.identity, player.grabReach))
        {
            foundOutline = CheckOutlineForHoldingNothing(player.outLinehit.collider.gameObject) ||
                           CheckOutlineForHoldingItem(player.outLinehit.collider.gameObject);
        }

        if (!foundOutline && Physics.Raycast(player.castingPosition.transform.position, player.castingPosition.transform.forward, out player.outLinehit, player.processReach))
        {
            foundOutline = CheckOutlineForProcess(player.outLinehit.collider.gameObject);
        }

        if (!foundOutline && Physics.SphereCast(player.castingPosition.transform.position, player.dragSphereRadius, player.castingPosition.transform.forward, out player.outLinehit, player.dragReach) && player.holdingState == PlayerStateMashineHandle.HoldingState.HoldingNothing)
        {
            foundOutline = CheckOutlineForDrag(player.outLinehit.collider.gameObject);
        }
    }


    bool CheckOutlineForHoldingNothing(GameObject hitObject)
    {
        if (player.holdingState != PlayerStateMashineHandle.HoldingState.HoldingNothing || !hitObject)
            return false;

        OutlineHandler outlineHandler = null;

        if (hitObject.TryGetComponent(out Item item))
        {
            outlineHandler = item.GetComponentInChildren<OutlineHandler>();
        } else if (hitObject.TryGetComponent(out ResourceBoxState resourceBoxState))
        {
            outlineHandler = resourceBoxState.GetComponentInChildren<OutlineHandler>();
        }
        // Check other conditions and assign the outline handler accordingly

        if (outlineHandler)
        {
            outlineHandler.ShowOutline(player.color, true);
            return true;
        }

        return false;
    }

    bool CheckOutlineForHoldingItem(GameObject hitObject)
    {
        if (player.holdingState != PlayerStateMashineHandle.HoldingState.HoldingItem || !hitObject)
            return false;

        OutlineHandler outlineHandler = null;

        if (hitObject.TryGetComponent(out CounterState counterState))
        {
            // Check and assign outlineHandler accordingly
        } else if (hitObject.TryGetComponent(out Workstation workstation))
        {
            outlineHandler = workstation.GetComponentInChildren<OutlineHandler>();
        }
        // Check other conditions and assign the outline handler accordingly

        if (outlineHandler)
        {
            outlineHandler.ShowOutline(player.color, true);
            return true;
        }

        return false;
    }

    bool CheckOutlineForProcess(GameObject hitObject)
    {
        if (player.holdingState != PlayerStateMashineHandle.HoldingState.HoldingNothing || !hitObject)
            return false;

        OutlineHandler outlineHandler = null;

        if (hitObject.TryGetComponent(out Saw saw))
        {
            saw.ShowSawOutlineIfOk(player, player.color, true);
            return true;
        }
        // Check other conditions and assign the outline handler accordingly

        return false;
    }

    bool CheckOutlineForDrag(GameObject hitObject)
    {
        if (player.holdingState != PlayerStateMashineHandle.HoldingState.HoldingNothing || !hitObject)
            return false;

        OutlineHandler outlineHandler = null;

        if (hitObject.TryGetComponent(out PlayerScript playerScript) && player.allowedToDragPlayers)
        {
            outlineHandler = playerScript.GetComponentInChildren<OutlineHandler>();
        } else if (hitObject.TryGetComponent(out Item item))
        {
            outlineHandler = item.GetComponentInChildren<OutlineHandler>();
        } else if (hitObject.TryGetComponent(out CounterState counterState) && counterState.storedItem != null)
        {
            outlineHandler = counterState.storedItem.GetComponentInChildren<OutlineHandler>();
        }

        if (outlineHandler)
        {
            outlineHandler.ShowOutline(player.color, false);
            return true;
        }

        return false;
    }
}
