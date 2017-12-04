using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : MonoBehaviour
{
    public float speed;
    public float range;
    public CharacterController controller;
    public Transform player;
    static Animator anim;
    public Physics gravity;

    // Use this for initialization
    void Start ()
    {

        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!inRange())
        {
            chase();

        }
        else
        {
            anim.SetBool("isIdle", true);
            anim.SetBool("isRunning", false);
        }

        //Debug.Log(inRange());
        
	}

    bool inRange ()
    {
        if (Vector3.Distance(transform.position, player.position)>range)
        {
            return true;
        }

        else 
        {
            return false;
        }
    }

    void chase()
    {
        transform.LookAt(player.position);
        controller.SimpleMove(transform.forward*speed);
        anim.SetBool("isRunning", true);
        anim.SetBool("isIdle", false);
        
    }

    private void OnMouseOver()
    {
        Debug.Log("Mouse over the enemy");
    }


}
