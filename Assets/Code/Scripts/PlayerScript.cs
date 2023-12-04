using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static PlayerStateMashineHandle;
using static Unity.VisualScripting.Member;

public class PlayerScript : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private float playerSpeed = 5.4f;
    private Vector3 moveDirection;
    private float movementSpeed;
    public float mass = 2f;

    [Header("Reach")]
    [SerializeField] private int grabReach = 1;
    [SerializeField] private int processReach = 1;
    [SerializeField] private int dragReach = 5;

    [Header("Player Type")]
    public PlayerType playerType;
    [SerializeField] private UnityEngine.Color color;

    [Header("Throw Player")]
    [SerializeField] float horizontalThrowForcePlayer = 4;
    [SerializeField] float verticalThrowForcePlayer = 4;

    [Header("Throw Item")]
    [SerializeField] float horizontalThrowForce = 4;
    [SerializeField] float verticalThrowForce = 4;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float deadZone = 0.1f;
    private bool hasForcedLook = false;

    [Header("PlayerController")]
    private CharacterController characterController;
    [SerializeField] private GameObject objectInHands;
    [SerializeField] private GameObject objectDragging;
    private Gamepad gamepad;

    [Header("Emote")]
    [SerializeField] private PopUpManager popUpManager;

    // ----------------------------------------------------------------

    [Header("States")]

    [SerializeField] private bool canHoldPlayerHoldingPlayer = false;
    [SerializeField] private PlayerStateMashineHandle.PlayerState playerState;
    [SerializeField] private PlayerStateMashineHandle.HoldingState holdingState;

    //[SerializeField] private bool beingHeld;
    //[SerializeField] private bool holding;
    //[SerializeField] private bool holdingPlayer;

    //[SerializeField] private bool interacting;

    //[SerializeField] private bool isBeingDragged;
    //[SerializeField] private bool dragging;
    //[SerializeField] public bool drag = false;

    [SerializeField] private Transform holdPosition;

    // --------------------------------------------------------------

    [Header("Audio")]
    [SerializeField] private AudioClip grabClip;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField] private AudioClip dragClip;
    [SerializeField] private AudioClip throwClip;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource footstepSource;

    private bool isInitialized = false;

    public enum PlayerType
    {
        PlayerOne, 
        PlayerTwo, 
        PlayerThree, 
        PlayerFour
    }

    public enum ControlVariant
    {
        HorizontalOne,
        HorizontalTwo,
        HorizontalThree,
        HorizontalFour,
        VerticalOne,
        VerticalTwo,
        VerticalThree,
        VerticalFour,
        PickUpOne,
        PickUpTwo,
        PickUpThree,
        PickUpFour,
        ThrowOne,
        ThrowTwo,
        ThrowThree,
        ThrowFour,
        DragOne,
        DragTwo, 
        DragThree,
        DragFour,
        ProcessOne,
        ProcessTwo,
        ProcessThree,
        ProcessFour
    }

    [Header("Console")]
    public ConsoleType consoleType = ConsoleType.Xbox;
    public enum ConsoleType
    {
        Xbox,
        ArcadeMachine,
        PlayStation, //idk
        KeyboardSolo //idk
    }


    [Header("Inputs")]
    private string horizontalName;
    private string verticalName;
    private string pickUpName;
    private string throwName;
    private string dragName;
    private string processName;
    public int playerIndex { get; private set; }

    [SerializeField] private Vector3 velocity;
    private float gravity = 9.8f;

    [Header("Counter")]
    [SerializeField] private CounterState currentCounter;

    [Header("RaycastStuff")]
    [SerializeField] private GameObject castingPosition;
    [SerializeField] private Transform dragToPosition;

    private RaycastHit hit;
    private RaycastHit dragHit;
    private RaycastHit outLinehit;

    [Header("Other")]
    [SerializeField] public bool waitingForGround;

    private GameObject currentProcessStation;

    private bool initialized = false;

    private Camera mainCamera;

    [Header("VFX")]
    [SerializeField] private GameObject walkVFX;
    private bool onlyPlayVFX = true;

    // Start is called before the first frame update
    void Start()
    {
        characterController = gameObject.AddComponent<CharacterController>();
        characterController.skinWidth = 0.25f;
        characterController.slopeLimit = 0.0f;
        characterController.stepOffset = 0.05f;

        mainCamera = Camera.main;

        if (!initialized)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        playerState = PlayerStateMashineHandle.PlayerState.None;
        holdingState = PlayerStateMashineHandle.HoldingState.HoldingNothing;

        gamepad = Gamepad.current;
        source = GetComponent<AudioSource>();
        GetComponent<Rigidbody>().drag = 1;

        // Get colors
        color = UnityEngine.Color.white;

        switch (playerType)
        {
            case PlayerType.PlayerOne:
                color = UnityEngine.Color.red;
                break;
            case PlayerType.PlayerTwo:
                color = UnityEngine.Color.blue;
                break;
            case PlayerType.PlayerThree:
                color = UnityEngine.Color.yellow;
                break;
            case PlayerType.PlayerFour:
                color = UnityEngine.Color.green;
                break;
            default:
                break;
        }

        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            Initialize();
        }

        CheckPlayerControls();
        Physics.Raycast(castingPosition.transform.position, castingPosition.transform.forward, out hit, Mathf.Infinity);

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        float horizontalInput = Input.GetAxisRaw(horizontalName);
        float verticalInput = Input.GetAxisRaw(verticalName);

        moveDirection = (cameraForward.normalized * verticalInput + cameraRight.normalized * horizontalInput);

        const float movementJoystickSensitivity = 1f;
        movementSpeed = moveDirection.magnitude * movementJoystickSensitivity;
        moveDirection = moveDirection.normalized * movementSpeed;

        // if allowed to move (state är none eller emoting)
        if (characterController.enabled && (playerState == PlayerStateMashineHandle.PlayerState.None || playerState == PlayerStateMashineHandle.PlayerState.Emoting))
        {
            
            characterController.Move(moveDirection * Time.deltaTime * playerSpeed);
            
            // if emoting
            if(playerState == PlayerStateMashineHandle.PlayerState.Emoting){
                playerState = PlayerStateMashineHandle.PlayerState.None;
            }
        }
        // movedirection or charactecotroller
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            if (onlyPlayVFX)
            {
                StartCoroutine(playWalkingPoof());
            }
            //walkVFX.GetComponent<ParticleSystem>().Play();
            
        }
        else
        { 
            walkVFX.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }

        if (Mathf.Abs(horizontalInput) > 0.4f || Mathf.Abs(verticalInput) > 0.4f)
        {
            footstepSource.UnPause();
        } else
        {
            footstepSource.Pause();
        }

        // Rotation
        if (!hasForcedLook)
        {
            if (Mathf.Abs(horizontalInput) > deadZone || Mathf.Abs(verticalInput) > deadZone)
            {
                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }

        // Add gravitation?????????

        // dont fall if being dragged
        if (playerState == PlayerStateMashineHandle.PlayerState.IsBeingDragged || characterController.isGrounded)
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        //Debug.Log("Velocity: " + velocity);

        if (characterController.enabled && playerState == PlayerState.None)
            characterController.Move(velocity * mass * Time.deltaTime);

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        //                     Input Mapping
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        // PickUp (A)
        if (Input.GetButtonDown(pickUpName))
        {
            PickUp();
        }

        // Throw (B)
        if (Input.GetButtonDown(throwName))
        {
            if(holdingState != HoldingState.HoldingNothing)
                Throw();
            else
                StartDragging();
        }

        // Dragging (Y)
        if(consoleType == ConsoleType.Xbox)
        {
            if (Input.GetButtonDown(dragName)) // (Y) på xbox
            {
                //StartDragging(); 

                //Emote
                Emote();
            }

        }
        else if(consoleType == ConsoleType.ArcadeMachine)
        {
            if (Input.GetButtonDown(processName)) // (X) på xbox
            {
                //StartDragging();

                // Emote
                Emote();
            }
        }

        // PROCESS (X)
        if (consoleType == ConsoleType.Xbox)
        {
            if (Input.GetButtonDown(processName)) // (X) på xbox
            {
                playerState = PlayerStateMashineHandle.PlayerState.Interacting;
                Process();
            }
            if (Input.GetButtonUp(processName)) // (X) på xbox
            {
                EndProcess();
            }
        }
        else if (consoleType == ConsoleType.ArcadeMachine)
        {
            if (Input.GetButtonDown(dragName)) // (Y) på xbox
            {
                playerState = PlayerStateMashineHandle.PlayerState.Interacting;
                Process();
            }

            if (Input.GetButtonUp(dragName)) // (Y) på xbox
            {
                EndProcess();
            }
        }


        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        // Outline

        CheckOutLine();
    }

 

    private IEnumerator playWalkingPoof()
    {
        onlyPlayVFX = false;
        yield return new WaitForSeconds(0.3f);
        walkVFX.GetComponent<ParticleSystem>().Play();
        onlyPlayVFX = true;
        yield break;
    }

    private void CheckOutLine()
    {
        bool foundOutline = false;

        // PICKUP/DROP CHECK
        if (Physics.BoxCast(castingPosition.transform.position, transform.localScale / 2, castingPosition.transform.forward, out outLinehit, Quaternion.identity, grabReach))
        {
            if (outLinehit.collider.gameObject)
            {
                GameObject hitObject = outLinehit.collider.gameObject;

                if (holdingState == PlayerStateMashineHandle.HoldingState.HoldingNothing)
                {
                    // ITEM
                    if (hitObject.GetComponent<Item>())
                    {
                        hitObject.GetComponent<Item>().GetComponentInChildren<Outline>().ShowOutline(color, true);
                        foundOutline = true;
                    }
                    // RESOURCE BOX
                    else if (hitObject.GetComponent<ResourceBoxState>() && hitObject.GetComponent<CounterState>() && hitObject.GetComponent<CounterState>().storedItem == null)
                    {
                        hitObject.GetComponent<ResourceBoxState>().GetComponentInChildren<Outline>().ShowOutline(color, true);
                        foundOutline = true;
                    }
                    // Resource box as in bottle box
                    else if (hitObject.GetComponent<ResourceBoxState>() && hitObject.GetComponent<ResourceBoxState>().GetResource() == Resource_Enum.Resource.Bottle)
                    {
                        hitObject.GetComponent<ResourceBoxState>().GetComponentInChildren<Outline>().ShowOutline(color, true);
                        foundOutline = true;
                    }
                    // KOLLAR PÅ EN COUNTER MED ETT ITEM PÅ => ITEM OUTLINE
                    else if (hitObject.GetComponent<CounterState>())
                    {
                        if (hitObject.GetComponent<CounterState>().storedItem)
                        {
                            hitObject.GetComponent<CounterState>().storedItem.GetComponentInChildren<Outline>().ShowOutline(color, true);
                            foundOutline = true;
                        } 
                    }
                    // PLAYER
                    else if (hitObject.GetComponent<PlayerScript>())
                    {
                        //Debug.Log("hit: " + hitObject);
                        hitObject.GetComponent<PlayerScript>().GetComponentInChildren<Outline>().ShowOutline(color, true);
                        foundOutline = true;
                    }
                }
                else if (holdingState == PlayerStateMashineHandle.HoldingState.HoldingItem)
                {
                    if (hitObject.GetComponent<CounterState>())
                    {
                        // if looking at goal with a bottle OR not looking at goal (but a counter still)
                        if ((hitObject.GetComponent<Goal>() && objectInHands.GetComponent<Bottle>()) || (!hitObject.GetComponent<Goal>()))
                        {
                            if (hitObject.GetComponent<CounterState>().GetComponentInChildren<Outline>())
                            {
                                hitObject.GetComponent<CounterState>().GetComponentInChildren<Outline>().ShowOutline(color, true);
                                foundOutline = true;
                            }
                            else
                            {
                                Debug.LogError(hitObject.gameObject + " doesnt have an outline attached to its mesh. FIX!");
                            }
                        }
                    }
                    else if (hitObject.GetComponent<Workstation>())
                    {
                        hitObject.GetComponent<Workstation>().GetComponentInChildren<Outline>().ShowOutline(color, true);
                        foundOutline = true;
                    }
                    // ADD INGREDIENT IN CAULDRON
                    else if (hitObject.GetComponent<CauldronState>() && objectInHands.GetComponent<Ingredient>() && hitObject.GetComponent<CauldronState>().GetIngredientCount() < 3)
                    {
                        hitObject.GetComponent<CauldronState>().SetUpAndGetCauldronOutline().ShowOutline(color, true);
                        foundOutline = true;
                    }
                    // TAKE POTION FROM CAULDRON INTO BOTTLE
                    else if (hitObject.GetComponent<CauldronState>() && objectInHands.GetComponent<Bottle>() && objectInHands.GetComponent<Bottle>().IsEmpty())
                    {
                        hitObject.GetComponent<CauldronState>().SetUpAndGetCauldronOutline().ShowOutline(color, true);
                        foundOutline = true;
                    }
                    // ADD FIREWOOD IN CAULDRON
                    else if (hitObject.GetComponent<CauldronState>() && objectInHands.GetComponent<Firewood>())
                    {
                        hitObject.GetComponent<CauldronState>().SetUpAndGetFireOutline().ShowOutline(color, true);
                        foundOutline = true;
                    }

                }
                else if (holdingState == PlayerStateMashineHandle.HoldingState.HoldingPlayer)
                {
                    // ska något ens outline:as om man håller player???
                }
            }
        }

        // PROCESS CHECK
        if (Physics.Raycast(castingPosition.transform.position, castingPosition.transform.forward, out outLinehit, processReach))
        {
            GameObject hitObject = outLinehit.collider.gameObject;

            if(holdingState == PlayerStateMashineHandle.HoldingState.HoldingNothing)
            {
                if (hitObject.GetComponent<Saw>())
                {
                    hitObject.GetComponent<Saw>().GetComponentInChildren<Outline>().ShowOutline(color, true);
                    foundOutline = true;
                }
                // om man kollar på workstation styrs längre upp för counterstate
            }
            else if(holdingState == PlayerStateMashineHandle.HoldingState.HoldingItem) // Är denna nödvändig??? counterstate fixar väll???
            {
                if (hitObject.GetComponent<Workstation>())
                {
                    hitObject.GetComponent<Workstation>().GetComponentInChildren<Outline>().ShowOutline(color, true);
                    foundOutline = true;
                }
            }
        }

        // DRAG CHECK //if holding nothing and hits something
        if (!foundOutline && Physics.Raycast(castingPosition.transform.position, castingPosition.transform.forward, out outLinehit, dragReach) && holdingState == PlayerStateMashineHandle.HoldingState.HoldingNothing)
        {
            GameObject hitObject = outLinehit.collider.gameObject;

            if (hitObject.GetComponent<PlayerScript>())
            {
                //Debug.Log("Hit outline: " + outLinehit.collider.gameObject);
                hitObject.GetComponent<PlayerScript>().GetComponentInChildren<Outline>().ShowOutline(color, false);
            }
            else if (hitObject.GetComponent<Item>())
            {
                hitObject.GetComponent<Item>().GetComponentInChildren<Outline>().ShowOutline(color, false);
            }
            else if (hitObject.GetComponent<CounterState>() && hitObject.GetComponent<CounterState>().storedItem != null)
            {
                //Debug.Log("Hit outline counter ");
                hitObject.GetComponent<CounterState>().storedItem.GetComponentInChildren<Outline>().ShowOutline(color, false);
            }
        }

    }

    private void FixedUpdate()
    {
        if (playerState == PlayerStateMashineHandle.PlayerState.Dragging)
        {
            Drag();
        }
    }

    public void Respawn(Transform spawnpoint)
    {
        characterController.enabled = false;
        transform.position = spawnpoint.position;
        characterController.enabled = true;
        velocity = new Vector3(0, 0, 0);
    }

    public void CheckPlayerControls()
    {
        switch (playerType)
        {
            case PlayerType.PlayerOne:
                horizontalName = nameof(ControlVariant.HorizontalOne);
                verticalName = nameof(ControlVariant.VerticalOne);
                pickUpName = nameof(ControlVariant.PickUpOne);
                throwName = nameof(ControlVariant.ThrowOne);
                dragName = nameof(ControlVariant.DragOne);
                processName = nameof(ControlVariant.ProcessOne);
                break;
            case PlayerType.PlayerTwo:
                horizontalName = nameof(ControlVariant.HorizontalTwo);
                verticalName = nameof(ControlVariant.VerticalTwo);
                pickUpName = nameof(ControlVariant.PickUpTwo);
                throwName = nameof(ControlVariant.ThrowTwo);
                dragName = nameof(ControlVariant.DragTwo);
                processName = nameof(ControlVariant.ProcessTwo);
                break;
            case PlayerType.PlayerThree:
                horizontalName = nameof(ControlVariant.HorizontalThree);
                verticalName = nameof(ControlVariant.VerticalThree);
                pickUpName = nameof(ControlVariant.PickUpThree);
                throwName = nameof(ControlVariant.ThrowThree);
                dragName = nameof(ControlVariant.DragThree);
                processName = nameof(ControlVariant.ProcessThree);
                break;
            case PlayerType.PlayerFour:
                horizontalName = nameof(ControlVariant.HorizontalFour);
                verticalName = nameof(ControlVariant.VerticalFour);
                pickUpName = nameof(ControlVariant.PickUpFour);
                throwName = nameof(ControlVariant.ThrowFour);
                dragName = nameof(ControlVariant.DragFour);
                processName = nameof(ControlVariant.ProcessFour);
                break;
            default:
                horizontalName = nameof(ControlVariant.HorizontalFour);
                verticalName = nameof(ControlVariant.VerticalFour);
                pickUpName = nameof(ControlVariant.PickUpFour);
                throwName = nameof(ControlVariant.ThrowFour);
                dragName = nameof(ControlVariant.DragFour);
                processName = nameof(ControlVariant.ProcessFour);
                break;
        }
    }

    public void Process() // (X)
    {
        Debug.Log("Process (X) Start");

        // if food in front of player && if player pressed chop - play animation, start progress idk
        //Debug.DrawRay(castingPosition.transform.position, castingPosition.transform.forward, Color.cyan, 5, true);

        if (Physics.Raycast(castingPosition.transform.position, castingPosition.transform.forward, out hit, processReach))
        {
            Debug.DrawRay(castingPosition.transform.position, castingPosition.transform.forward * hit.distance, UnityEngine.Color.red, 50, true);

            // DEN RAYCASTEN FUNGERAR INTE
            //Debug.Log("RayCast!");

            
            GameObject hitObject = hit.collider.gameObject;

            Debug.Log("hitObject: " + hitObject);

            if (holdingState == PlayerStateMashineHandle.HoldingState.HoldingNothing && playerState == PlayerStateMashineHandle.PlayerState.Interacting)
            {
                // sågen
                if (hitObject.GetComponent<Saw>())
                {
                    Debug.Log("Saw");

                    Saw saw = hitObject.GetComponent<Saw>();

                    saw.DoSawProcess(this);
                    currentProcessStation = saw.gameObject;
                    
                }
                else if (hitObject.GetComponent<SawingPlate>())
                {
                    Debug.Log("SawingPlate --> saw");

                    Saw saw = hitObject.GetComponent<SawingPlate>().GetSaw();
                    saw.DoSawProcess(this);
                    currentProcessStation = saw.gameObject;
                }
                else if (hitObject.GetComponent<Workstation>())
                {
                    
                    Workstation workstation = hitObject.GetComponent<Workstation>();

                    workstation.DoWorkProcess(this);
                    currentProcessStation = workstation.gameObject;
                }

            }
        }

    }

    public void EndProcess()
    {
        Debug.Log("Process (X) End");

        playerState = PlayerStateMashineHandle.PlayerState.None;

        if (currentProcessStation && currentProcessStation.GetComponent<Saw>())
        {
            currentProcessStation.GetComponent<Saw>().StopSawProcess(this);
        }
        else if (currentProcessStation && currentProcessStation.GetComponent<Workstation>())
        {
            Workstation workstation = currentProcessStation.GetComponent<Workstation>();

            workstation.StopWorkProcess(this);
        }

        currentProcessStation = null;
    }

    public void PickUp() // (A)
    {
        // if object in front of player && not holding- set object as child of player?
        if (Physics.BoxCast(castingPosition.transform.position, transform.localScale / 2, castingPosition.transform.forward, out hit, Quaternion.identity, grabReach))
        {
            //Debug.DrawRay(castingPosition.transform.position, castingPosition.transform.forward * hit.distance, Color.yellow, 5, true);
            GameObject hitObject = hit.collider.gameObject;

            Debug.Log("hitObject: " + hitObject);

            if (holdingState == PlayerStateMashineHandle.HoldingState.HoldingNothing)
            {
                // stuff
                //Debug.Log("Nothing in hand");
                if (hitObject.GetComponent<Item>())
                {
                    //Debug.Log("Try to grab Item");
                    Grab(hitObject.GetComponent<Item>());
                }
                else if (hitObject.GetComponent<ResourceBoxState>() && hitObject.GetComponent<CounterState>() && hitObject.GetComponent<CounterState>().storedItem == null)
                {
                    //Debug.Log("hit ResourceBox");
                    ResourceBoxHandler resourceBoxHandler = hitObject.GetComponent<ResourceBoxHandler>();

                    resourceBoxHandler.GetResource(this);
                }
                // bottle box
                else if (hitObject.GetComponent<ResourceBoxState>() && hitObject.GetComponent<ResourceBoxState>().GetResource() == Resource_Enum.Resource.Bottle)
                {
                    //Debug.Log("hit BottleBox");
                    ResourceBoxHandler resourceBoxHandler = hitObject.GetComponent<ResourceBoxHandler>();

                    resourceBoxHandler.GetResource(this);
                }
                else if (hitObject.GetComponent<CounterState>())
                {
                    //Debug.Log("Hit counter, not holding");
                    currentCounter = hitObject.GetComponent<CounterState>();
                    GrabFromCounter();
                }
                //player
                else if (hitObject.GetComponent<PlayerScript>())
                {
                    //Debug.Log("Try to grab Player");
                    
                    GrabPlayer(hitObject.GetComponent<PlayerScript>());

                }

            }
            else if (holdingState != HoldingState.HoldingNothing)
            {
                if(holdingState == HoldingState.HoldingItem)
                {
                    if (hitObject.GetComponent<CounterState>())
                    {
                        // if looking at goal with a bottle OR not looking at goal (but a counter still)
                        if ((hitObject.GetComponent<Goal>() && objectInHands.GetComponent<Bottle>()) || (!hitObject.GetComponent<Goal>()))
                        {
                            //Debug.Log("hit counter, holding");
                            currentCounter = hitObject.GetComponent<CounterState>();
                            PlaceOnCounter();
                        }
                    }
                    else if (hitObject.GetComponent<CauldronState>())
                    {
                        //Debug.Log("hit Cauldron");

                        if (objectInHands.GetComponent<Ingredient>())
                        {
                            CauldronState cauldronState = hitObject.GetComponent<CauldronState>();

                            bool result = cauldronState.AddIngredient(objectInHands.GetComponent<Ingredient>());
                            if (result)
                            {
                                Destroy(objectInHands);
                                holdingState = HoldingState.HoldingNothing;
                            }
                        }
                        else if (objectInHands.GetComponent<Bottle>())
                        {
                            CauldronState cauldronState = hitObject.GetComponent<CauldronState>();

                            Potion cauldronPotion = cauldronState.GetPotion();
                            if (cauldronPotion == null)
                            {
                                return;
                            }

                            if (objectInHands.GetComponent<Bottle>().IsEmpty())
                            {
                                objectInHands.GetComponent<Bottle>().SetPotion(cauldronPotion);
                            }
                        }
                        else if (objectInHands.GetComponent<Item>().GetItemType() == Resource_Enum.Resource.FireWood)
                        {
                            FireState fireState = hitObject.GetComponent<FireState>();
                            fireState.AddWood();
                            Destroy(objectInHands);
                            holdingState = HoldingState.HoldingNothing;
                        }
                        else
                        {
                            Drop(false);
                        }
                    }
                }
                else if (holdingState == HoldingState.HoldingPlayer)
                {
                    DropPlayer(false);
                }
            }
        }
        else
        {
            //Debug.Log("hit nothing");

            if (holdingState != HoldingState.HoldingNothing)
            {
                if (holdingState == HoldingState.HoldingPlayer)
                {
                    DropPlayer(false);
                }
                else
                {
                    Drop(false);
                }
            }
        }
    }

    // Remove from release version, will perma draw on screen xd
    // Why not just debug.draw? well because there is no draw box.
    // This shit exists only in Gizmo.DrawCube. And to invoke that method - i HAVE to invoke it in OnDrawGizmo xdddd
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.green;
        Gizmos.DrawRay(castingPosition.transform.position, transform.forward * hit.distance);
        Gizmos.DrawWireCube(castingPosition.transform.position + transform.forward * hit.distance, transform.localScale);
    }

    public void Grab(Item item)
    {
        Debug.Log("Grabbing");

        holdingState = HoldingState.HoldingItem;

        objectInHands = item.gameObject;
        objectInHands.GetComponent<Rigidbody>().isKinematic = true;
        objectInHands.transform.parent = transform;

        if (!item.GetComponent<Firewood>())
        {
            objectInHands.transform.rotation = Quaternion.Euler(objectInHands.GetComponent<Item>().originalRotation.eulerAngles.x, // X
                                                                transform.rotation.eulerAngles.y + 90f,                             // Y
                                                                objectInHands.GetComponent<Item>().originalRotation.eulerAngles.z); // Z
        }
        else
        {
            objectInHands.transform.rotation = holdPosition.rotation;
        }
        
        objectInHands.transform.position = holdPosition.position;
        objectInHands.GetComponent<Item>().SetIsPickedUp(true);
        objectInHands.GetComponent<Item>().lastHeldPlayer = this;
        source.PlayOneShot(dropClip);

    }

    public void GrabPlayer(PlayerScript player)
    {
        if (canHoldPlayerHoldingPlayer || player.GetHoldingState() != HoldingState.HoldingPlayer)
        {
            //Debug.Log("Grabbing player");

            holdingState = HoldingState.HoldingPlayer;

            objectInHands = player.gameObject;
            objectInHands.GetComponent<Rigidbody>().isKinematic = true; // could maybe be removed
            player.GetCharacterController().enabled = false;

            objectInHands.transform.parent = transform;
            objectInHands.transform.rotation = transform.rotation;
            objectInHands.transform.position = holdPosition.position;

            objectInHands.GetComponent<PlayerScript>().SetPlayerState(PlayerState.IsBeingHeld);

            source.PlayOneShot(grabClip);
        }
    }

    /// <summary>
    /// Player will drop/release/deattach an item from itself. Parameter isThrowing "true" if the de-attachment is happening because of throwing the item.
    /// If Holding a player, it will automatically go to "DropPlayer".
    /// </summary>
    /// <param name="isThrowing">Is the item being thrown</param>
    public void Drop(bool isThrowing)
    {
        // if holding true - get holding object reference and set parent to null.
        if (holdingState == HoldingState.HoldingItem)
        {
            if (!isThrowing)
            {
                objectInHands.GetComponent<Item>().SetIsPickedUp(false);
            }

            objectInHands.transform.parent = null;
            objectInHands.GetComponent<Rigidbody>().isKinematic = false;

            holdingState = HoldingState.HoldingNothing;
            source.PlayOneShot(dropClip);

            objectInHands = null;
            //Debug.Log("object in hands should be null: " + objectInHands);
        }
        else
        {
            DropPlayer(false);
        }
    }

    /// <summary>
    /// Player will drop/release/deattach player from itself. Parameter isThrowing "true" if the de-attachment is happening because of throwing the player.
    /// </summary>
    /// <param name="isThrowing">Is the player being thrown</param>
    public void DropPlayer(bool isThrowing)
    {
        // if holding true - get holding object reference and set parent to null.
        if (holdingState == HoldingState.HoldingPlayer)
        {
            source.PlayOneShot(dropClip);

            if (isThrowing)
            {
                objectInHands.GetComponent<PlayerScript>().waitingForGround = true;
            }
            else
            {
                objectInHands.GetComponent<PlayerScript>().GetDropped();
            }

            objectInHands.transform.parent = null;

            holdingState = HoldingState.HoldingNothing;
            playerState = PlayerState.None;

            objectInHands = null;
        }
    }

    public void GetDropped()
    {
        GetCharacterController().enabled = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = true; // could maybe be removed
        gameObject.transform.parent = null;

        playerState = PlayerState.None;

        waitingForGround = false;
        
        ParticleSystem ps = walkVFX.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        UnityEngine.Color newColor;
        UnityEngine.ColorUtility.TryParseHtmlString("#BFBEBE", out newColor);
        main.startColor = newColor;//System.Drawing.Color.FromArgb(0.7450981, 0.7450981, 0.7450981);
        

        GameObject poofVFX = Instantiate(walkVFX, transform.position + new Vector3(0,-1,0), Quaternion.identity);
        poofVFX.transform.localScale = new Vector3(3,3,3);
        
        Destroy(poofVFX, 0.6f);
    }

    public void StartDragging() // (Y)
    {
        if (playerState != PlayerState.IsBeingDragged)
        {
            if (holdingState == HoldingState.HoldingNothing)
            {
                if (Physics.Raycast(castingPosition.transform.position, castingPosition.transform.forward, out dragHit, dragReach))
                {
                    if (dragHit.collider.gameObject.GetComponent<Item>() || dragHit.collider.gameObject.GetComponent<PlayerScript>()) 
                    {
                        playerState = PlayerState.Dragging;

                        objectDragging = dragHit.collider.gameObject;
                    }
                    else if(dragHit.collider.gameObject.GetComponent<CounterState>() && dragHit.collider.gameObject.GetComponent<CounterState>().storedItem != null)
                    {
                        playerState = PlayerState.Dragging;

                        objectDragging = dragHit.collider.gameObject.GetComponent<CounterState>().storedItem;
                        dragHit.collider.gameObject.GetComponent<CounterState>().ReleaseItem(objectDragging);

                    }
                }
            }
        }
    }

    public void Drag()
    {
        if(holdingState == HoldingState.HoldingNothing)
        {
            if (playerState == PlayerState.Dragging && objectDragging && objectDragging.GetComponent<Item>())
            {
                // special for item
                objectDragging.GetComponent<Rigidbody>().isKinematic = false;
                objectDragging.GetComponent<Item>().SetIsBeingDragged(true);

                // maybe replace MoveTowards with Lerp
                //objectDragging.GetComponent<Rigidbody>().AddForce(transform.up * 2, ForceMode.Force);

                objectDragging.transform.position = Vector3.MoveTowards(objectDragging.transform.position, dragToPosition.transform.position, 0.5f);
                objectDragging.transform.position.Set(objectDragging.transform.position.x, objectDragging.transform.position.y, objectDragging.transform.position.z);

                // sound
                //if (!source.isPlaying)
                //{
                    source.PlayOneShot(dragClip);
                //}

                // pickUp if close
                //Debug.Log("Grab Boxcast!");
                if (Physics.BoxCast(castingPosition.transform.position, transform.localScale / 2, castingPosition.transform.forward, out hit, Quaternion.identity, grabReach))
                {
                    //Debug.Log("Hit Something");
                    //Debug.Log("hit: " + hit.collider.gameObject + " - object dragging: " + objectDragging);
                    if (hit.collider.gameObject && hit.collider.gameObject == objectDragging)
                    {
                        //Debug.Log("TRy to grab item!");
                        objectDragging.GetComponent<Item>().SetIsBeingDragged(false);
                        Grab(objectDragging.GetComponent<Item>());

                        objectDragging = null;
                    }
                }
                
            }
            else if (playerState == PlayerState.Dragging && objectDragging && objectDragging.GetComponent<PlayerScript>())
            {

                // special for player
                objectDragging.GetComponent<Rigidbody>().isKinematic = true;
                objectDragging.GetComponent<PlayerScript>().SetPlayerState(PlayerState.IsBeingDragged);

                // maybe replace MoveTowards with Lerp
                objectDragging.transform.position = Vector3.MoveTowards(objectDragging.transform.position, holdPosition.position, 0.5f);
                objectDragging.transform.position += new Vector3(0, 0.0004f, 0);

                // sound
                //if (!source.isPlaying)
                //{
                    source.PlayOneShot(dragClip);
                //}

                // pickUp if close
                if (Physics.BoxCast(castingPosition.transform.position, transform.localScale / 2, castingPosition.transform.forward, out hit, Quaternion.identity, grabReach))
                {
                    if (hit.collider.gameObject && hit.collider.gameObject == dragHit.collider.gameObject)
                    {
                        objectDragging = null;
                        PickUp();
                    }
                    else
                    {
                        DropPlayer(false);
                    }
                }
                
            }
            else
            {
                playerState = PlayerState.None;
            }
        }
        else if(playerState == PlayerState.Dragging) // ?????????
        {
            playerState = PlayerState.None;
        }
        
    }

    private void ResetDrag()
    {
        if (objectDragging != null)
        {
            if (objectDragging.GetComponent<Item>())
            {
                Drop(false);
                objectDragging = null;
            }
            else if (objectDragging.GetComponent<PlayerScript>())
            {
                DropPlayer(false);

                objectDragging.GetComponent<PlayerScript>().SetPlayerState(PlayerState.None);
                objectDragging = null;
            }
        }
        
    }

    public void Throw()
    {
        // if holding then button press down, start animation (lift hands) 
        if (holdingState != HoldingState.HoldingNothing)
        {
            if (holdingState == HoldingState.HoldingPlayer)
            {
                objectInHands.GetComponent<PlayerScript>().gameObject.transform.parent = null;
                objectInHands.GetComponent<Rigidbody>().isKinematic = false;
                objectInHands.GetComponent<Rigidbody>().AddForce((transform.forward * horizontalThrowForcePlayer) + (transform.up * verticalThrowForcePlayer), ForceMode.Impulse);
                
                objectInHands.GetComponent<PlayerScript>().waitingForGround = true;

                //holdingState = HoldingState.HoldingNothing;

                //Debug.Log("Throw Player");

                //if (!source.isPlaying)
                //{
                    source.PlayOneShot(throwClip);
                //}

                // HERE
                DropPlayer(true);
            }
            else
            {
                objectInHands.GetComponent<Rigidbody>().isKinematic = false;
                objectInHands.GetComponent<Rigidbody>().AddForce((transform.forward * horizontalThrowForce) + (transform.up * verticalThrowForce), ForceMode.Impulse);
                objectInHands.GetComponent<Item>().SetIsBeingTrown(true);

                //Debug.Log("Throw Item");

                //if (!source.isPlaying)
                //{
                    source.PlayOneShot(throwClip);
                //}

                Drop(true);
            }
        }
    }

    private void Emote()
    {
        popUpManager.SpawnPopUp(mainCamera, transform, "slay", color);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CounterState>())
        {
            //why not triggering???
            //Debug.Log("fixez");
            GetComponent<Rigidbody>().AddForce((transform.forward * -1) * 5, ForceMode.Impulse);
        }
        //Debug.Log("TOUCHED GROUND");
        if (!collision.collider.GetComponent<PlayerScript>())
        {
            //Debug.Log("TOUCHED NOT FLOOR");
            if (waitingForGround)
            {
                GetDropped();
            }
        }
    }     
                                   

    public void InitializePlayer(int playerIndex)
    {
        isInitialized = true;
        this.playerIndex = playerIndex;
        // Customize player appearance, controls, etc.
    }

    void PlaceOnCounter()
    {
        if (currentCounter != null)
        {
            //Debug.Log("current counter isnt null");

            if (holdingState != HoldingState.HoldingNothing && !objectInHands.GetComponent<PlayerScript>())
            {
                // Attempt to place the item on the counter
                //Debug.Log("attempted to place item on counter: " + currentCounter);
                bool result = currentCounter.PlaceItem(objectInHands);

                //Debug.Log("PlaceItem result:" + result);

                if (result)
                {
                    //Debug.Log("FAAAALSEE");
                    objectInHands = null;
                    holdingState = HoldingState.HoldingNothing;
                    source.PlayOneShot(dropClip);
                }

            }
            else
            {
                // Attempt to pick up an item from the counter
                GameObject pickedUpItem = currentCounter.PickUpItem();
                if (pickedUpItem != null)
                {
                    Grab(pickedUpItem.GetComponent<Item>());
                }
            }
        }
    }

    void GrabFromCounter()
    {
        // Attempt to pick up an item from the counter
        GameObject pickedUpItem = currentCounter.PickUpItem();
        if (pickedUpItem != null)
        {
            Grab(pickedUpItem.GetComponent<Item>());
            source.PlayOneShot(grabClip);
        }
    }

    public CharacterController GetCharacterController()
    {
        return characterController;
    }

    public GameObject GetObjectInHands()
    {
        return objectInHands;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }
    public float GetMovementSpeed()
    {
        return movementSpeed;
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public void SetHasForcedLook(bool forceLook)
    {
        hasForcedLook = forceLook;
    }
    public void StartFootSteps()
    {
        footstepSource.Play();
    }

    public PlayerState GetPlayerState()
    {
        return playerState;
    }
    public void SetPlayerState(PlayerState newState)
    {
        playerState = newState;
    }
    public HoldingState GetHoldingState()
    {
        return holdingState;
    }
    public void SetPlayerState(HoldingState newState)
    {
        holdingState = newState;
    }

}
