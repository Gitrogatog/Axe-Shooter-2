using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    Gun equippedGun;
    public Gun[] guns;
    private List<Gun> createdGuns;

    public Transform gunHold;
    bool gunEnabled = false;
    int currentGun = 0;

    private void Awake()
    {
        createdGuns = new List<Gun>();
        foreach(Gun gunToAdd in guns){
            equippedGun = Instantiate(gunToAdd, gunHold.position, gunHold.rotation) as Gun;
            equippedGun.transform.parent = gunHold;
            createdGuns.Add(equippedGun);
            equippedGun.gameObject.SetActive(false);
        }
        if (guns.Length >= 1)
        {
            EquipGun(0);
        }
    }

    public void EquipGun(int gunToEquip)
    {
        /*
        if (equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, gunHold.position, gunHold.rotation) as Gun;
        equippedGun.transform.parent = gunHold;
        */
        createdGuns[currentGun].gameObject.SetActive(false);
        createdGuns[gunToEquip].gameObject.SetActive(true);
        currentGun = gunToEquip;
        equippedGun = createdGuns[gunToEquip];
        equippedGun.OnTriggerRelease();
    }
    /*
    public void EquipGun(int gunIndex){
        currentGun = gunIndex;
        EquipGun(guns[gunIndex]);
    } */


    public void Aim(Vector3 aimPoint){
        if(equippedGun != null){
            equippedGun.Aim(aimPoint);
        }
    }

    public void OnTriggerHold(){
        if(equippedGun != null){
            equippedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease(){
        if(equippedGun != null){
            equippedGun.OnTriggerRelease();
        }
    }

    public void Shoot()
    {
        if (equippedGun != null)
        {
            equippedGun.Shoot();
        }
    }

    public void EnableGraphics(){
        equippedGun.gameObject.SetActive(true);
    }

    public void DisableGraphics(){
        equippedGun.gameObject.SetActive(false);
    }

    public void IncrementHeldGun(){
        int theGun = currentGun + 1;
        if(theGun >= guns.Length){
            theGun = 0;
        }
        EquipGun(theGun);
    }

    public void DecrementHeldGun(){
        int theGun = currentGun - 1;
        if(theGun < 0){
            theGun = guns.Length - 1;
        }
        EquipGun(theGun);
    }

    public void FlipGuns(){
        foreach(Gun gun in createdGuns){
            Vector3 theScale = gun.transform.localScale;
            theScale.x *= -1;
            gun.transform.localScale = theScale;
        }
    }
}
