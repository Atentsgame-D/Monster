using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattle
{
    Transform transform { get; }
    float DefensePower { get; }

    void Attack(IBattle target);
    void TakeDamage(float damage);
}
