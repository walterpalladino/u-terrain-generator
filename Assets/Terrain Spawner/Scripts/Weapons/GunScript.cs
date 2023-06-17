using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{

    [SerializeField]
    [Range(0.5f, 1.0f)]
    float fireRate = 1.0f;

    [SerializeField]
    [Range(1.0f, 10.0f)]
    float damage = 1.0f;


    [SerializeField]
    Transform firePoint;

    Camera mainCamera;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;   
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > fireRate)
        {
            if (Input.GetButton("Fire1"))
            {
                timer = 0.0f;
                FireGun();
            }
        }
    }

    private void FireGun()
    {
        Debug.Log("Gun Fired");

        //  Play Sound
        //  Activate Particles

        Ray ray = mainCamera.ViewportPointToRay(Vector3.one * 0.5f);
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.red, 2.0f);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            Debug.Log(hit.collider.name);
        }

    }
}
