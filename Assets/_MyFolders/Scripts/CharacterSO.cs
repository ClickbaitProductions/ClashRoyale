using UnityEngine;

[CreateAssetMenu]
public class CharacterSO : ScriptableObject
{

    public GameObject prefab;
    public float walkingSpeed;
    public int damage;
    public float attackRange;
    public int elixerCost;

}
