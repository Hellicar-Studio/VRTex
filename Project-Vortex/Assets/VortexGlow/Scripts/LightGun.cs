﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGun : MonoBehaviour
{
    public GameObject bullet;
    private Transform t;
    private Rigidbody bulletBody;
    private Light bulletLight;
    private Material orbMat;
    public AudioInput audioInput;
    public float cooldown;

    private float lastFireTime;

    private List<Coroutine> activeRoutines;

    public float maxRange;
    public float speed = 0.01f;

    [SerializeField] private VRStandardAssets.Utils.VRInput m_VRInput;

    // Use this for initialization
    void Start()
    {
        t = this.transform;
        bullet = Instantiate(bullet, t);
        bullet.transform.parent = t.parent;
        bulletBody = bullet.GetComponent<Rigidbody>();
        bulletLight = bullet.GetComponent<Light>();
        //orbMat = bullet.GetComponentInChildren<MeshRenderer>().material;

        activeRoutines = new List<Coroutine>();

        bulletBody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnEnable()
    {
        Debug.Log("Enabled! in Light Gun!");
        m_VRInput.OnClick += fireLight;
    }

    private void OnDisable()
    {
        Debug.Log("Disabled! in Light Gun!");
        m_VRInput.OnClick -= fireLight;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastFireTime > cooldown)
        {
            if (Input.GetKeyUp("space"))
            {
                fireLight();
            }
            if (audioInput.pulse)
            {
                fireLight();
            }
        }
    }

    void fireLight()
    {
        for (int i = 0; i < activeRoutines.Count; i++)
        {
            if (activeRoutines[i] != null)
            {
                StopCoroutine(activeRoutines[i]);
            }
        }
        activeRoutines.Clear();
        if (orbMat != null)
        {
            activeRoutines.Add(StartCoroutine(FadeOrb(new Color(1.0f, 1.0f, 1.0f))));
        }
        activeRoutines.Add(StartCoroutine(FadeLight(50.0f)));
        activeRoutines.Add(StartCoroutine(MoveForward(t.forward)));

        lastFireTime = Time.time;
    }

    IEnumerator FadeOrb(Color target)
    {
        Color col = orbMat.GetColor("_RimColor");
        while (col != target)
        {
            col = Color.Lerp(col, target, 0.1f);
            orbMat.SetColor("_RimColor", col);
            yield return null;
        }
    }

    IEnumerator FadeLight(float target)
    {
        float range = bulletLight.range;
        while (range < target)
        {
            range = Mathf.Lerp(range, target, 0.1f);
            bulletLight.range = range;
            yield return null;
        }
    }

    IEnumerator MoveForward(Vector3 forward)
    {
        bullet.transform.localPosition = new Vector3(0.0f, -0.5f, 0.0f);
        Vector3 pos = bullet.transform.position;
        float dist = Vector3.Distance(pos, t.position);
        while (dist < maxRange)
        {
            pos = Vector3.Lerp(pos, pos + forward * maxRange, speed);
            bulletLight.transform.position = pos;
            dist = Vector3.Distance(pos, t.position);
            yield return null;
        }
    }

}
