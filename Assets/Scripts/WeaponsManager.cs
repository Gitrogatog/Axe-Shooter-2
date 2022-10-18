using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    public Transform throwingPoint;
    public GameObject axe;
    //public Animator animator;
    public float throwSpeed = 10f;
    public float recallSpeed;
    Axe retribution;
    GunController gunController;
    SwordSwingScript swordSwingScript;
    bool isHolding = true;
    bool isAttacking = false;
    public float rechargeAmount = 5f;
    float currentCharge = 0f;
    public float maxCharge = 30f;
    bool hasHitSinceLastThrow = false;

    void Start()
    {
        gunController = GetComponent<GunController>();
        swordSwingScript = GetComponentInChildren<SwordSwingScript>();
        retribution = GetComponentInChildren<Axe>();
        swordSwingScript.EnableGraphics();
        gunController.DisableGraphics();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerHold(){
        if(!isHolding){
            gunController.OnTriggerHold();
        }
        else{
            swordSwingScript.OnTriggerHold();
        }
    }

    public void OnTriggerRelease(){
        if(!isHolding){
            gunController.OnTriggerRelease();
        }
        else{
            swordSwingScript.OnTriggerRelease();
        }
    }

     public void Aim(Vector3 aimPoint){
        if(!isHolding){
            gunController.Aim(aimPoint);
        }
    }

    public void OnThrowPress(){
        
        if (isHolding)
        {
            if(hasHitSinceLastThrow){
                retribution.ThrowWeapon(throwingPoint, throwSpeed);
                isHolding = false;
                swordSwingScript.DisableGraphics();
                gunController.EnableGraphics();
                hasHitSinceLastThrow = false;
            }
            
        } 
        else
        {
            retribution.RecallWeapon(recallSpeed);
        }
    }

    public void RegrabWeapon(){
        isHolding = true;
        swordSwingScript.EnableGraphics();
        gunController.DisableGraphics();
    }

    public void RechargeWeapon(){
        currentCharge += rechargeAmount;
        if(currentCharge > maxCharge){
            currentCharge = maxCharge;
        }
    }

    public float GetCurrentCharge(){
        return currentCharge;
    }

    public void DechargeWeapon(float amount){
        currentCharge -= amount;
        if(currentCharge < 0){
            currentCharge = 0;
        }
    }

    public void ChangeGun(float change){
        if(!isHolding){
            if(change > 0){
                gunController.IncrementHeldGun();
            }
            else if(change < 0){
                gunController.DecrementHeldGun();
            }
        }
        
    }

    public void FlipGuns(){
        gunController.FlipGuns();
    }

    public bool IsAttacking(){
        return isAttacking;
    }

    public void SwordHit(){
        hasHitSinceLastThrow = true;
    }
}
