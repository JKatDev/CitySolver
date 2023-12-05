using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;

public class PlayerController : Singleton<PlayerController>
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Transform movePoint;

    private const float initialPosX = 0.5f;
    private const float stepMovement = 1f;
    private bool canMove = true; // Flag to control the player's movement on update
    private bool isFrozen = false; // Flag to control if player is frozen in place temporarely or not

    void Awake()
    {
        movePoint.position = new Vector2(initialPosX, 0); // Set movepoint initial position
        transform.position = movePoint.position; // Set player initial position
        movePoint.parent = null; // Make movepoint parent null so it doesn't follow player
    }

    void Update()
    {
        PlayerMovement();
    }

    #region Movement

    void PlayerMovement()
    {
        if (transform.position != movePoint.position)
        {
            // Move the player towards the movePoint at each frame
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            canMove = true;
        }

        // If the player is permitted to move, retrieves input and moves horizontally and vertically
        if (canMove && !isFrozen)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (horizontal != 0 && vertical != 0)
            {
                movePoint.position += new Vector3(stepMovement * (horizontal < 0 ? -1 : 1), stepMovement * (vertical < 0 ? -1 : 1), 0);
                canMove = false;

            }
            else if (horizontal != 0)
            {
                movePoint.position += new Vector3(stepMovement * (horizontal < 0 ? -1 : 1), 0, 0);
                canMove = false;
            }
            else if (vertical != 0)
            {
                movePoint.position += new Vector3(0, stepMovement * (vertical < 0 ? -1 : 1), 0);
                canMove = false;
            }
        }
    }

    #endregion

    #region SET

    public void SetIsFrozen(bool value)
    {
        isFrozen = value;
    }

    #endregion

}
