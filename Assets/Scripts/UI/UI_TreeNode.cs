using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;
    private UI_TreeConnectHandler connectHandler;

    [Header("Unlock Details")]
    public bool isUnlocked;
    public bool isLocked;
    public UI_TreeNode[] neededNodes;
    public UI_TreeNode[] conflictNodes;

    [Header("Skill Details")]
    public Player_SkillDataSO player_SkillData;
    [SerializeField] private string skillName;
    [SerializeField] private int skillCost;
    [SerializeField] private Image skillIcon;
    [SerializeField] private string lockedColorHex = "#5F5858";
    private Color lastIconColor;

    private void OnValidate()
    {
        if (player_SkillData == null)
            return;

        skillName = player_SkillData.skillName;
        skillCost = player_SkillData.cost;
        skillIcon.sprite = player_SkillData.icon;
        gameObject.name = "UI_TreeNode - " + player_SkillData.skillName;
    }


    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();
        connectHandler = GetComponent<UI_TreeConnectHandler>();

        UpdateIconColor(GetColorByHex(lockedColorHex));
    }

    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        LockConflictNode();

        skillTree.UseSkillPoints(player_SkillData.cost);
        connectHandler.UnlockConnectionImage(true);
    }

    public void Refund()
    {
        if (isUnlocked)
            skillTree.AddSkillPoints(player_SkillData.cost);
        isLocked = false;
        isUnlocked = false;
        UpdateIconColor(GetColorByHex(lockedColorHex));

        connectHandler.UnlockConnectionImage(false);
        // skill manager and reset skill
    }

    private void LockConflictNode()
    {
        foreach (var node in conflictNodes)
            node.isLocked = true;
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
            return;

        lastIconColor = skillIcon.color;
        skillIcon.color = color;
    }

    // private bool CanBeUnlocked()
    // {
    //     if (isLocked || isUnlocked)
    //         return false;

    //     if (!skillTree.EnoughSkillPoints(player_SkillData.cost))
    //         return false;

    //     foreach (var node in neededNodes)
    //     {
    //         if (!node.isUnlocked)
    //             return false;
    //     }

    //     foreach (var node in conflictNodes)
    //     {
    //         if (node.isUnlocked)
    //             return false;
    //     }

    //     return true;
    // }

    // ultimate shortest version: ( only return true if all conditions are met)
    private bool CanBeUnlocked() => !isLocked && !isUnlocked && skillTree.EnoughSkillPoints(player_SkillData.cost) && neededNodes.All(node => node.isUnlocked) && conflictNodes.All(node => !node.isUnlocked);

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
            Unlock();
        else
            ui.skillToolTip.LockedSkillEffect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, this);

        if (isUnlocked || isLocked)
            return;

        Color color = Color.white * .9f;
        color.a = 1; // a = alpha as in RGBA, which control transparency
        UpdateIconColor(color);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);
        if (isUnlocked || isLocked)
            return;

        UpdateIconColor(lastIconColor);
    }

    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);
        return color;
    }

    private void OnDisable()
    {
        if (isLocked)
            UpdateIconColor(GetColorByHex(lockedColorHex));
        if (isUnlocked)
            UpdateIconColor(Color.white);
    }
}
