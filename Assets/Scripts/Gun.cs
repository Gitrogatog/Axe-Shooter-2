using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;
    public Bullet bullet;
    public float msBetweenShots = 100;
    public float muzzleVelocity= 35;
    public float randomAngle;
    float nextShotTime;

     public enum FireMode {Auto, Burst, Single};
    public FireMode fireMode;
    public Transform[] projectileSpawns;
    public int burstCount;
    //public int clipSize = 5;
    //public float reloadTime = 2f;
    //public float maxReloadAngle;
    WeaponsManager weaponsManager;
    public float chargePerShot = 10f;
    public float damage = 1f;


    [Header("Effects")]
    public Transform shell;
    public Transform shellEjector;
    public AudioClip shootAudio;

    //MuzzleFlashScript muzzleFlash;

    bool triggerReleasedSinceLastShot = true;
    int shotsRemainingInBurst;
    int projectilesRemaining = 1;
    bool isReloading = false;
    public float shakeDuration;
    public float shakeMagnitude;

    Vector2 recoilSmoothDampVelocity;
    [Header("Recoil")]
    public Vector2 recoilMinMax = new Vector2(0.2f, 0.3f);
    public float recoilSettleTime = 0.1f;

    void Awake(){
        weaponsManager = FindObjectOfType<WeaponsManager>();
    }
    

    public void Shoot()
    {
        if (Time.time > nextShotTime && weaponsManager.GetCurrentCharge() >= chargePerShot) {
            if(fireMode == FireMode.Burst){
                if(shotsRemainingInBurst <= 0){
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if(fireMode == FireMode.Single){
                if(!triggerReleasedSinceLastShot){
                    return;
                }
            }
            for(int i = 0; i < projectileSpawns.Length; i++){
                if(projectilesRemaining <= 0){
                    break;
                }
                Quaternion bulletRotation = Quaternion.identity;
                bulletRotation.eulerAngles = new Vector3(0, 0, projectileSpawns[i].rotation.eulerAngles.z + Random.Range(-randomAngle, randomAngle));
                Bullet newBullet = Instantiate(bullet, projectileSpawns[i].position, bulletRotation) as Bullet;
                newBullet.SetCharacteristics(muzzleVelocity, true, damage);
                //projectilesRemaining--;
            }
            CameraScript.instance.Shake(shakeDuration, shakeMagnitude);
            if(shootAudio != null){
                AudioManagerScript.instance.PlaySound(shootAudio, transform.position);
            }

            weaponsManager.DechargeWeapon(chargePerShot);

            nextShotTime = Time.time + msBetweenShots / 1000;

            Destroy(Instantiate(shell, shellEjector.position, shellEjector.rotation).gameObject, 3f);
            //muzzleFlash.Activate();
            transform.localPosition -= Vector3.up * Random.Range(recoilMinMax.x, recoilMinMax.y);

            //AudioManagerScript.instance.PlaySound(shootAudio, transform.position);

            //nextShotTime = Time.time + msBetweenShots / 1000;
            //Bullet newBullet = Instantiate(bullet, muzzle.position, muzzle.rotation) as Bullet;
            //newBullet.SetCharacteristics(muzzleVelocity, true);
        }
    }

    void Update(){
        transform.localPosition = Vector2.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilSettleTime);
    }

    public void Aim(Vector3 aimPoint){
        if(!isReloading){
            Vector2 lookDir = aimPoint - transform.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    public void OnTriggerHold(){
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease(){
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}
