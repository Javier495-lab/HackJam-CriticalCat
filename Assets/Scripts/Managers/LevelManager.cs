using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public static LevelManager Instance;
	[SerializeField] private float levelVelocity = 5f;
	[SerializeField] private float currentLevelVelocity;
	[SerializeField] private GameObject canvas;
	[SerializeField] private RawImage canvasVideo;
	[SerializeField] private VideoPlayer videoPlayer;
	[SerializeField] private VideoClip parentsLoop;
	[SerializeField] private VideoClip introMovie;

	[Header("Hand Movement")]
	[SerializeField] private GameObject hand;
	[SerializeField] private Vector3 handOriginalPosition;
	[SerializeField] private float handReturnDuration = 0.2f;
	[SerializeField] private float handMovementReduction = 0.05f;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		SetLoopVideo();
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(this);
		}
		currentLevelVelocity = 0;
	}

	private void Start()
	{
		handOriginalPosition = hand.transform.position;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			ReloadCurrentScene();
		}
	}

	private void ReloadCurrentScene()
	{
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		Destroy(gameObject);
		SceneManager.LoadScene(currentSceneIndex);
	}

	public void MoveHand(Vector3 newPosition)
	{
		StopAllCoroutines();
		Vector3 handOffset = (newPosition - handOriginalPosition) * handMovementReduction;
		hand.transform.position = new Vector3(handOriginalPosition.x + handOffset.x,
										handOriginalPosition.y + handOffset.y,
										hand.transform.position.z);
	}

	public void ReturnHand()
	{
		StartCoroutine(LerpHandToOriginalPosition());
	}

	private IEnumerator LerpHandToOriginalPosition()
	{
		Vector3 startPosition = hand.transform.position;
		float elapsed = 0f;

		while (elapsed < handReturnDuration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / handReturnDuration);
			hand.transform.position = Vector3.Lerp(startPosition, handOriginalPosition, t);
			yield return null;
		}

		hand.transform.position = handOriginalPosition;
	}

	private void SetLoopVideo()
	{
		videoPlayer.clip = parentsLoop;
		videoPlayer.isLooping = true;
		videoPlayer.Play();
	}

	public void OnPlayButton()
	{
		currentLevelVelocity = levelVelocity;
		videoPlayer.Stop();
		videoPlayer.frame = 0;
		videoPlayer.clip = introMovie;
		videoPlayer.Play();
		videoPlayer.isLooping = false;
		videoPlayer.loopPointReached += OnVideoEnded;
	}

	private void OnVideoEnded(VideoPlayer source)
	{
		StartCoroutine(FadeOutRawImage());
	}

	private IEnumerator FadeOutRawImage()
	{
		Color startColor = canvasVideo.color;
		float duration = 1f;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / duration);
			canvasVideo.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0f), t);
			yield return null;
		}

		canvasVideo.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
		canvasVideo.enabled = false;
	}

	public float GetCurrentLevelVelocity()
	{
		return currentLevelVelocity;
	}

	public void SetCurrentLevelVelocity(float velocity)
	{
		currentLevelVelocity = velocity;
	}
}
