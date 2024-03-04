using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private Vector3 move;
    public Animator animator;
    
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
    }
    
        void Update()
        {
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
                


        }
        
        controller.Move(move * Time.deltaTime * playerSpeed);
    
            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            
            animator.SetTrigger("walk");
        }

        // // Changes the height position of the player..
        // if (Input.GetButtonDown("Jump") && groundedPlayer)
        // {
        //     playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        // }
        
        playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        //UnityEngine.Debug.Log(animator.GetFloat("Vertical"));
            
    }

        public void OnMove(InputAction.CallbackContext context)
        {
        animator.ResetTrigger("walk");
        Vector2 movement = context.ReadValue<Vector2>();
            move = new Vector3(-movement.x, 0, -movement.y);
        }
}
