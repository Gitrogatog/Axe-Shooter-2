using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float health {get; protected set;}
    public float maxHealth = 3;
    public GameObject deathSprite;
    protected bool isDead = false;
    public event System.Action OnDeath;
    public float hurtShakeDuration;
    public float hurtShakeMagnitude;
    public float deathShakeDuration;
    public float deathShakeMagnitude;
    
    protected virtual void Awake(){
        health = maxHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitpoint, Vector3 hitDirection){
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage){
        health -= damage;
        if(health <= 0 && !isDead){
            Die();
        }
        else{
            CameraScript.instance.Shake(hurtShakeDuration, hurtShakeMagnitude);
        }
    }

    [ContextMenu("Self Destruct")]
    public virtual void Die(){
        CameraScript.instance.Shake(deathShakeDuration, deathShakeMagnitude);
        
        isDead = true;
        if(deathSprite != null){
            Destroy(Instantiate(deathSprite, transform.position, Quaternion.identity), 1f);
        }
        if(OnDeath != null){
            OnDeath();
        }
        Destroy(gameObject);
    }
}
