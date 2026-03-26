using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "File/StatFile")]
public class StatFileData : ScriptableObject
{
    public int fileIndex;
    public string fileName;
    public float attackPowerBonus;
    public float moveSpeedBonus;
    public float defenseBonus;
    public float criticalChanceBonus;
    public float criticalDamageBonus;
    public float maxHpBonus;
    public float hpBonus;
    public float hpRegenBonus;
}