using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeScript : MonoBehaviour, IEnemyWeapon
{
    public enum State {Idle, Startup, Attack, Endlag, Cooldown, Pause};
    State currentState;
    public bool isAttacking {get; set;}
    public bool isCooling {get; set;}
    public float startupTime;
    public float attackTime;
    public float endlagTime;
    public float attackCooldown;
    public float pauseAttackTime;
    float endTimer;
    //public GameObject swordObject;
    public float swordRotation;
    private Quaternion leftRotation = Quaternion.identity;
    private Quaternion rightRotation = Quaternion.identity;
    bool facingRight = true;
    public Transform swordGFX;
    public Transform attackPoint;
    public Transform indicatorPoint;
    public float attackRadius;
    public LayerMask playerLayers;
    public float damage;

    public AttackIndicatorScript attackIndicator;
    
    void Awake(){
        isAttacking = false;
        isCooling = false;
        leftRotation.eulerAngles = new Vector3(0, 0, swordRotation);
        rightRotation.eulerAngles = new Vector3(0, 0, -swordRotation);
        if(facingRight){
            swordGFX.localRotation = rightRotation;
        }
        else{
            swordGFX.localRotation = leftRotation;
        }
    }

    void Update(){
        if(currentState != State.Idle && Time.time > endTimer){
            switch(currentState){
                case State.Startup:
                    StartCoroutine(AttackAnimation());
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
        if(currentState == State.Idle){
            currentState = State.Startup;
            endTimer = Time.time + startupTime;
            AttackIndicatorScript indicator = Instantiate(attackIndicator, indicatorPoint.position, Quaternion.identity);
            indicator.SetCharacteristics(startupTime);
            indicator.transform.parent = indicatorPoint;
        }
    }

    public void StartPause(){
        currentState = State.Pause;
        endTimer = Time.time + pauseAttackTime;
    }

    public void CancelAttack(){
        if(currentState == State.Attack){
            StopCoroutine(AttackAnimation());
        }
        currentState = State.Idle;
    }
    
    public bool IsAttacking(){
        return currentState == State.Attack;
    }

    void AttackDamage(){
        Collider2D player = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayers);
        if(player != null){
            IDamageable damageableObject = player.GetComponent<IDamageable>();
            if(damageableObject != null){
                damageableObject.TakeHit(damage, player.transform.position, transform.up);
            }
        }
    }

    IEnumerator AttackAnimation(){
        AudioManagerScript.instance.PlaySound("Swipe", transform.position);
        currentState = State.Attack;
        Quaternion originalRotation;
        Quaternion newRotation;
        if(facingRight){
            originalRotation = rightRotation;
            newRotation = leftRotation;
        }
        else{
            originalRotation = leftRotation;
            newRotation = rightRotation;
        }
        
        float attackSpeed = 1 / attackTime;
        float percent = 0;

        bool hasFlipped = false;
        //AudioManagerScript.instance.PlaySound("Enemy Attack", transform.position);

        while(percent <= 1){

            if(percent >= 0.5f && !hasFlipped){
                facingRight = !facingRight;
                hasFlipped = true;
                AttackDamage();
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = percent * percent * (3f - 2f * percent);
            swordGFX.localRotation = Quaternion.Lerp(originalRotation, newRotation, interpolation);

            yield return null;
        }
        currentState = State.Endlag;
        endTimer = Time.time + endlagTime;
    }

    public void ReceiveAngle(float angle){
        //aimAngle = angle;
    }

    public void SetDamage(float newDamage){
        damage = newDamage;
    }
    public void Flip(){
        
    }
}
