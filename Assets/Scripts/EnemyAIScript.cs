using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAIScript : LivingEntity
{
    public enum State {Wander, Chase, Attack, ReturnToStart, Stunned, Victory};
    State currentState;
    private Vector3 startPos;
    private Vector3 roamPos;
    //public ParticleSystem deathEffect;
    public static event System.Action OnDeathStatic;

    public float reachedPointDistance = 1f;
    public float nextWaypointDistance = 3f;
    public float viewPlayerDistance;
    public float forgetPlayerDistance;
    public float minStopDistance;
    public float maxStopDistance;
    public float minAttackDistance;
    public float maxAttackDistance;
    public bool stopWhileAttacking = false;

    private Rigidbody2D rb;
    public float moveSpeed;
    Transform playerT;
    public Transform weaponRotator;
    private IEnemyWeapon weapon;
    private LevelGenerator levelGenerator;

    public float losCheckTime;
    public float pathfindingCheckTime;
    private float nextLOSTime = 0;
    private float nextPathTime = 0;
    public LayerMask viewMask;
    private bool seenPlayer = false;

    public float stunTime;
    private float stunInverseTime;
    private float currentStunPercentage;
    public float knockback;
    private Vector2 stunDirection;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    Seeker seeker;

    bool facingRight = false;
    public Transform enemyGFX;
    public Animator animator;
    protected override void Awake()
    {
        base.Awake();
        playerT = FindObjectOfType<PlayerController>().transform;
        levelGenerator = FindObjectOfType<LevelGenerator>();
        seeker = GetComponent<Seeker>();
        weapon = GetComponentInChildren<IEnemyWeapon>();
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        currentState = State.Wander;
        roamPos = GetRoamingPosition();
        UpdatePath(roamPos);
        stunInverseTime = 1 / stunTime;
        weapon.StartPause();
    }

    void FixedUpdate(){
        bool followPlayerPath = false;
        if(playerT != null){
            Vector2 dirToPlayer = (Vector2)(playerT.position - transform.position);
            float sqrDistance = dirToPlayer.sqrMagnitude;
            if(nextLOSTime < Time.time){
                bool seenPrev = seenPlayer;
                seenPlayer = false;
                nextLOSTime = Time.time + losCheckTime;
                if(sqrDistance < Mathf.Pow(viewPlayerDistance, 2) && currentState != State.Stunned){
                    RaycastHit2D hit2D = Physics2D.Raycast(transform.position, dirToPlayer, viewPlayerDistance, viewMask);
                    if(hit2D && hit2D.collider.tag == "Player"){
                        currentState = State.Chase;
                        seenPlayer = true;
                    }
                }
                //Debug.Log(seenPlayer);
                if(!seenPrev && seenPlayer){
                    weapon.StartPause();
                }
            }
            if(seenPlayer && currentState != State.Stunned){
                if(sqrDistance <= Mathf.Pow(maxAttackDistance, 2) && sqrDistance >= Mathf.Pow(minAttackDistance, 2)){
                    weapon.Attack();
                }

                if(stopWhileAttacking && weapon.IsAttacking()){
                    rb.velocity = Vector2.zero;
                }
                else{
                    if(sqrDistance < Mathf.Pow(minStopDistance, 2)){
                        rb.velocity = -dirToPlayer.normalized * moveSpeed * Time.deltaTime;
                    }

                    else if(sqrDistance < Mathf.Pow(maxStopDistance, 2)){
                        rb.velocity = Vector2.zero;
                    }

                    else if(sqrDistance < Mathf.Pow(forgetPlayerDistance, 2)){
                        //rb.velocity = dirToPlayer.normalized * moveSpeed;
                        followPlayerPath = true;
                    }

                    else{
                        currentState = State.ReturnToStart;
                        UpdatePath(startPos);
                    }
                }
            }
            else{
                switch(currentState){

                    case State.Wander:
                        //rb.velocity = (roamPos - transform.position).normalized * moveSpeed;
                        if((roamPos - transform.position).sqrMagnitude < Mathf.Pow(reachedPointDistance, 2)){
                            roamPos = GetRoamingPosition();
                            UpdatePath(roamPos);
                        }
                        //FindTarget();
                        break;
                    
                    case State.Chase:
                        if(sqrDistance < Mathf.Pow(forgetPlayerDistance, 2)){
                            //rb.velocity = dirToPlayer.normalized * moveSpeed;
                            followPlayerPath = true;
                        }
                        else{
                            currentState = State.ReturnToStart;
                            UpdatePath(startPos);
                        }
                        break;
                    
                    case State.ReturnToStart:
                        Vector2 dirToStart = (Vector2)(startPos - transform.position);
                        //rb.velocity = dirToStart.normalized * moveSpeed;
                        
                        if(dirToStart.sqrMagnitude < Mathf.Pow(reachedPointDistance, 2)){
                            roamPos = GetRoamingPosition();
                            currentState = State.Wander;
                            UpdatePath(roamPos);
                        }
                        //FindTarget();
                        break;

                    case State.Stunned:
                        currentStunPercentage += Time.fixedDeltaTime * stunInverseTime;
                        rb.velocity = Vector2.Lerp(stunDirection, Vector2.zero, currentStunPercentage);
                        if(currentStunPercentage >= 1){
                            currentState = State.Chase;
                            weapon.StartPause();
                        }
                        break;
                }
                if(followPlayerPath && nextPathTime < Time.time){
                    UpdatePath(playerT.position);
                    nextPathTime = Time.time + pathfindingCheckTime;
                }
                if(followPlayerPath || currentState == State.Wander || currentState == State.ReturnToStart){
                    FollowPath();
                }
            }
            Vector2 lookDir = rb.velocity;
            if(currentState == State.Chase){
                lookDir = (Vector2)playerT.position - rb.position;
            }
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            weaponRotator.eulerAngles = new Vector3(0, 0, angle);
            if(lookDir.x < 0 && facingRight){
            Flip();
            }
            else if(lookDir.x > 0 && !facingRight){
                Flip();
            }
        }
        else{
            currentState = State.Victory;
            rb.velocity = Vector2.zero;
        }
        animator.SetBool("SeePlayer", currentState == State.Attack || currentState == State.Chase);
        animator.SetBool("IsHurt", currentState == State.Stunned);
        animator.SetBool("EnemyWon", currentState == State.Victory);
    }

    void FollowPath(){
        if(path == null){
            return;
        }
        if(currentWaypoint >= path.vectorPath.Count){
            reachedEndOfPath = true;
            return;
        }
        else{
            reachedEndOfPath = false;
        }
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
        Vector2 force = direction * moveSpeed * Time.deltaTime;
        rb.velocity = force;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if(distance < nextWaypointDistance){
            currentWaypoint++;
        }
    }

    public override void TakeHit(float damage, Vector3 hitpoint, Vector3 hitDirection)
    {
        
        if(damage >= health && !isDead){
            AudioManagerScript.instance.PlaySound2D("Enemy Death");
            //Destroy(Instantiate(deathEffect.gameObject, hitpoint, Quaternion.FromToRotation(Vector3.up, hitDirection)) as GameObject, deathEffect.startLifetime);
        }
        else{
            AudioManagerScript.instance.PlaySound2D("Impact");
        }
        
        if(stunTime > 0){
            currentState = State.Stunned;
            stunDirection = hitDirection.normalized * knockback;
            currentStunPercentage = 0;
            rb.velocity = stunDirection;
            weapon.CancelAttack();
        }
        else{
            animator.SetTrigger("Damaged");
            currentState = State.Chase;
        }
        
        base.TakeHit(damage, hitpoint, hitDirection);
    }

    void OnPathComplete(Path p){
        if(!p.error){
            path = p;
            currentWaypoint = 0;
        }
    }

    void UpdatePath(Vector3 target){
        Vector3 dirToTarget = (target - transform.position).normalized;
        Vector3 targetPos = target - dirToTarget;
        seeker.StartPath(rb.position, targetPos, OnPathComplete);
    }

    private Vector3 GetRoamingPosition(){
        //Vector3 randDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        //return startPos + randDir * Random.Range(1f, 10f);
        return levelGenerator.GetRandFloorCoordNearPoint(startPos);
    }

    private void FindTarget(){
        //Debug.Log((playerT.position - transform.position).sqrMagnitude);
        if(((Vector2)(playerT.position - transform.position)).sqrMagnitude < Mathf.Pow(viewPlayerDistance, 2)){
            //Debug.Log("Hey");
            currentState = State.Chase;
        }
    }

    public void SetCharacteristics(float newSpeed, float newHealth, float newDamage){
        moveSpeed = newSpeed;
        health = newHealth;
        weapon.SetDamage(newDamage);
    }

    void Flip(){
        facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = enemyGFX.localScale;
		theScale.x *= -1;
		enemyGFX.localScale = theScale;
        //weapon.Flip();
    }
}
