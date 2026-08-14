using UnityEngine;

public class Player_SkillBase : MonoBehaviour
{
    public Player player { get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public DamageScaleData damageScaleData { get; private set; }

    [Header("General Details")]
    [SerializeField] protected SkillType skillType;
    [SerializeField] protected SkillUpgradeType upgradeType;
    [SerializeField] protected float coolDown;
    private float lastTimeUsed = 0f;


    protected virtual void Awake()
    {
        player = GetComponentInParent<Player>();
        skillManager = GetComponentInParent<Player_SkillManager>();
        damageScaleData = new DamageScaleData();

        ResetCoolDown();
    }

    public virtual void TryUseSkill()
    {

    }

    public void SetSkillUpgrade(UpgradeData upgrade)
    {
        upgradeType = upgrade.upgradeType;
        coolDown = upgrade.cooldown;
        damageScaleData = upgrade.damageScaleData;
        ResetCoolDown();
    }

    protected bool UnlockedSkill(SkillUpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;

    public virtual bool CanUseSkill()
    {
        if (upgradeType == SkillUpgradeType.None)
            return false;

        if (OnCoolDown())
        {
            Debug.Log($"On Cooldown: {(lastTimeUsed + coolDown) - Time.time}");
            return false;
        }
        return true;
    }

    protected bool OnCoolDown() => Time.time < lastTimeUsed + coolDown;
    public void SetSkillOnCoolDown() => lastTimeUsed = Time.time;
    public void ReduceCoolDownBy(float coolDownReduction) => coolDown = coolDown * (1 - coolDownReduction);

    public void ResetCoolDown() => lastTimeUsed = Time.time - coolDown;
}
