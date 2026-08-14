using UnityEngine;

public class Skill_SwordThrow : Player_SkillBase
{
    private SkillObject_Sword currentSword;
    [Header("Regular Sword Upgrade")]
    [SerializeField] private GameObject swordPrefab;

    [Range(0, 5)]
    [SerializeField] public float throwPower = 2.5f;

    [Header("Sword Pierce Upgrade")]
    [SerializeField] private GameObject pierceSwordPrefab;

    [Header("Sword Spin Upgrade")]
    [SerializeField] private GameObject spinSwordPrefab;
    public int maxDistance = 5;
    public int attackFrequence = 4; // number of times damage trigger per second
    public float maxSpinDuration = 3f;

    [Header("Bounce Sword Upgrade")]
    [SerializeField] private GameObject bounceSwordPrefab;
    public int bounceCount = 2;
    public float bounceSpeed = 12f;

    [Header("Trajectory prediction")]
    [SerializeField] private GameObject predictionDot;
    [SerializeField] private int dotCount = 20; // number of dots
    [SerializeField] private float spaceBetweenDots = .05f; // space between dots
    private Transform[] dots; // transfer all dots collected into an array to change their places ...
    private Vector2 confirmedDirection;
    private float swordGravityScale;

    protected override void Awake()
    {
        base.Awake();

        swordGravityScale = swordPrefab.GetComponent<Rigidbody2D>().gravityScale;
        dots = GenerateDots();
    }

    public override bool CanUseSkill()
    {
        if (currentSword != null)
        {
            currentSword.GetSwordBack();
            return false;
        }
        return base.CanUseSkill();
    }

    private GameObject GetSwordPrefab()
    {
        if (UnlockedSkill(SkillUpgradeType.SwordThrow))
            return swordPrefab;
        if (UnlockedSkill(SkillUpgradeType.SwordThrow_Pierce))
            return pierceSwordPrefab;
        if (UnlockedSkill(SkillUpgradeType.SwordThrow_Spin))
            return spinSwordPrefab;
        if (UnlockedSkill(SkillUpgradeType.SwordThrow_Bounce))
            return bounceSwordPrefab;

        Debug.Log("haven't set or unlock any upgrade type yet!");
        return null;
    }

    public void ThrowSword()
    {
        GameObject swordPrefab = GetSwordPrefab();
        // use dots[1].position as the position to create the sword so the sword not looks like it's coming out of player's body
        GameObject newSword = Instantiate(swordPrefab, dots[1].position, Quaternion.identity);

        currentSword = newSword.GetComponent<SkillObject_Sword>();
        currentSword.SetUpSword(this, GetThrowPower());
    }

    private Vector2 GetThrowPower() => confirmedDirection * throwPower * 10;

    public void PredictTrajectory(Vector2 direction)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].position = GetTrajectoryPoint(direction, i * spaceBetweenDots);
        }
    }

    private Vector2 GetTrajectoryPoint(Vector2 direction, float t)
    {
        float scaleThrowPower = throwPower * 10;

        //initial velocity and direction of the throw
        Vector2 initialVelocity = direction * scaleThrowPower;

        //(1/2)g.t^2 | Physics2D.gravity = g = (0, -9.81) . add swordGravityScale so it flies and falls faster
        Vector2 gravityEffect = 0.5f * Physics2D.gravity * (t * t) * swordGravityScale;

        // predicted point where sword will lie at the end. S = v.​t + (1/2)​a.t^2    ( it's 9th grade physics, you cute little dumb fuck)
        Vector2 predictedPoint = (initialVelocity * t) + gravityEffect;

        // transform.root = highest parent of the gameObject this script attached to
        Vector2 playerPosition = transform.root.position;

        return playerPosition + predictedPoint;
    }

    public void ConfirmTrajectory(Vector2 direction) => confirmedDirection = direction;

    public void EnableDots(bool enable)
    {
        foreach (Transform t in dots)
            t.gameObject.SetActive(enable);
    }

    private Transform[] GenerateDots()
    {
        Transform[] newDots = new Transform[dotCount];
        for (int i = 0; i < dotCount; i++)
        {
            newDots[i] = Instantiate(predictionDot, transform.position, Quaternion.identity, transform).transform; // creates dots prefab clones and attach it to the gameObject called this script (all 20 dots at the same initiate position)
            newDots[i].gameObject.SetActive(false); // deactive them all ( setActive when trying to aim later)
        }

        return newDots;
    }
}
