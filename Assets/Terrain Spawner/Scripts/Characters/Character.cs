using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField]
    bool onWater;
    [SerializeField]
    float initialDepth;
    [SerializeField]
    float waterDepth;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnteredOnWater()
    {
        onWater = true;
        initialDepth = gameObject.transform.position.y;
    }

    public void StayOnWater()
    {
        waterDepth = initialDepth - gameObject.transform.position.y;

        if (CheckUnderwater())
        {
            EnabledUnderwaterEffects();
        }
        else
        {
            DisableUnderwaterEffects();
        }
    }

    public void LeaveWater()
    {
        onWater = false;
    }


    //
    private bool CheckUnderwater()
    {
        return (waterDepth > 1.7f);
    }

    private void DisableUnderwaterEffects()
    {
        RenderSettings.fog = false;
    }

    private void EnabledUnderwaterEffects()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.1477217f, 0.2632219f, 0.429f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.27f;
    }



}
