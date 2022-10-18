using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float speed = 10f;
    bool isPlayerBullet = true;

    public LayerMask collisionMask;
    //public Color trailColor;
    float damage = 1;
    public float lifetime = 3f;
    float skinWidth = 0.1f;

    void Start(){
        Destroy(gameObject, lifetime);

        Collider2D[] initialCollisions = Physics2D.OverlapCircleAll(transform.position, 0.1f, collisionMask);
        if(initialCollisions.Length >= 1){
            OnHitObject(initialCollisions[0], transform.position);
        }
    }

    public void SetCharacteristics(float newSpeed, bool playerOwns, float newDamage=1f)
    {
        speed = newSpeed;
        isPlayerBullet = playerOwns;
        damage = newDamage;
    }

    void Update()
    {
        float moveDistance = Time.deltaTime * speed;
        CheckCollisions(moveDistance + skinWidth);
        transform.Translate(Vector3.up * moveDistance);
    }

    void CheckCollisions(float moveDistance){
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, transform.up, moveDistance, collisionMask);
        if(hit2D){
            Debug.Log("Hit");
            OnHitObject(hit2D.collider, hit2D.point);
        }
    } 

    void OnHitObject(Collider2D col, Vector3 hitPoint){
        IDamageable damageableObject = col.GetComponent<IDamageable>();
        if(damageableObject != null){
            if(col.tag == "Enemy" && isPlayerBullet){
                damageableObject.TakeHit(damage, hitPoint, transform.up);
                Destroy(gameObject);
            }
            else if(col.tag == "Player" && !isPlayerBullet){
                damageableObject.TakeHit(damage, hitPoint, transform.up);
                Destroy(gameObject);
            }
        }
        else{
            Destroy(gameObject);
        }
    }
}
