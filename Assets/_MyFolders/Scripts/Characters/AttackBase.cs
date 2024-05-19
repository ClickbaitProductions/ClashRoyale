using UnityEngine;

public class AttackBase : MonoBehaviour
{

    public Observer<Health> TargetHealth = new Observer<Health>(null);

    CharacterSO characterSO;
    bool swordIsCollidingWithTarget = false;

    public void Initialize(Health target, CharacterSO _characterSO)
    {
        TargetHealth.Value = target;
        characterSO = _characterSO;
    }

    // Gets called by animation event
    public void HitFrame()
    {
        if (swordIsCollidingWithTarget)
        {
            DealDamage();
        }
    }

    void DealDamage()
    {
        TargetHealth.Value.TakeDamage(characterSO.damage);
    }

    public void SetIfSwordIsColliding(bool isColliding)
    {
        swordIsCollidingWithTarget = isColliding;
    }

}
