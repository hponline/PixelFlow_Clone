using NaughtyAttributes;
using UnityEngine;

public class TurretAnimController : MonoBehaviour
{
    static readonly int IdleHash = Animator.StringToHash("Idle");
    static readonly int AttackHash = Animator.StringToHash("Attack");

    [SerializeField] Animator animator;

    private void Start()
    {
        animator.Play(IdleHash, 0, Random.Range(0, 1f));
    }

    public void Attack()
    {
        animator.SetTrigger(AttackHash);
    }
}
