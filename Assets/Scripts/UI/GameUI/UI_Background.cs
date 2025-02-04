using UnityEngine;

public class UI_Background : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PuzzleErrored() 
    {
        animator.SetTrigger("Error");
    }

    public void PuzzleComplete()
    {
        animator.SetTrigger("Completed");

    }
}
