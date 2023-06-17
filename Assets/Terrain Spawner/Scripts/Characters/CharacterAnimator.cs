using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{

    private SimpleTPSController controller;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<SimpleTPSController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Walk(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);
        UpdateAnimations();
    }


    public void UpdateAnimations()
    {
        animator.SetFloat("Vertical", controller.vertical);
        animator.SetFloat("Horizontal", controller.horizontal);
        animator.SetBool("Running", controller.isRunning);
    }
}
