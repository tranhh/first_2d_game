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

    public enum UnlockResult
    {
        success,
        alreadyUnlocked,
        notEnoughSkillPoints,
        locked,
        wrongPath
    }

    private void OnValidate()
    {
        if (player_SkillData == null)
            return;

        skillName = player_SkillData.skillName;
        skillCost = player_SkillData.cost;
        skillIcon.sprite = player_SkillData.icon;
        gameObject.name = "UI_TreeNode - " + player_SkillData.skillName;
        //UpdateIconColor(GetColorByHex(lockedColorHex));
    }


    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();
        connectHandler = GetComponent<UI_TreeConnectHandler>();

        UpdateIconColor(GetColorByHex(lockedColorHex));
    }

    private void Start()
    {
        // can't do this in awake() since there's another child function will need to be called first
        if (player_SkillData.unlockedByDefault)
        {
            Unlock();
        }
    }

    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        LockConflictNode();

        skillTree.UseSkillPoints(player_SkillData.cost);
        connectHandler.UnlockConnectionImage(true);

        skillTree.skillManager.GetSkillByType(player_SkillData.skillType).SetSkillUpgrade(player_SkillData.upgradeData);
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
            node.LockChildNodes();

    }

    public void LockChildNodes()
    {
        isLocked = true;
        // foreach child node found, that child node also has their childnodes, so we have to make a loop
        foreach (var node in connectHandler.GetChildNodes())
            node.LockChildNodes();
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
            return;

        lastIconColor = skillIcon.color;
        skillIcon.color = color;
    }

    private UnlockResult CanBeUnlocked()
    {
        if (isUnlocked)
            return UnlockResult.alreadyUnlocked;

        if (conflictNodes.Any(node => node.isUnlocked))
            return UnlockResult.wrongPath;

        if (!skillTree.EnoughSkillPoints(player_SkillData.cost))
            return UnlockResult.notEnoughSkillPoints;

        if (isLocked || !neededNodes.All(node => node.isUnlocked))
            return UnlockResult.locked;


        return UnlockResult.success;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UnlockResult result = CanBeUnlocked();

        switch (result)
        {
            case UnlockResult.alreadyUnlocked:
                return;

            case UnlockResult.success:
                Unlock();
                break;

            case UnlockResult.wrongPath:
            case UnlockResult.locked:
            case UnlockResult.notEnoughSkillPoints:
                ui.skillToolTip.LockedSkillEffect(result);
                break;
        }
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
        ui.skillToolTip.StopTextEffect();
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
