using UnityEngine;
using System;

[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill Data - ")]

public class Player_SkillDataSO : ScriptableObject
{

    [Header("Skill Details")]
    public string skillName;
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Unlock and Upgrade")]
    public int cost;
    public bool unlockedByDefault;
    public SkillType skillType;
    public UpgradeData upgradeData;
}

[Serializable]
public class UpgradeData
{
    public SkillUpgradeType upgradeType;
    public float cooldown;
    public DamageScaleData damageScaleData;
}
