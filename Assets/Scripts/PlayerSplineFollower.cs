using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.Splines;
using UnityEngine;
using System.Linq;

public class PlayerSplineFollower : MonoBehaviour
{
	[SerializeField] private LevelGenerator levelGenerator;
	[Header("Runner Behaviour")]
	[SerializeField] private GameObject runner;
	public float moveSpeed = 1f;
	[SerializeField] private float speedMultiplier = 1f;
	public float rotationSpeed = 5f;
	[SerializeField] private AnimationCurve velocityCurve;
	[SerializeField] private Transform xTargetPosition;
	[SerializeField] private float currentDistance = 0f;

	[Header("Splines")]
	[SerializeField] private Chunk currentChunk;
	[SerializeField] private SplineContainer spline;
	public bool isDead = false;
	
	void Start()
	{
		isDead = false;
		currentChunk = levelGenerator.GetActiveChunks()[0].GetComponent<Chunk>();
		spline = currentChunk.GetSpline();
	}

	void Update()
	{
		CalculateSpeed();
		if (isDead) return;
		PlayerFollowSpline();
	}

	private void FindNextSpline()
	{
		var activeChunks = levelGenerator.GetActiveChunks();
		if (currentDistance > 1f)
		{
			int currentIndex = activeChunks.FindIndex(chunk =>
				chunk.GetComponent<Chunk>() == currentChunk);
			int nextIndex = currentIndex + 1;
				
			SwitchToSpline(activeChunks[nextIndex].GetComponent<Chunk>());
		}
	}

	private void PlayerFollowSpline()
	{
		// Calculate the target position on the spline
		Vector3 targetPosition = spline.EvaluatePosition(currentDistance);

		// Move the character towards the target position on the spline
		runner.transform.position = Vector3.MoveTowards(runner.transform.position, targetPosition, moveSpeed * speedMultiplier * Time.deltaTime);

		// Calculate the target rotation on the spline
		Vector3 targetDirection = spline.EvaluateTangent(currentDistance);

		// Rotate the character towards the target rotation on the spline
		if (targetDirection != Vector3.zero)
		{
			Quaternion targetRotation = Quaternion.LookRotation(targetDirection, runner.transform.up);
			runner.transform.rotation = Quaternion.Slerp(runner.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		}

		// If the end of the spline is reached, loop back to the beginning
		if (currentDistance >= 1f)
		{
			FindNextSpline();
		}
		else
		{
			// Adjust the movement based on the length of the spline
			float splineLength = spline.CalculateLength();
			float movement = moveSpeed * speedMultiplier * Time.deltaTime / splineLength;
			currentDistance += movement;
		}
	}

	public void SwitchToSpline(Chunk newChunk)
	{
		if (newChunk == currentChunk) return;
		
		currentDistance = 0f;
		currentChunk = newChunk;
		spline = newChunk.GetSpline();
	}

	private void CalculateSpeed()
	{
		float deltaX = xTargetPosition.position.x - runner.transform.position.x;
		speedMultiplier = velocityCurve.Evaluate(deltaX);
	}
}