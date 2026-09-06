using UnityEngine;

public class InfiniteFloor : MonoBehaviour
{
	[SerializeField] private float scrollVelocity;
	[SerializeField] private Material myMat;

	private void Start()
	{
		myMat = GetComponent<Renderer>().material;
	}
	private void Update()
	{
		UpdateScrollVelocity();
	}

	private void UpdateScrollVelocity()
	{
		scrollVelocity = LevelManager.Instance.GetCurrentLevelVelocity();
		myMat.SetFloat("_ScrollVelocity", scrollVelocity / 11f);
	}
}
