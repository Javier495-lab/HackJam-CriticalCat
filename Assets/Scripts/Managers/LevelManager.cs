using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public static LevelManager Instance;
	[SerializeField] private float currentLevelVelocity;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);

		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(this);
		}
	}

	public float GetCurrentLevelVelocity()
	{
		return currentLevelVelocity;
	}

	public void SetCurrentLevelVelocity(float velocity)
	{
		currentLevelVelocity = velocity;
	}

	public void UnloadChunk()
	{
		
	}
}
