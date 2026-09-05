using UnityEngine;
using UnityEngine.Splines;

public class Chunk : MonoBehaviour
{
	[SerializeField] private Transform startPoint;
	[SerializeField] private Transform endPoint;
	[SerializeField] private SplineContainer splineContainer;

	private void Awake()
	{
		if (splineContainer == null)
		{
			splineContainer = GetComponentInChildren<SplineContainer>();
		}
	}

	public SplineContainer GetSpline()
	{
		return splineContainer;
	}

	public Vector3 GetStartPoint()
	{
		return startPoint.position;
	}

	public Vector3 GetEndPoint()
	{
		return endPoint.position;
	}
}
