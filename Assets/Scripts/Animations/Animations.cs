using UnityEngine;

[System.Serializable]
public class Animations
{
    [SerializeField] private Animator animator;

    public void UpdateMovementAnimation(Vector2 movement)
    {
        animator.SetFloat("Xspeed", movement.x);
        animator.SetFloat("Yspeed", movement.y);

        bool isWalking = false;

        if (movement.magnitude > 0.1f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        animator.SetBool("isWalking", isWalking);
    }
    public void SetInteract(bool isInteracting)
    {
        animator.SetBool("isInteract", isInteracting);
    }
}
