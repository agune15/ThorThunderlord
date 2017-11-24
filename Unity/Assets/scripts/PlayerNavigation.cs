
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavigation : MonoBehaviour {

    public Camera cam;
    public NavMeshAgent agent;

    public Animator anim;



	// Use this for initialization
	void Start ()
    {
       cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if(hit.transform)
            {
                Debug.Log(hit.transform.name);
                Debug.DrawLine(this.transform.position, hit.point, Color.red, 1);
                agent.SetDestination(hit.point);
                anim.SetBool("movement", true);

            }
        }
        if (agent.isStopped) anim.SetBool("movement", false);
        
        

	}
}
