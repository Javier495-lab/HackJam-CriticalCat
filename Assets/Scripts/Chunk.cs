using UnityEngine;

public class Chunk : MonoBehaviour
{
	[SerializeField] private Transform startPoint;
	[SerializeField] private Transform endPoint;

	void Update()
	{
	}

	public Vector3 GetStartPoint()
	{
		return startPoint.position;
	}

	public Vector3 GetEndPoint()
	{
		return endPoint.position;
	}

	public void Move(float speed)
	{
		transform.Translate(Vector3.right * speed * Time.deltaTime);
	}
}
