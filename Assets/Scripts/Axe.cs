using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public Rigidbody2D rb;
    Collider2D thisCollider;
    private SpriteRenderer sprite;
    public GameObject wielder;
    WeaponsManager weaponsManager;
    // 0 = held by player, 1 = thrown, 2 = stuck, 3 = returning

    //public int state;
    public enum State {Held, Thrown, Stuck, Return};
    public State currentState = State.Held;
    float returnSpeed = 10f;
    public int damage = 1;
    // Start is called before the first frame update
    public float spinSpeed = 60;
    private float throwAngle;

    public float throwShakeDuration;
    public float throwShakeMagnitude;
    public float stickShakeDuration;
    public float stickShakeMagnitude;
    public float reacllShakeDuration;
    public float recallShakeMagnitude;

    public AudioClip throwSound;
    public AudioClip stickSound;
    public AudioClip grabSound;

    //public CameraScript cameraShake;
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        thisCollider = gameObject.GetComponent<Collider2D>();
        rb.simulated = false;
        weaponsManager = wielder.GetComponent<WeaponsManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (currentState == State.Return)
        {
            rb.velocity = (wielder.transform.position - transform.position).normalized * returnSpeed;
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        }
        if(currentState == State.Held || currentState == State.Stuck){
            thisCollider.enabled = false;
        }
        else{
            thisCollider.enabled = true;
            //transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        }
        if(currentState == State.Held){
            sprite.enabled = false;
        }
        else{
            sprite.enabled = true;
        }
        if(currentState == State.Stuck){
            rb.velocity = Vector2.zero;
        }
    }

    public void ThrowWeapon(Transform throwingPoint, float axeForce)
    {
        /*if(CinemachineShakeScript.instance != null){
            CinemachineShakeScript.instance.ShakeCamera(4, .12f);
        }*/
        CameraScript.instance.Shake(throwShakeDuration, throwShakeMagnitude);
        if(throwSound != null){
            AudioManagerScript.instance.PlaySound(throwSound, transform.position);
        }

        transform.parent = null;
        currentState = State.Thrown;
        rb.simulated = true;
        transform.position = throwingPoint.position;
        transform.eulerAngles = new Vector3(0, 0, throwingPoint.eulerAngles.z + 90);
        throwAngle = throwingPoint.eulerAngles.z;
        rb.velocity = throwingPoint.up * axeForce;
        //AudioManager.instance.PlaySound("Axe Throw");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "wall" && currentState == State.Thrown)
        {
            currentState = State.Stuck;
            rb.velocity = Vector2.zero;
            if(stickSound != null){
                AudioManagerScript.instance.PlaySound(stickSound, transform.position);
            }
            CameraScript.instance.Shake(stickShakeDuration, stickShakeMagnitude);

            transform.eulerAngles = new Vector3(0, 0, throwAngle + 90f);
        } else if (collision.tag == "Player" && currentState == State.Return)
        {
            //CinemachineShakeScript.instance.ShakeCamera(4f, .12f);
            rb.velocity = Vector2.zero;
            rb.simulated = false;
            transform.parent = wielder.transform;
            currentState = State.Held;
            weaponsManager.RegrabWeapon();
            transform.position = weaponsManager.throwingPoint.position;
            transform.rotation = weaponsManager.throwingPoint.rotation;
            
            if(grabSound != null){
                AudioManagerScript.instance.PlaySound(grabSound, transform.position);
            }
        } else if ((collision.tag == "object" || collision.tag == "Enemy") && (currentState == State.Thrown || currentState == State.Return))
        {
            /*ObjectHealthScript objectHealth = collision.GetComponent<ObjectHealthScript>();
            objectHealth.UpdateHealth(damage);
            if(CinemachineShakeScript.instance != null){
                CinemachineShakeScript.instance.ShakeCamera(6.5f, .2f);
            }*/
            IDamageable damageableObject = collision.GetComponent<IDamageable>();
            if(damageableObject != null){
                damageableObject.TakeHit(damage, collision.transform.position, transform.up);
            }
        }

    }

    public void RecallWeapon(float axeForce)
    {
        if(currentState == State.Stuck){
            currentState = State.Return; 
            CameraScript.instance.Shake(reacllShakeDuration, recallShakeMagnitude);
            /*if(CinemachineShakeScript.instance != null){
                CinemachineShakeScript.instance.ShakeCamera(4, .12f);
            }*/
            //AudioManager.instance.PlaySound("Axe Throw");
        }
        returnSpeed = axeForce;
    }

    public void ForceRecall(){
        if(currentState == State.Stuck || currentState == State.Thrown){
            currentState = State.Return;
        }
    }
}
