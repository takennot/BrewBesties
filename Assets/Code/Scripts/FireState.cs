using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FireState : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private bool needsFire = true;
    [SerializeField] private float maxFireValue = 35f;
    [SerializeField] private float woodValue = 7f;
    [SerializeField] private float fireDecreaseSpeed = 1f;
    private float minValue = 10f;

    [Header("Visuals")]
    [SerializeField] private Material off;
    [SerializeField] private Material on;
    [SerializeField] private MeshRenderer fireMeshRenderer;
    [SerializeField] private ParticleSystem fire;
    ChangeVFXParameter changeVFXParameter;

    [Header("UI")]
    [SerializeField] private Slider fireSlider;
    [SerializeField] private Image fireSliderFillArea;
    [SerializeField] private Transform fireSlider_Pos;
    [SerializeField] private Billboard infiniteFireCanvas;

    [Header("Audio")]
    [SerializeField] private AudioClip addFireClip;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource boilingAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        fireSlider.maxValue = maxFireValue;
        if(fire != null)
        {
            changeVFXParameter = fire.GetComponent<ChangeVFXParameter>();
        }

        source = GetComponent<AudioSource>();
        if(needsFire)
        {
            infiniteFireCanvas.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    [System.Obsolete]
    void FixedUpdate()
    {
        if(!needsFire)
        {
            fireSlider.gameObject.SetActive(false);
            IsWarm();
            return;
        }

        if(needsFire)
        {
            fireSlider.value -= Time.deltaTime * fireDecreaseSpeed;

            if (!IsWarm())
            {
                fireSliderFillArea.color = Color.cyan;
                //fireMeshRenderer.material = off;
            } else
            {
                fireSliderFillArea.color = Color.red;
                //fireMeshRenderer.material = on;
            }

            if (fire != null)
            {
                if (fireSlider.value <= minValue)
                {
                    changeVFXParameter.particlasEmmison = 0;
                    boilingAudioSource.Pause();
                } else
                {
                    changeVFXParameter.particlasEmmison = fireSlider.value;
                    boilingAudioSource.UnPause();
                }
            }
        }
    }

    public Transform GetFireSliderTransform()
    {
        return fireSlider_Pos;
    }
    public Slider GetSlider()
    {
        return fireSlider;
    }

    public bool IsWarm()
    {
        if(!needsFire)
        {
            return true;
        }
        return fireSlider.value >= minValue;
    }

    public void AddWood()
    {
        source.PlayOneShot(addFireClip);
        fireSlider.value += woodValue;
    }
}
