using UnityEngine;

// put this on the player (same object as the PlayerController)
// it reads how the CharacterController is moving and passes it to the cat's Animator
public class CatAnimation : MonoBehaviour
{
    public Animator animator;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // the Animator sits on the cat model, which is a child of the player
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        // only use left/right/forward/back speed for walking, not falling speed
        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0f;

        animator.SetFloat("Speed", horizontalVelocity.magnitude);
        animator.SetFloat("VerticalSpeed", controller.velocity.y);
        animator.SetBool("IsGrounded", controller.isGrounded);
    }
}
