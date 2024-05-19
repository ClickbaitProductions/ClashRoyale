using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{

    Animator anim;
    AgentWalker agentWalker;

    void Start()
    {
        anim = GetComponent<Animator>();
        agentWalker = GetComponent<AgentWalker>();

        agentWalker.IsWalking.AddListener(OnWalkingToggle);
    }

    void OnWalkingToggle(bool isWalking)
    {
        anim.SetBool("Attacking", !isWalking);
    }

}
