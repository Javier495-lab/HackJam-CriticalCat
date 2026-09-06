using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

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
