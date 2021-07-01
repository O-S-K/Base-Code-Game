using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public float horizontalMove = 0f;

    public CharacterController2D controller;

    public float runSpeed = 40f;
    bool jump = false;
    bool crouch = false;

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            crouch = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            crouch = false;
        }

        controller.Move(horizontalMove * Time.deltaTime, crouch, jump);
        jump = false;
    }
}
