using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMonster : MonoBehaviour, IDamageable
{
    #region Entity Data
    [SerializeField] EntityDataSO entityDataSO;
    public EntityDataSO DataSO => this.entityDataSO;

    public int level = 0;
    public float curMaxHP, curATK, curATKSpeed;
    public float curCritChance, curCritDMG;
    public float curDEF, curDodgeRate;

    public float curHP;
    #endregion

    void Start()
    {
        level = 1;
        InitData();
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
    }

    private void InitData()
    {
        curMaxHP = DataSO.baseMaxHP * level;
        curATK = DataSO.baseATK * level;
        curATKSpeed = DataSO.baseATKSpeed * level;
        curCritChance = DataSO.baseCritChance * level;
        curCritDMG = DataSO.baseCritDMG * level;
        curDEF = DataSO.baseDEF * level;
        curDodgeRate = DataSO.baseDodgeRate * level;

        curHP = curMaxHP;
    }

    public void TakeDamage(float damage, float def)
    {
        curHP = curHP - damage;
        Debug.Log(curHP);
    }

    public void CauseDamage(float atk, float critChange, float critRate)
    {
        throw new System.NotImplementedException();
    }

    public bool BeDodged(float dodgeRate)
    {
        throw new System.NotImplementedException();
    }
}
