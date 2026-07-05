using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill Data - ")]

public class Player_SkillDataSO : ScriptableObject
{
    public int cost;
    [Header("Skill Details")]
    public string skillName;
    [TextArea]
    public string description;
    public Sprite icon;
}
