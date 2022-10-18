using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : LivingEntity
{
    public float moveSpeed = 5f;

    public Transform crosshair;
    public Camera cam;
    WeaponsManager weaponsManager;
    LevelGenerator levelGenerator;

    bool isHolding = true;
    Rigidbody2D rb;
    Vector2 currentVelocity;
    Vector2 mousePos;
    public Transform playerGFX;
    public Transform weaponsRotator;
    public Animator animator;
    bool facingRight = false;
    float invincibilityTime;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        weaponsManager = GetComponent<WeaponsManager>();
        rb = GetComponent<Rigidbody2D>();
        levelGenerator = FindObjectOfType<LevelGenerator>();
        //FindObjectOfType<EnemySpawner>().OnNewWave += OnNewWave;
    }

    public void OnNewWave(){
        health = maxHealth;
        transform.position = levelGenerator.GetRandomFloorCoord();
        //gunController.EquipGun(waveNum - 1);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        currentVelocity = moveInput.normalized * moveSpeed;
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        crosshair.position = mousePos;
        
        if(Input.GetMouseButton(0)){
            weaponsManager.OnTriggerHold();
        }
        if(Input.GetMouseButtonUp(0)){
            weaponsManager.OnTriggerRelease();
        }
        if(Input.GetMouseButtonDown(0)){
            //animator.SetTrigger("Attack");
        }
        if(Input.GetMouseButtonDown(1)){
            weaponsManager.OnThrowPress();
            //animator.SetTrigger("Attack");
        }
        if((mousePos - (Vector2)transform.position).sqrMagnitude > 1){
            weaponsManager.Aim(mousePos);
        }
        if(Input.mouseScrollDelta.y != 0){
            weaponsManager.ChangeGun(Input.mouseScrollDelta.y);
        }
        Vector2 lookDir = mousePos - (Vector2)transform.position;
        if(lookDir.x < 0 && facingRight){
            Flip();
        }
        else if(lookDir.x > 0 && !facingRight){
            Flip();
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("Pressed Damage");
            TakeDamage(0f);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        weaponsRotator.eulerAngles = new Vector3(0, 0, angle);
    }

    public override void Die(){
        AudioManagerScript.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }

    void Flip(){
        facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = playerGFX.localScale;
		theScale.x *= -1;
		playerGFX.localScale = theScale;
        //weaponsManager.FlipGuns();
    }

    public override void TakeDamage(float damage)
    {
        if(invincibilityTime < Time.time){
            animator.SetTrigger("Hurt");
            AudioManagerScript.instance.PlaySound("Player Hurt", transform.position);
            base.TakeDamage(damage);
            invincibilityTime = Time.time + 1.5f;
        }
        
    }

    public float GetAmmoFraction(){
        return weaponsManager.GetCurrentCharge() / weaponsManager.maxCharge;
    }
}
