using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    private UI ui;
    private UI_SkillTree skillTree;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;

    [Space]
    [SerializeField] private string metConditionHex;
    [SerializeField] private string notMetConditionHex;
    [SerializeField] private string importantInforHex;
    [SerializeField] private string lockedSkillText = "you've taken a different path - this skill is now locked.";
    [SerializeField] private Color exampleColor;

    private Coroutine textEffectCo;
    protected override void Awake()
    {
        base.Awake();
        ui = GetComponentInParent<UI>();
        skillTree = ui.GetComponentInChildren<UI_SkillTree>(true); // add "true" so that even when the gameObject gets disbaled in hierachy, it can still get its components
    }

    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);
    }

    public void ShowToolTip(bool show, RectTransform targetRect, UI_TreeNode node)
    {
        base.ShowToolTip(show, targetRect);
        if (!show)
            return;

        skillName.text = node.player_SkillData.skillName;
        skillDescription.text = node.player_SkillData.description;

        string skillLockedText = $"<color={importantInforHex}>{lockedSkillText} </color>";
        string requirements = node.isLocked ? skillLockedText : GetRequirements(node.player_SkillData.cost, node.neededNodes, node.conflictNodes);
        skillRequirements.text = requirements;
    }

    public void LockedSkillEffect()
    {
        if (textEffectCo != null)
            StopCoroutine(textEffectCo);

        textEffectCo = StartCoroutine(TextBlinkEffectCo(skillRequirements, .15f, 3));
    }

    private IEnumerator TextBlinkEffectCo(TextMeshProUGUI text, float blinkInterval, int blinkCount)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            text.text = GetColoredText(notMetConditionHex, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);

            text.text = GetColoredText(importantInforHex, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private string GetRequirements(int skillcost, UI_TreeNode[] neededNodes, UI_TreeNode[] conflictNodes)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Requirements: ");
        string costColor = skillTree.EnoughSkillPoints(skillcost) ? metConditionHex : notMetConditionHex;

        sb.AppendLine($"<color={costColor}>- {skillcost} skill point(s) </color>");

        foreach (var node in neededNodes)
        {
            string nodeColor = node.isUnlocked ? metConditionHex : notMetConditionHex;
            sb.AppendLine($"<color={nodeColor}>- {node.player_SkillData.skillName}</color>");
        }

        if (conflictNodes.Length <= 0)
            return sb.ToString();

        sb.AppendLine(); // add spacing
        sb.AppendLine($"<color={importantInforHex}>- Locks Out: </color>");

        foreach (var node in conflictNodes)
        {
            sb.AppendLine($"<color={importantInforHex}>- {node.player_SkillData.skillName}</color>");
        }

        return sb.ToString();
    }
}
