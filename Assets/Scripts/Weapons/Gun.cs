using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public abstract class Gun : Weapon
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 80f;
    [SerializeField] private SteamVR_Input_Sources rightHand;
    [SerializeField] private SteamVR_Action_Boolean shootAction = null;
    [SerializeField] private Sprite sprite = null;
    [SerializeField] private GameObject shootEffectPrefab = null;
    [SerializeField] private GameObject front = null;
    [SerializeField] private GameObject parent = null;
    [SerializeField] private int bullets = 2;
    [SerializeField] private GameObject prefabHand = null;
    [SerializeField] private GameObject rightHandGO = null;
    private int currentBullets = 0;
    private bool shoot = false;

    //sounds
    [SerializeField] private AudioClip shootingSound = null;
    [SerializeField] private AudioClip reloadSound = null;
    private AudioSource audioSource = null; //current audio source

    public Sprite Sprite
    {
        get => sprite;
    }

    public int Bullets
    {
        get => bullets;
        set
        {
            bullets = value;
        }
    }

    public int CurrentBullets
    {
        get => currentBullets;
        set
        {
            currentBullets = value;
        }
    }

    public GameObject PrefabHand
    {
        get => prefabHand;
    }

    public AudioClip ReloadSound
    {
        get => reloadSound;
    }

    public SteamVR_Action_Boolean ShootAction
    {
        get => shootAction;
    }

    public SteamVR_Input_Sources RightHand
    {
        get => rightHand;
    }

    public bool Shoot
    {
        get => shoot;
        set
        {
            shoot = value;
        }
    }

    public GameObject RightHandGO
    {
        get => rightHandGO;
    }

    public AudioSource AudioSource
    {
        get => audioSource;
        set
        {
            audioSource = value;
        }
    }

    public AudioClip ShootingSound
    {
        get => shootingSound;
        set
        {
            shootingSound = value;
        }
    }

    public GameObject Front
    {
        get => front;
    }

    public float Damage
    {
        get => damage;
    }

    public float Range
    {
        get => range;
    }


    private void Awake()
    {
        parent = GameObject.Find("ShootEffect");
        rightHandGO = GameObject.Find("RightHand");
        audioSource = GetComponent<AudioSource>();
        prefabHand.GetComponent<RenderModel>().controllerPrefab.gameObject.GetComponent<Gun>().CurrentBullets = 0;
    }

    private void Update()
    {
        if (shootAction.GetStateDown(rightHand) && shoot == false && AmmoManager.CurrentBullets > 0)
        {
            shoot = true;
            ShootGun();
        }
    }

    public void ShootGun()
    {
        GetComponentInChildren<ParticleSystem>().Play();
        rightHandGO.GetComponent<Hand>().renderModelPrefab.gameObject.GetComponent<RenderModel>().controllerPrefab.gameObject.GetComponent<Gun>().CurrentBullets -= 1;
        //sound
        audioSource.clip = shootingSound;
        audioSource.Play();

        RaycastHit hit;

        if (Physics.Raycast(front.transform.position, front.transform.forward, out hit, range))
        {
            ShootEffect(hit.point, hit);
            //if Dummy - get script for Dummy and call TakeDamage
            if (hit.collider.gameObject.CompareTag("Dummy"))
            {
                Dummy target = hit.collider.gameObject.GetComponent<Dummy>();

                if (target != null)
                {
                    target.TakeDamage(damage);

                }
            }
            //if falling target - get script for TargetFall and call TakeDamage
            else if (hit.collider.gameObject.CompareTag("TargetFall"))
            {
                TargetFall target = hit.collider.gameObject.GetComponent<TargetFall>();

                if (target != null)
                {
                    target.TakeDamage(damage);

                }
            }
        }
        shoot = false;
    }

    public void ShootEffect(Vector3 hitPoint, RaycastHit hit)
    {
        GameObject shootEffect = Instantiate(shootEffectPrefab, hitPoint + new Vector3(0.05f, 0.05f, 0.05f), Quaternion.LookRotation(hit.normal));
        shootEffect.transform.parent = parent.transform;

        StartCoroutine(DespawnShootEffect(shootEffect));

    }

    private IEnumerator DespawnShootEffect(GameObject shootEffect)
    {
        yield return new WaitForSeconds(2f);
        Destroy(shootEffect);
    }
}
