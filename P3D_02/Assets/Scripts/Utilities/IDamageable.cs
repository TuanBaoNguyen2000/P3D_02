using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    public void TakeDamage(float damage, float def);

    public void CauseDamage(float atk, float critChange, float critRate);

    public bool BeDodged(float dodgeRate);
}
