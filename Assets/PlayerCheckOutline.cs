using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
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

        // BoxCast(player.castingPosition.transform.position, new Vector3(1, 1, 1), player.castingPosition.transform.forward, out player.outLinehit, Quaternion.identity, player.dragReach)
        // SphereCast(player.castingPosition.transform.position, player.dragSphereRadius, player.castingPosition.transform.forward, out player.outLinehit, player.dragReach)

        if (!foundOutline && Physics.BoxCast(player.castingPosition.transform.position, new Vector3(1, 1, 1), player.castingPosition.transform.forward, out player.outLinehit, Quaternion.identity, player.dragReach) && player.holdingState == PlayerStateMashineHandle.HoldingState.HoldingNothing)
        {
            foundOutline = CheckOutlineForDrag(player.outLinehit.collider.gameObject);
        }
    }


    private bool CheckOutlineForHoldingNothing(GameObject hitObject)
    {
        if (player.holdingState != PlayerStateMashineHandle.HoldingState.HoldingNothing || !hitObject)
            return false;

        OutlineHandler outlineHandler = null;

        if (hitObject.TryGetComponent(out Item item))
        {
            outlineHandler = item.GetComponentInChildren<OutlineHandler>();
        } 
        else if (hitObject.TryGetComponent(out ResourceBoxState resourceBoxState))
        {
            if (hitObject.GetComponent<CounterState>() && hitObject.GetComponent<CounterState>().storedItem == null
            ||  hitObject.GetComponent<ResourceBoxState>().GetResource() == Resource_Enum.Resource.Bottle )
            {
                outlineHandler = resourceBoxState.GetComponentInChildren<OutlineHandler>();
            }
        }
        else if(hitObject.TryGetComponent(out CounterState counterState) && hitObject.GetComponent<CounterState>().storedItem)
        {
            outlineHandler = counterState.GetComponentInChildren<OutlineHandler>();
        }
        else if (hitObject.TryGetComponent(out PlayerScript playerScript))
        {
            outlineHandler = playerScript.GetComponentInChildren<OutlineHandler>();
        }

        // Check other conditions and assign the outline handler accordingly

        if (outlineHandler)
        {
            outlineHandler.ShowOutline(player.GetPlayerColor(), true);
            return true;
        }

        return false;
    }

    private bool CheckOutlineForHoldingItem(GameObject hitObject)
    {
        if (player.holdingState != PlayerStateMashineHandle.HoldingState.HoldingItem || !hitObject)
            return false;

        OutlineHandler outlineHandler = null;

        // counter in some way
        if (hitObject.TryGetComponent(out CounterState counterState))
        {
            // Check and assign outlineHandler accordingly : FUCKOFF

            outlineHandler = counterState.GetComponentInChildren<OutlineHandler>();
        } 
        // workstation
        else if (hitObject.TryGetComponent(out Workstation workstation))
        {
            outlineHandler = workstation.GetComponentInChildren<OutlineHandler>();
        }
        // cauldron
        else if (hitObject.GetComponent<CauldronState>())
        {

            if (player.GetObjectInHands().GetComponent<Ingredient>() && hitObject.GetComponent<CauldronState>().GetIngredientCount() < 3
            || player.GetObjectInHands().GetComponent<Bottle>() && player.GetObjectInHands().GetComponent<Bottle>().IsEmpty())
            {
                hitObject.GetComponent<CauldronState>().SetUpAndGetCauldronOutline().ShowOutline(player.GetPlayerColor(), true);
            }
            else if (player.GetObjectInHands().GetComponent<Firewood>())
            {
                hitObject.GetComponent<CauldronState>().SetUpAndGetFireOutline().ShowOutline(player.GetPlayerColor(), true);
            }
        }

        // Check other conditions and assign the outline handler accordingly : FUCKOFF

        if (outlineHandler)
        {
            outlineHandler.ShowOutline(player.GetPlayerColor(), true);
            return true;
        }

        return false;
    }

    private bool CheckOutlineForProcess(GameObject hitObject)
    {
        if (player.holdingState != PlayerStateMashineHandle.HoldingState.HoldingNothing || !hitObject)
            return false;

        if (hitObject.TryGetComponent(out Saw saw))
        {
            saw.ShowSawOutlineIfOk(player, player.GetPlayerColor(), true);
            return true;
        }

        // Check other conditions and assign the outline handler accordingly : FUCKOFF

        return false;
    }

    private bool CheckOutlineForDrag(GameObject hitObject)
    {
        if (player.holdingState != PlayerStateMashineHandle.HoldingState.HoldingNothing || !hitObject)
            return false;

        OutlineHandler outlineHandler = null;

        if (hitObject.TryGetComponent(out PlayerScript playerScript) && player.allowedToDragPlayers)
        {
            outlineHandler = playerScript.GetComponentInChildren<OutlineHandler>();
        } 
        else if (hitObject.TryGetComponent(out Item item))
        {
            outlineHandler = item.GetComponentInChildren<OutlineHandler>();
        } 
        else if (hitObject.TryGetComponent(out CounterState counterState) && counterState.storedItem != null)
        {
            outlineHandler = counterState.storedItem.GetComponentInChildren<OutlineHandler>();
        }

        if (outlineHandler)
        {
            outlineHandler.ShowOutline(player.GetPlayerColor(), false);
            return true;
        }

        return false;
    }
}
