using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity Data", menuName = "EntityData/NewData")]
public class EntityDataSO : ScriptableObject
{
    public float baseMaxHP, baseATK, baseATKSpeed;
    public float baseCritChance, baseCritDMG;
    public float baseDEF, baseDodgeRate;
    public int ATKRank;
}
