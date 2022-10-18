using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunController))]
public class ThrowingAndTheWorks : MonoBehaviour
{
    public Transform throwingPoint;
    public GameObject axe;
    public Animator animator;
    public float axeForce = 10f;
    public bool holdingAxe;
    Axe retribution;
    GunController gunController;
    //bool clingPossibility;

    private void Start()
    {
        retribution = axe.GetComponent<Axe>();
        gunController = GetComponent<GunController>();
        //Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), axe.GetComponent<PolygonCollider2D>());
        //clingPossibility = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (holdingAxe)
            {
                retribution.ThrowWeapon(throwingPoint, axeForce);
                holdingAxe = false;
            } else
            {
                retribution.RecallWeapon(axeForce);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (holdingAxe)
            {
                //swing code
                /*if (retribution.state == 2)
                {
                    transform.position = axe.transform.position;
                    axe.transform.parent = gameObject.transform;
                    holdingAxe = true;
                    Rigidbody2D axeRigidbody = axe.GetComponent<Rigidbody2D>();
                    axeRigidbody.simulated = false;
                    Axe aScript = axe.GetComponent<Axe>();
                    aScript.state = 0;
                    //GameObject shockwave = GameObject.FindGameObjectWithTag("shockwave");
                    //Destroy(shockwave);
                }*/
            } else
            {
                gunController.Shoot();
            }
        }
        //animator.SetBool("Holding", holdingAxe);
    }
}
