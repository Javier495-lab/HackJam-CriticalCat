using System.Collections;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
	[SerializeField] private PlayerSplineFollower playerSplineFollower; 
	[SerializeField] private Animator animator;
	private bool isJumping = false;

	private void Update()
	{
		animator.SetFloat("Velocity", playerSplineFollower.GetSpeedMultiplier());
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "JumpTrigger")
		{
			if (!isJumping)
			{
				isJumping = true;
				animator.SetTrigger("Jump");
			}
			else
			{
				isJumping = false;
				animator.SetTrigger("Land");
			}

		}
	}
}
