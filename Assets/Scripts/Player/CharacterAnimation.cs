using System.Collections;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
	[SerializeField] private PlayerSplineFollower playerSplineFollower; 
	[SerializeField] private Animator animator;
	private float previousY;
	private bool isJumping = false;

	private void Awake()
	{
		previousY = transform.position.y;
	}

	private void Update()
	{
		animator.SetFloat("Velocity", playerSplineFollower.GetSpeedMultiplier());
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("Holita");
		if (other.tag == "JumpTrigger")
		{
			Debug.Log("Trigger");
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
