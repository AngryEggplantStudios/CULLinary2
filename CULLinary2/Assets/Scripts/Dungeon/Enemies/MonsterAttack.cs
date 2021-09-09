using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class MonsterAttack : MonoBehaviour
{
    [SerializeField] public int attackDamage;

    abstract public void attackPlayerStart();
    abstract public void attackPlayerDealDamage();
    abstract public void attackPlayerEnd();


}
