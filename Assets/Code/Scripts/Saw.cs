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

    [Header("Refs")]
    [SerializeField] private Slider sawSlider;
    [SerializeField] private Image sawSliderFillArea;
    [SerializeField] private Transform sawSlider_Pos;
    [SerializeField] private Transform woodSpawnpoint;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    public float baseSpeed = 2;

    [Header("Sound")]
    [SerializeField] private AudioClip sawingSound;
    [SerializeField] private AudioClip woodDropSound;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource sourceSawing;

    public bool hasSawed;

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

            currentSawSpeed = speedCalc;

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
        if(sawingPlate1.GetPlayerColliding() == thisPlayer || sawingPlate2.GetPlayerColliding() == thisPlayer)
        {
            if (!playersSawing.Contains(thisPlayer) && playersSawing.Count < 2)
            {
                Debug.Log("added to saw: " + thisPlayer.name);
                playersSawing.Add(thisPlayer);
                sourceSawing.Play();
            }
        }
    }

    public void StopSawProcess(PlayerScript thisPlayer)
    {
        Debug.Log("Removing from saw: " + thisPlayer.name);
        playersSawing.Remove(thisPlayer);
        sourceSawing.Pause();
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
}
