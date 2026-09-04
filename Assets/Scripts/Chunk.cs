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

	public Spline GetSpline()
	{
		return splineContainer != null ? splineContainer.Spline : null;
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
