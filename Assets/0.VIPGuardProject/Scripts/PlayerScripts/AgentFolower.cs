using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentFolower : MonoBehaviour
{
    public Transform guard;
    public Vector3 offset;
    public NavMeshAgent agent;
    public Animator guestAnim;
    public bool photoSesison;

    public Transform vehicleTarget;

    public bool followGuard;

    public bool runToACar;

    
    // Update is called once per frame
    void Update()
    {
        if (agent.enabled) 
        {
            if (followGuard)
            {
                if (!photoSesison && !runToACar)
                {
                    if (agent != null)
                        agent.destination = guard.position - offset;
                    if (agent.velocity.magnitude <= 0)
                    {
                        guestAnim.SetBool("Walk", false);
                        
                    }
                    else
                    {
                        guestAnim.SetBool("Walk", true);
                    }
                }
                else
                {
                    if (agent != null)
                        agent.destination = this.transform.position;
                    if (agent.velocity.magnitude <= 0)
                    {
                        guestAnim.SetBool("Walk", false);
                    }
                }
            }
            else
            {
                if (agent != null)
                    agent.destination = vehicleTarget.position;
                if (agent.velocity.magnitude <= 0)
                {
                    guestAnim.SetBool("Run", false);
                    guestAnim.SetBool("Walk", false);
                }
                else
                {
                    guestAnim.SetBool("Walk", false);
                    guestAnim.SetBool("Run", true);
                }
            }
        }
    }
}
