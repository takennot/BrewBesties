using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Saw : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private List<PlayerScript> playersSawing = new List<PlayerScript>();
    [SerializeField] private int players;

    [Header("Colliders")]
    [SerializeField] private SawingPlate sawingPlate1;
    [SerializeField] private SawingPlate sawingPlate2;

    [Header("Speed")]
    [SerializeField] private float currentSawSpeed = 0;
    [SerializeField] private float sliderValue = 0;

    [SerializeField] private float playerSawSpeed = 2;
    [SerializeField] private float maxSawProcess = 15;
    [Range(0, 4)]
    [SerializeField] private float twoPlayerExtraSpeed = 0.5f;

    [Header("Refs")]
    [SerializeField] private Slider sawSlider;
    [SerializeField] private Image sawSliderFillArea;
    [SerializeField] private Transform sawSlider_Pos;
    [SerializeField] private Transform woodSpawnpoint;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    public float baseSpeed = 2;

    [Header("Outline")]
    [SerializeField] private OutlineHandler outlineHandler1;
    [SerializeField] private OutlineHandler outlineHandler2;

    [Header("Sound")]
    [SerializeField] private AudioClip sawingSound;
    [SerializeField] private AudioClip woodDropSound;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource sourceSawing;

    public bool hasSawed;

    [Header("VFX")]
    [SerializeField] GameObject sawEffect;
    [SerializeField] Transform particalSystemPostion;
    GameObject instanciestEffect;
    private bool createOnce = true;


    // Start is called before the first frame update
    void Start()
    {
        sawSlider.maxValue = maxSawProcess;
        source = GetComponent<AudioSource>();
        sourceSawing.clip = sawingSound;
        sourceSawing.Pause();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        players = playersSawing.Count;
        animator.SetInteger("Players", players);

        if (playersSawing.Count > 0)
        {
            animator.speed = players * baseSpeed; 

            float speedCalc = 0;
            foreach (PlayerScript player in playersSawing)
            {
                speedCalc += playerSawSpeed;
            }
            currentSawSpeed = (players == 2) ? speedCalc + twoPlayerExtraSpeed : playerSawSpeed;

            sourceSawing.pitch = playersSawing.Count;
        }
        else
        {
            if (sourceSawing.isPlaying)
            {
                sourceSawing.Pause();
            }

            currentSawSpeed = 0;
            animator.speed = 0;
        }

        sawSlider.value += Time.deltaTime * currentSawSpeed;

        sliderValue = sawSlider.value;

        if(sawSlider.value == sawSlider.maxValue)
        {
            DropWood();
        }
    }

    public void DoSawProcess(PlayerScript thisPlayer)
    {
        if(PlatesIsActivatedByPlayer(thisPlayer))
        {
            if (!playersSawing.Contains(thisPlayer) && playersSawing.Count < 2)
            {
                Debug.Log("added to saw: " + thisPlayer.name);
                playersSawing.Add(thisPlayer);
                sourceSawing.Play();
                createWoodEffekt();
            }
        }
    }

    public void StopSawProcess(PlayerScript thisPlayer)
    {
        Debug.Log("Removing from saw: " + thisPlayer.name);
        playersSawing.Remove(thisPlayer);
        sourceSawing.Pause();
        createOnce = true;
        Destroy(instanciestEffect);
    }

    private void DropWood()
    {
        hasSawed = true; //For the tutorial
        sawSlider.value = 0;

        ResourceBoxHandler resourceBoxHandler = gameObject.GetComponent<ResourceBoxHandler>();

        GameObject wood = Instantiate<GameObject>(resourceBoxHandler.fireWood);
        source.PlayOneShot(woodDropSound);

        wood.transform.position = woodSpawnpoint.position;
        wood.transform.rotation = woodSpawnpoint.rotation;
        wood.gameObject.name = "FireWood";
    }

    public Transform GetSawSliderTransform()
    {
        return sawSlider_Pos;
    }
    public Slider GetSlider()
    {
        return sawSlider;
    }

    public void ShowSawOutlineIfOk(PlayerScript playerScript, Color playerColor, bool thick)
    {
        if(PlatesIsActivatedByPlayer(playerScript))
        {
            outlineHandler1.ShowOutline(playerColor, thick);
        }
    }

    private bool PlatesIsActivatedByPlayer(PlayerScript playerScript)
    {
        return sawingPlate1.GetPlayerColliding() == playerScript || sawingPlate2.GetPlayerColliding() == playerScript;
    }

    void createWoodEffekt()
    {
        if (createOnce)
        {
            instanciestEffect = Instantiate(sawEffect, particalSystemPostion);
            createOnce = false;
        }
      
    }
}
