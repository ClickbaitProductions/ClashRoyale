using Fusion;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AgentWalker : NetworkBehaviour
{

    public event Action OnWalk;
    public event Action OnStop;

    [SerializeField] float attackRange = 5;
    [SerializeField] float walkingSpeed = 4;

    [SerializeField] Image healthBarFill;
    [SerializeField] Color redHealthBarCol;

    [Networked] PlayerType type { get; set; }

    Transform leftEnemyTower;
    Transform rightEnemyTower;

    NavMeshAgent agent;

    Vector3 target;
    float targetThreshold = 2;

    bool isWalking = false;
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

        agent.speed = walkingSpeed;
        Vector3 eulerAngles = transform.eulerAngles;

        if (type == PlayerType.Blue)
        {
            gameObject.name = "BlueKnight";

            eulerAngles.y = 180;
            leftEnemyTower = Singleton.Instance.leftRedTower;
            rightEnemyTower = Singleton.Instance.rightRedTower;
        } else if (type == PlayerType.Red)
        {
            gameObject.name = "RedKnight";
            gameObject.tag = "Red";
            healthBarFill.color = redHealthBarCol;

            eulerAngles.y = 0;
            leftEnemyTower = Singleton.Instance.leftBlueTower;
            rightEnemyTower = Singleton.Instance.rightBlueTower;
        }

        transform.eulerAngles = eulerAngles;
    }

    Vector3 GetNearestTower()
    {
        if (transform.position.x >= 0)
            return leftEnemyTower.position;
        else
            return rightEnemyTower.position;
    }

    void CalculateTarget()
    {
        Vector3 tower = GetNearestTower();
        tower.y = transform.position.y;

        target = tower;
    }

    void SetTarget()
    {
        agent.SetDestination(target);
        OnWalk?.Invoke();
        isWalking = true;
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
            if (isWalking)
            {
                if (agent.remainingDistance < targetThreshold + attackRange)
                    StopAgent();
            }
        }
    }
    
    void StopAgent()
    {
        agent.isStopped = true;
        OnStop?.Invoke();
        isWalking = false;
    }

}
