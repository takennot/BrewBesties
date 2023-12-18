using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static PlayerStateMashineHandle;
using static Unity.VisualScripting.Member;

public class PlayerScript : MonoBehaviour
{
    [Header("Gameplay")]
    public float playerSpeed = 5.4f;
    public bool isSlippery = false; 
    public float accelerationRate = 0.4f;
    private float currentVerticalInput;
    private float currentHorizontalInput;
    public bool allowedToDragPlayers = true;

    [SerializeField] private Vector3 moveDirection;
    private float movementSpeed;
    public float mass = 2f;
    private bool isInFence = false;

    [Header("Reach")]
    public float grabReach = 1;
    public float processReach = 0.8f;
    public float dragReach = 5.5f;
    public float dragSphereRadius = 1f;

    [Header("Player Type")]
    public PlayerType playerType;
    private UnityEngine.Color color;

    [Header("Throw Player")]
    public float horizontalThrowForcePlayer = 4;
    public float verticalThrowForcePlayer = 4;

    [Header("Throw Item")]
    public float horizontalThrowForce = 4;
    public float verticalThrowForce = 4;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float deadZone = 0.1f;
    private bool hasForcedLook = false;

    [Header("PlayerController")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private CollidingTriggerCounting groundCheckColliderCounter;
    public bool isGrounded = false;

    [SerializeField] private GameObject objectInHands;
    [SerializeField] private GameObject objectDragging;
    private Gamepad gamepad;

    [Header("Drag")]
    [SerializeField] private CollidingTriggerCounting dragGrabTriggerCount;

    [Header("Emote")]
    [SerializeField] private PopUpManager popUpManager;

    // ----------------------------------------------------------------

    [Header("States")]

    [SerializeField] private bool canHoldPlayerHoldingPlayer = false;
    [SerializeField] private PlayerStateMashineHandle.PlayerState playerState;
    public PlayerStateMashineHandle.HoldingState holdingState;
    [SerializeField] private bool moving = false;

    [Header("Animation")]
    [SerializeField] private Animator animatorPlayer;

    // --------------------------------------------------------------
    private PlayerAudio audio;

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
    public GameObject castingPosition;
    [SerializeField] private Transform dragToPosition;
    [SerializeField] private Transform holdPosition;

    private RaycastHit hit;
    private RaycastHit dragHit;
    [HideInInspector] public RaycastHit outLinehit;

    //public Vector3 boxCastWidth = new Vector3(1, 1, 1);

    [Header("Other")]
    [SerializeField] public bool waitingForGround;

    private GameObject currentProcessStation;

    private Camera mainCamera;

    [Header("VFX")]
    [SerializeField] private GameObject walkVFX;
    private bool onlyPlayVFX = true;

    [SerializeField] GameObject dragEffekt;
    [SerializeField] LineRenderer dragline;
    GameObject lineEffekt;
    [SerializeField] GameObject dragStart;
    [SerializeField] GameObject[] dragObejct;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        Initialize();
    }

    //private void InitCC()
    //{
    //    characterController = gameObject.AddComponent<CharacterController>();
    //    characterController.skinWidth = 0.25f;
    //    characterController.slopeLimit = 0.0f;
    //    characterController.stepOffset = 0.025f;
    //}

    public void InitializePlayer(int playerIndex)
    {
        Initialize();
        this.playerIndex = playerIndex;

        // Customize player appearance, controls, etc.
    }

    private void Initialize()
    {
        //InitCC();

        playerState = PlayerStateMashineHandle.PlayerState.None;
        holdingState = PlayerStateMashineHandle.HoldingState.HoldingNothing;

        gamepad = Gamepad.current;
        audio = GetComponent<PlayerAudio>();
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

        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("TIMEE PLAYERR");

        if (!isInitialized)
        {
            Initialize();
        }

        if(playerState == PlayerState.IsBeingHeld || playerState == PlayerState.IsBeingThrown)
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerThrown");
        }

        if(!(playerState == PlayerState.IsBeingHeld) && !(playerState == PlayerState.IsBeingThrown) && !isInFence)
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }


        if(objectInHands == null && holdingState != HoldingState.HoldingNothing)
        {
            holdingState = HoldingState.HoldingNothing;
        }

        //Debug.Log((int)playerState + " | " + (int)holdingState);

        // ground stuff
        if (waitingForGround)
        {
            playerState = PlayerState.IsBeingThrown;
        }

        if (groundCheckColliderCounter.GetGameobjectsCollidingWith().Count > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded= false;
        }

        // Set Up Animator
        animatorPlayer.SetInteger("PlayerState", (int)playerState);
        animatorPlayer.SetInteger("PlayerHoldingState", (int)holdingState);
        animatorPlayer.SetBool("Moving", moving);

        if (!characterController.enabled) return;

        if(playerState != PlayerState.Interacting)
        {
            Movement();
        }

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
                playerState = PlayerState.Interacting;
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
                playerState = PlayerState.Interacting;
                Process();
            }

            if (Input.GetButtonUp(dragName)) // (Y) på xbox
            {
                EndProcess();
            }
        }


        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        // Outline

        //CheckOutLine();
    }

 
    private void Movement()
    {
        //Debug.Log("MOVEEEE");
        CheckPlayerControls();

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        float horizontalInput = Input.GetAxisRaw(horizontalName);
        float verticalInput = Input.GetAxisRaw(verticalName);

        float targetVerticalInput = verticalInput;
        float targetHorizontalInput = horizontalInput;

        if (isSlippery)
        {
            currentVerticalInput = Mathf.Lerp(currentVerticalInput, targetVerticalInput, Time.deltaTime * accelerationRate);
            currentHorizontalInput = Mathf.Lerp(currentHorizontalInput, targetHorizontalInput, Time.deltaTime * accelerationRate);

            //NEEDS TO NOT LOSE ALL CURRENT INPUT DIRECTOIN WHEN THE VELOCITY OF THAT DIRECTION IS 0 BECAUSE TOUCHING A WALL
            //would casting backwards of player character direction to see if there is a wall within 0.05 distance be a good solution? probably dumb?
        } else
        {
            currentVerticalInput = targetVerticalInput;
            currentHorizontalInput = targetHorizontalInput;
        }

        moveDirection = (cameraForward.normalized * currentVerticalInput + cameraRight.normalized * currentHorizontalInput);

        const float movementJoystickSensitivity = 1f;
        movementSpeed = moveDirection.magnitude * movementJoystickSensitivity;
        moveDirection = moveDirection.normalized * movementSpeed;

        // Gravitation --------------------

        // dont fall if being dragged
        if (playerState == PlayerState.IsBeingDragged || characterController.isGrounded)
        {
            velocity.y = 0;
        } else
        {
            velocity.y -= gravity * Time.deltaTime;
            if(velocity.y < -1)
            {
                velocity.y = -1;
            }
        }

        //Debug.Log("Velocity: " + velocity);

        if (characterController.enabled && (playerState == PlayerState.None || playerState == PlayerState.Dead))
            characterController.Move(velocity * mass * Time.deltaTime);

        // ---------------------------------

        if (playerState == PlayerState.Dead)
        {
            audio.PlayFootstep(false);
            return;
        }

        // if allowed to move (state är none eller emoting)
        if (characterController.enabled && (playerState == PlayerState.None || playerState == PlayerState.Emoting))
        {
            characterController.Move(moveDirection * Time.deltaTime * playerSpeed);
        }

        if (isSlippery)
        {
            //Debug.Log("Move: " + (moveDirection.x > 0.1f) + (moveDirection.z > 0.1f) + (velocity.x > 0.1f) + (velocity.z > 0.1f));

            if (moveDirection.x > 0.1f || moveDirection.z > 0.1f || moveDirection.x < -0.2f|| moveDirection.z < -0.2f )
            {
                moving = true;

                if (playerState == PlayerState.Emoting)
                    playerState = PlayerState.None;

                if (onlyPlayVFX)
                {
                    StartCoroutine(PlayWalkingPoof());
                }
            }
            else
            {
                moving = false;
                walkVFX.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        }
        else
        {
            if (moveDirection.x != 0.0f || moveDirection.z != 0.0f)
            {
                moving = true;

                if (playerState == PlayerState.Emoting)
                    playerState = PlayerState.None;

                if (onlyPlayVFX)
                {
                    StartCoroutine(PlayWalkingPoof());
                }

            }
            else
            {
                moving = false;
                walkVFX.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }

        }

        if (Mathf.Abs(targetHorizontalInput) > 0.4f || Mathf.Abs(targetVerticalInput) > 0.4f)
        {
            audio.PlayFootstep(true);
        } else
        {
            audio.PlayFootstep(false);
        }

        // Rotation
        if (!hasForcedLook)
        {
            if (Mathf.Abs(targetHorizontalInput) > deadZone || Mathf.Abs(targetVerticalInput) > deadZone)
            {
                if (moveDirection != Vector3.zero)
                {
                    if(isSlippery)
                    {
                        Vector3 joystickDirection = new Vector3(targetHorizontalInput, 0f, targetVerticalInput);
                        Quaternion cameraRotation = Quaternion.Euler(0f, mainCamera.transform.rotation.eulerAngles.y, 0f);
                        Vector3 combinedDirection = cameraRotation * joystickDirection;
                        Quaternion targetRotation = Quaternion.LookRotation(combinedDirection, Vector3.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    } else
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    }

                }
            }
        }
    }

    private IEnumerator PlayWalkingPoof()
    {
        onlyPlayVFX = false;
        yield return new WaitForSeconds(0.3f);
        walkVFX.GetComponent<ParticleSystem>().Play();
        onlyPlayVFX = true;
        yield break;
    }

    private void FixedUpdate()
    {
        if (playerState == PlayerState.Dragging)
        {
            Drag();
        }
    }

    public void Die()
    {
        playerState = PlayerState.Dead;
    }

    public void Respawn(Transform spawnpoint)
    {
        playerState = PlayerState.None;

        Debug.Log("cc" + characterController);

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

        if (Physics.BoxCast(castingPosition.transform.position, transform.localScale / 2, castingPosition.transform.forward, out hit, Quaternion.identity, processReach))
        {
            Debug.DrawRay(castingPosition.transform.position, castingPosition.transform.forward * hit.distance, UnityEngine.Color.red, 50, true);

            // DEN RAYCASTEN FUNGERAR INTE
            //Debug.Log("RayCast!");
                        
            GameObject hitObject = hit.collider.gameObject;

            Debug.Log("hitObject: " + hitObject);

            if (holdingState == HoldingState.HoldingNothing && playerState == PlayerState.Interacting)
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

        playerState = PlayerState.None;

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
                            Debug.Log("hit counter, holding");
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
                    else
                    {
                        Drop(false);
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
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.green;
        //this logic is flawed 
        //Gizmos.DrawRay(castingPosition.transform.position, transform.forward * dragHit.distance);
        Gizmos.DrawWireSphere(hit.point, dragSphereRadius);
    }

    public void Grab(Item item)
    {
        if(holdingState != HoldingState.HoldingNothing)
        {
            return;
        }

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
        audio.PlayDrop();

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

            audio.PlayGrab();
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

            //objectInHands.transform.position = objectInHands.transform.position + (this.transform.forward * 5);
            objectInHands.transform.parent = null;
            objectInHands.GetComponent<Rigidbody>().isKinematic = false;

            holdingState = HoldingState.HoldingNothing;
            audio.PlayDrop();

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
            audio.PlayDrop();

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
        playerState = PlayerState.None;

        // wind vfx
        ParticleSystem ps = walkVFX.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        UnityEngine.Color newColor;
        UnityEngine.ColorUtility.TryParseHtmlString("#BFBEBE", out newColor);
        main.startColor = newColor;//System.Drawing.Color.FromArgb(0.7450981, 0.7450981, 0.7450981);
        
        // poof vfx
        GameObject poofVFX = Instantiate(walkVFX, transform.position + new Vector3(0,-1,0), Quaternion.identity);
        poofVFX.transform.localScale = new Vector3(3,3,3);
        
        Destroy(poofVFX, 0.6f);
    }

    public void StartDragging()
    {
        if (playerState == PlayerState.IsBeingDragged || holdingState != HoldingState.HoldingNothing)
            return;

        if (Physics.BoxCast(castingPosition.transform.position, transform.localScale * 1.2f, castingPosition.transform.forward, out dragHit, Quaternion.identity, dragReach))
        {
            Debug.Log("Drag " + dragHit.collider.gameObject);
            GameObject hitObject = dragHit.collider.gameObject;

            if (hitObject.TryGetComponent(out Item item) || (allowedToDragPlayers && hitObject.TryGetComponent(out PlayerScript playerScript)))
            {
                playerState = PlayerState.Dragging;
                objectDragging = hitObject;

                audio.PlayDrag();
                CreateDragEffects();
            } 
            else if (hitObject.TryGetComponent(out CounterState counterState) && counterState.storedItem != null)
            {
                Debug.Log("Drag found counterstate");
                if (!counterState.GetComponent<Trashcan>())
                {
                    playerState = PlayerState.Dragging;
                    objectDragging = counterState.storedItem;
                    counterState.ReleaseItem(objectDragging);

                    audio.PlayDrag();
                    CreateDragEffects();
                }
            }
            else
            {
                Debug.Log("Drag didnt find shit");
            }
        }
    }

    void CreateDragEffects()
    {
        GameObject start = Instantiate(dragStart, holdPosition);
        Destroy(start, 0.6f);

        GameObject hitEffect = Instantiate(objectDragging.GetComponent<PlayerScript>() ? dragObejct[0] : dragObejct[1], objectDragging.transform);
        Destroy(hitEffect, 0.6f);

        Destroy(lineEffekt);
        lineEffekt = Instantiate(dragEffekt);
        lineEffekt.transform.position = Vector3.zero;
        dragline = lineEffekt.GetComponentInChildren<LineRenderer>();
        dragline.SetPosition(0, holdPosition.position);
        dragline.SetPosition(1, objectDragging.transform.position);
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
                objectDragging.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                dragline.SetPosition(1, objectDragging.gameObject.transform.position);

                objectDragging.transform.position = Vector3.MoveTowards(objectDragging.transform.position, dragToPosition.transform.position, 0.5f);
                objectDragging.transform.position.Set(objectDragging.transform.position.x, objectDragging.transform.position.y, objectDragging.transform.position.z);

                // USING COLLISION
                if (dragGrabTriggerCount.GetGameobjectsCollidingWith().Contains(objectDragging.gameObject))
                {
                    Debug.Log("TRy to grab item!");

                    objectDragging.GetComponent<Item>().SetIsBeingDragged(false);
                    Grab(objectDragging.GetComponent<Item>());
                    Destroy(lineEffekt);

                    objectDragging = null;
                }

                audio.PlayDrag();

            }
            else if (playerState == PlayerState.Dragging && objectDragging && objectDragging.GetComponent<PlayerScript>())
            {

                // special for player
                objectDragging.GetComponent<Rigidbody>().isKinematic = true;
                objectDragging.GetComponent<PlayerScript>().SetPlayerState(PlayerState.IsBeingDragged);

                // maybe replace MoveTowards with Lerp
                objectDragging.transform.position = Vector3.MoveTowards(objectDragging.transform.position, holdPosition.position, 0.5f);
                objectDragging.transform.position += new Vector3(0, 0.0004f, 0);

                if (dragGrabTriggerCount.GetGameobjectsCollidingWith().Contains(objectDragging.gameObject))
                {
                    Debug.Log("TRy to grab player!");

                    GrabPlayer(objectDragging.GetComponent<PlayerScript>());
                    Destroy(lineEffekt);

                    objectDragging = null;
                }

                audio.PlayDrag();
            }
            else
            {
                playerState = PlayerState.None;
                Destroy(lineEffekt);
            }
        }
        else if(playerState == PlayerState.Dragging) // ?????????
        {
            playerState = PlayerState.None;
            Destroy(lineEffekt);
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
                audio.PlayThrow();
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
                audio.PlayThrow();
                //}

                Drop(true);
            }
        }
    }

    private void Emote()
    {
        if (!isGrounded)
            return;

        float rand = Random.Range(0, 2.1f);
        animatorPlayer.SetFloat("EmoteToUse", rand);

        playerState = PlayerState.Emoting;

        //Pick a random text to use in emote pop up
        string[] commonMessages = new string[] { "slay", "slay", "slay", "wow","queen", "awesome", "haha", "epic", "bestie", "sweet", "whoa", "yippie", "deserved" }; //Can add more pop up text here
        string[] uncommonMessages = new string[] { ">:D", "git gud", "oops", "damn", "problem?", "yay", "*kiss*", "mvp" }; //Can add more pop up text here
        string[] rareMessages = new string[] { "victory royale", "for the horde", "poggers", "hadouken", "therese approved", "bestest bestie", "it's a me, bestie" };   //Can add more pop up text here

        string[] emoteMessages;
        float random = Random.value;
        if (random < 0.70f)
        {
            emoteMessages = commonMessages;
        } else if (random < 0.96f)
        {
            emoteMessages = uncommonMessages;
        } else {
            emoteMessages = rareMessages;
        }

        int randomIndex = Random.Range(0, emoteMessages.Length);
        string selectedMessage = emoteMessages[randomIndex];
        popUpManager.SpawnPopUp(mainCamera, transform, selectedMessage, color);
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
                    audio.PlayDrop();
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
            audio.PlayGrab();
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

    public UnityEngine.Color GetPlayerColor()
    {
        return color;
    }

    //This doesnt work really...
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Fence"))
        {
            isInFence = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fence"))
        {
            isInFence = false;
        }
    }

    public string GetPlayerTypeString()
    {
        switch (playerType)
        {
            case (PlayerType.PlayerOne):
                return "p1";
            case (PlayerType.PlayerTwo):
                return "p2";
            case (PlayerType.PlayerThree):
                return "p3";
            case (PlayerType.PlayerFour):
                return "p4";
            default:
                return "p1";
        }
    }
}
