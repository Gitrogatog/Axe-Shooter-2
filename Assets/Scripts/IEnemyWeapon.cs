using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyWeapon
{
    public void Attack();
    public void StartPause();
    public bool IsAttacking();
    public void CancelAttack();
    public void ReceiveAngle(float angle);
    public void SetDamage(float newDamage);
    public void Flip();
}
