using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AgentWalker : NetworkBehaviour
{

    public Observer<bool> IsWalking = new Observer<bool>(false);

    [SerializeField] CharacterSO characterSO;

    [SerializeField] Image healthBarFill;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Transform canvasTransform;
    [SerializeField] Color redHealthBarCol;
    AttackBase attack;

    [Networked] PlayerType type { get; set; }

    Health leftEnemyTowerHealth;
    Health rightEnemyTowerHealth;

    Health targetHealth;
    Vector3 targetPos;

    NavMeshAgent agent;
    
    bool isFirstWalkingFrame = true;

    public void Init(PlayerType playerType)
    {
        type = playerType;
    }

    public override void Spawned()
    {
        InitializeAgent();
        CalculateTarget();
        SetTarget();
    }

    void InitializeAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        attack = GetComponent<AttackBase>();

        agent.speed = characterSO.walkingSpeed;
        Vector3 eulerAngles = transform.eulerAngles;

        if (type == PlayerType.Blue)
        {
            if (HasStateAuthority)
            {
                // I'm blue and my guy is going to invade
                Vector3 rot = canvasTransform.localEulerAngles;
                rot.y = 180;
                canvasTransform.localEulerAngles = rot;

                Vector3 pos = canvasTransform.localPosition;
                pos.z = 0.57f;
                canvasTransform.localPosition = pos;
            } else
            {
                // I'm red and blue guy is invading
                Vector3 rot = canvasTransform.localEulerAngles;
                rot.y = 0;
                canvasTransform.localEulerAngles = rot;

                Vector3 pos = canvasTransform.localPosition;
                pos.z = -1.67f;
                canvasTransform.localPosition = pos;
            }

            gameObject.name = "BlueKnight";

            eulerAngles.y = 180;
            leftEnemyTowerHealth = Singleton.Instance.leftRedTower.GetComponent<Health>();
            rightEnemyTowerHealth = Singleton.Instance.rightRedTower.GetComponent<Health>();
        } else if (type == PlayerType.Red)
        {
            if (!HasStateAuthority)
            {
                // It's the red local player on the client side
                Vector3 rot = canvasTransform.localEulerAngles;
                rot.y = 180;
                canvasTransform.localEulerAngles = rot;

                Vector3 pos = canvasTransform.localPosition;
                pos.z = 1.75f;
                canvasTransform.localPosition = pos;
            } else
            {
                // I'm blue and red guy is invading
                Vector3 rot = canvasTransform.localEulerAngles;
                rot.y = 0;
                canvasTransform.localEulerAngles = rot;

                Vector3 pos = canvasTransform.localPosition;
                pos.z = -0.59f;
                canvasTransform.localPosition = pos;
            }

            gameObject.name = "RedKnight";
            gameObject.tag = "Red";
            healthBarFill.color = redHealthBarCol;
            healthText.color = redHealthBarCol;

            eulerAngles.y = 0;
            leftEnemyTowerHealth = Singleton.Instance.leftBlueTower.GetComponent<Health>();
            rightEnemyTowerHealth = Singleton.Instance.rightBlueTower.GetComponent<Health>();
        }

        transform.eulerAngles = eulerAngles;
    }

    Health GetNearestTower()
    {
        if (transform.position.x >= 0)
            return leftEnemyTowerHealth;
        else
            return rightEnemyTowerHealth;
    }

    void CalculateTarget()
    {
        targetHealth = GetNearestTower();
        Vector3 _towerPos = targetHealth.transform.position;
        _towerPos.y = transform.position.y;

        targetPos = _towerPos;
    }

    void SetTarget()
    {
        agent.SetDestination(targetPos);
        IsWalking.Value = true;
        isFirstWalkingFrame = true;
    }

    void Update()
    {

        if (isFirstWalkingFrame)
        {
            // For some reason remainingDist is set as 0 for the first frame
            isFirstWalkingFrame = false;
        } else
        {
            if (IsWalking.Value)
            {
                if (agent.remainingDistance < characterSO.attackRange)
                    StopAgent();
            }
        }
    }
    
    void StopAgent()
    {
        agent.isStopped = true;
        IsWalking.Value = false;
        attack.Initialize(targetHealth, characterSO);
    }

}
