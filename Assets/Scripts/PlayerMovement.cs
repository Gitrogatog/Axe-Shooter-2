using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : LivingEntity
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    Vector2 movement;
    Vector2 mousePos;
    public Camera cam;
    private float timer = 0;
    private Vector2 lockDir;
    private int state = 0; //0 = normal walking, 1 = movement locked, 2 = dead

    // Update is called once per frame
    void Update()
    {
        if(state == 2){
            movement = Vector2.zero;
            rb.velocity = Vector2.zero;
        }
        else{
            if(state == 1){
                /*
                timer -= Time.deltaTime;
                if(timer <= 0){
                    state = 0;
                }
                else{
                    movement = lockDir;
                }
                */
                movement = lockDir;
            }
            if(state == 0){
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");
                movement = movement.normalized;
            }
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    public void LockPlayerWalk(Vector2 walkDir){
        if(state != 2){
            lockDir = walkDir;
            //timer = lockTime;
            state = 1;
        }
    }

    public void UnlockPlayerWalk(){
        if(state != 2){
            state = 0;
        }
    }

    public void KillPlayer(){
        state = 2;
        rb.velocity = Vector2.zero;
    }
}
