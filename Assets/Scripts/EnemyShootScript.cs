using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootScript : MonoBehaviour, IEnemyWeapon
{
    public enum State {Idle, Startup, Attack, Endlag, Cooldown, Pause};
    State currentState;
    public bool isAttacking {get; set;}
    public bool isCooling {get; set;}
    /*
    public float attackTime;
    public float attackCooldown;
    public float pauseAttackTime;
    float endAttackTime;
    float endCooldownTime;
    float endPauseTime;
    */
    public float startupTime;
    public float attackTime;
    public float endlagTime;
    public float attackCooldown;
    public float pauseAttackTime;
    float endTimer;

    public Bullet projectile;
    public float bulletSpeed;
    public float bulletDamage = 1;
    public Transform shootPoint;
    public Transform indicatorPoint;
    public AttackIndicatorScript attackIndicator;
    void Awake(){
        isAttacking = false;
        isCooling = false;
    }

    void Update(){
        /*
        if(isAttacking && Time.time > endAttackTime){
            isAttacking = false;
            isCooling = true;
            endCooldownTime = Time.time + attackCooldown;
        }
        if(isCooling && Time.time > endCooldownTime){
            isCooling = false;
        }
        */
        if(currentState != State.Idle && Time.time > endTimer){
            switch(currentState){
                case State.Startup:
                    FireBullet();
                    currentState = State.Endlag;
                    endTimer = Time.time + attackTime;
                    break;
                case State.Endlag:
                    currentState = State.Cooldown;
                    endTimer = Time.time + attackCooldown;
                    break;
                case State.Cooldown:
                    currentState = State.Idle;
                    break;
                case State.Pause:
                    currentState = State.Idle;
                    break;
            }
        }
    }

    public void Attack(){
        /*
        if(!isAttacking && !isCooling && Time.time > endPauseTime){
            isAttacking = true;
            endAttackTime = Time.time + attackTime;
            
        }
        */
        if(currentState == State.Idle){
            currentState = State.Startup;
            endTimer = Time.time + startupTime;
            AttackIndicatorScript indicator = Instantiate(attackIndicator, indicatorPoint.position, Quaternion.identity);
            indicator.SetCharacteristics(startupTime);
            indicator.transform.parent = indicatorPoint;
        }
    }

    void FireBullet(){
        AudioManagerScript.instance.PlaySound("Enemy Shoot", transform.position);
        Bullet bulletScript = Instantiate(projectile, shootPoint.position, shootPoint.rotation) as Bullet;
        bulletScript.SetCharacteristics(bulletSpeed, false, bulletDamage);
        Destroy(bulletScript.gameObject, 5f);
    }

    public void StartPause(){
        currentState = State.Pause;
        endTimer = Time.time + pauseAttackTime;
    }

    public bool IsAttacking(){
        return currentState == State.Startup || currentState == State.Endlag;
    }

    public void CancelAttack(){
        if(currentState != State.Cooldown && currentState != State.Pause){
            currentState = State.Idle;
        }
    }

    public void ReceiveAngle(float angle){
        //transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public void SetDamage(float newDamage){
        bulletDamage = newDamage;
    }

    public void Flip(){
        Vector3 theScale = transform.localScale;
            theScale.y *= -1;
            transform.localScale = theScale;
    }
}
