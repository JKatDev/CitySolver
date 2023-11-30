using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;

    private bool canMove = true; // Flag to control the player's movement

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
        // Snap movePoint to grid
        movePoint.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        // Move the player towards the movePoint at each frame
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        // If the player is close enough to the movePoint and canMove is true, process input
        if (Vector3.Distance(transform.position, movePoint.position) <= .05f && canMove)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // If there's horizontal input, move horizontally
            if (Mathf.Abs(horizontal) == 1f)
            {
                movePoint.position += new Vector3(horizontal, 0f, 0f);
            }
            // Else if there's vertical input, move vertically
            else if (Mathf.Abs(vertical) == 1f)
            {
                movePoint.position += new Vector3(0f, vertical, 0f);
            }
        }
    }
}
