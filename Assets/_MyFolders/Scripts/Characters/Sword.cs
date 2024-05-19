using UnityEngine;

public class Sword : MonoBehaviour
{

    Observer<bool> isCollidingWithTarget = new Observer<bool>(false);

    [SerializeField] AttackBase attack;
    Health targetHealth;

    void Start()
    {
        isCollidingWithTarget.AddListener((isColliding) => attack.SetIfSwordIsColliding(isColliding));

        attack.TargetHealth.AddListener((health) =>
        {
            targetHealth = health;
            attack.SetIfSwordIsColliding(false);
        });
    }

    public void OnTriggerStay(Collider col)
    {
        if (isCollidingWithTarget.Value)
            return;

        if (targetHealth != null)
        {
            if (col.gameObject.TryGetComponent(out Health health))
            {
                if (health == targetHealth)
                {
                    isCollidingWithTarget.Value = true;
                }
            }
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (!isCollidingWithTarget.Value)
            return;

        if (targetHealth != null)
        {
            if (col.gameObject.TryGetComponent(out Health health))
            {
                if (health == targetHealth)
                    isCollidingWithTarget.Value = false;
            }
        }
    }

}
