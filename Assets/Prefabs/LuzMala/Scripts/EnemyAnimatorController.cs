using UnityEngine;


/// <summary>
/// Es un listener de los eventos pueden presentarse para LuzMala 
/// y manejará el Animator
/// </summary>
[RequireComponent(typeof(Animator))]
public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;

    [Header("Parámetros del Animator")]
    [SerializeField] private string walkParameter = "isWalking";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartWalking()
    {
        animator.SetBool(walkParameter, true);
    }

    public void StopWalking()
    {
        animator.SetBool(walkParameter, false);
    }
}
