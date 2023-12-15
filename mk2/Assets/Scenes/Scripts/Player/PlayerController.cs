using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;
    private int desireLane = 1;
    public float laneDistance = 4;
    public float jumpForce;
    public float Gravity = -20;
    public float maxSpeed;
    public Animator animator;
    private bool isSliding = false;
	void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerManager.isGameStarted)
            return;
        
        if(forwardSpeed<maxSpeed)
        forwardSpeed += 0.1f * Time.deltaTime;

        direction.z = forwardSpeed;

        if (controller.isGrounded)
        {
            direction.y = -2;
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				Jump();
               
			}
           
		}
        else
        {
			direction.y += Gravity * Time.deltaTime;
		}
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isSliding)
        {
            StartCoroutine(Slide());
        }
		if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desireLane++;
            if (desireLane == 3)
            {
                desireLane = 2;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desireLane--;
            if (desireLane == -1)
            {
                desireLane = 0;
            }
        }

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;

        if (desireLane == 0)
        {
            targetPosition += Vector3.left * laneDistance;
        }
        else if (desireLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
        }
        // transform.position =Vector3.Lerp(targetPosition,targetPosition,70*Time.deltaTime);
        if (transform.position == targetPosition)
        {
            return;
        }
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);
    }
    private void FixedUpdate()
	{
		if (!PlayerManager.isGameStarted)
			return;
		controller.Move(direction * Time.fixedDeltaTime);
	}
    private void Jump()
    {
        direction.y = jumpForce;
        
    }

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.transform.tag == "Obstacle")
        {
            PlayerManager.gameOver = true;
            FindObjectOfType<AudioManager>().PlaySound("GameOver");
        }
	}
    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetBool("isSliding", true);
        controller.center = new Vector3(0, -0.5f, 0);
        controller.height = 1;

        yield return new WaitForSeconds(1.3f);

        controller.center = new Vector3(0,0,0);
        controller.height = 2;

        animator.SetBool("isSliding", false);
        isSliding = false;
    }

}
