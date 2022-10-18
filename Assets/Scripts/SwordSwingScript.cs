using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwingScript : MonoBehaviour
{
    public enum State {Idle, Attack, Cooldown};
    State currentState;
    public float attackTime;
    public float attackCooldown;
    float endTimer;
    public float swordRotation;
    private Quaternion leftRotation = Quaternion.identity;
    private Quaternion rightRotation = Quaternion.identity;
    bool facingRight = true;
    bool triggerReleasedSinceLastShot = true;
    public Transform swordGFX;
    public Transform attackPoint;
    public Transform indicatorPoint;
    public AttackIndicatorScript attackIndicator;
    public float attackRadius;
    public LayerMask enemyLayers;
    public float damage;
    WeaponsManager weaponsManager;

    void Awake(){
        weaponsManager = GetComponentInParent<WeaponsManager>();
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
        if(currentState == State.Cooldown && Time.time > endTimer){
            currentState = State.Idle;
        }
    }

    public void Attack(){
        if(currentState == State.Idle){
            AttackDamage();
            StartCoroutine(AttackAnimation());
            AudioManagerScript.instance.PlaySound("Swipe", transform.position);
        }
    }

    public void CancelAttack(){
        if(currentState == State.Attack){
            StopCoroutine(AttackAnimation());
        }
        if(currentState != State.Cooldown){
            currentState = State.Idle;
        }
    }

    void AttackDamage(){
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayers);
        foreach(Collider2D enemy in hitEnemies){
            Debug.Log("Attempt");
            IDamageable damageableObject = enemy.GetComponent<IDamageable>();
            if(damageableObject != null){
                damageableObject.TakeHit(damage, enemy.transform.position, transform.right);
                weaponsManager.RechargeWeapon();
                weaponsManager.SwordHit();
            }
        }
    }

    IEnumerator AttackAnimation(){
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
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = percent * percent * (3f - 2f * percent);
            swordGFX.localRotation = Quaternion.Lerp(originalRotation, newRotation, interpolation);

            yield return null;
        }
        currentState = State.Cooldown;
        endTimer = Time.time + attackCooldown;
        AttackIndicatorScript indicator = Instantiate(attackIndicator, indicatorPoint.position, Quaternion.identity);
        indicator.SetCharacteristics(attackCooldown);
        indicator.transform.parent = indicatorPoint;
    }

    public void EnableGraphics(){
        swordGFX.gameObject.SetActive(true);
    }

    public void DisableGraphics(){
        StopCoroutine(AttackAnimation());
        swordGFX.gameObject.SetActive(false);
    }

    public void OnTriggerHold(){
        if(triggerReleasedSinceLastShot){
            Attack();
            triggerReleasedSinceLastShot = false;
        }
    }

    public void OnTriggerRelease(){
        triggerReleasedSinceLastShot = true;
    }
}
