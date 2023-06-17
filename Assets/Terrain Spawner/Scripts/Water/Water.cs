using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        Character character = other.gameObject.GetComponent<Character>();
        if (character != null)
        {
            character.EnteredOnWater();
        }

    }


    private void OnTriggerStay(Collider other)
    {
        Character character = other.gameObject.GetComponent<Character>();
        if (character != null)
        {
            character.StayOnWater();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        Character character = other.gameObject.GetComponent<Character>();
        if (character != null)
        {
            character.LeaveWater();
        }
    }

}
