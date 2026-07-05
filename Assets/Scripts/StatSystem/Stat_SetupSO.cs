using UnityEngine;


[CreateAssetMenu(menuName = "RPG Setup/Default Stat Setup", fileName = "Default Stat Setup")]
public class Stat_SetupSO : ScriptableObject
{
    [Header("Resources Stat")]
    public float maxHealth = 100;
    public float healthRegen = 0;

    [Header("Offense - Basic Stat")]
    public float damage = 10;
    public float attackSpeed;
    public float critChance = 0;
    public float critDamage = 150;
    public float armorPenetration;

    [Header("Offense - Elemental Stat")]
    public float fireDamage;
    public float iceDamage;
    public float lightningDamage;

    [Header("Defense - Basic Stat")]
    public float armor;
    public float evasion;

    [Header("Defense - Elemental Stat")]
    public float fireRes;
    public float iceRes;
    public float lightningRes;

    [Header("Major Stat")]
    public float strength;
    public float intelligence;
    public float agility;
    public float vitality;
}

