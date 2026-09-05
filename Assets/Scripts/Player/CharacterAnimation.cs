using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
	[SerializeField] private PlayerSplineFollower playerSplineFollower; 
	[SerializeField] private Animator animator;
	[SerializeField] private float verticalSpeed;
	[SerializeField] private float jumpThreshold = 1f;
	[SerializeField] private float fallThreshold = -1f;
	private float previousY;

	private void Awake()
	{
		previousY = transform.position.y;
	}

	private void Update()
	{
		verticalSpeed = (transform.position.y - previousY) / Time.deltaTime;
		previousY = transform.position.y;
		animator.SetFloat("Velocity", playerSplineFollower.GetSpeedMultiplier());
		TriggerAnimations();
	}

	private void TriggerAnimations()
	{
		if (verticalSpeed > jumpThreshold)
			animator.SetTrigger("Jump");
		else if (verticalSpeed < fallThreshold)
			animator.SetTrigger("Fall");
		else
			animator.SetTrigger("Run");
	}
}
