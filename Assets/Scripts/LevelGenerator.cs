using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	[Header("Chunk Lists")]
	[Tooltip("Place here all the chunk prefabs")]
	[SerializeField] private List<GameObject> ChunkList;
	[Tooltip("List of hidden but loaded chunks. There's two of each one")]
	[SerializeField] private List<GameObject> hiddenChunks;
	[Tooltip("List of active chunks being used for level generation")]
	[SerializeField] private List<GameObject> activeChunks;
	[SerializeField] private int activeChunksLenght = 6;

	[Header("Level Generation Configuration")]
	[Tooltip("Distance (x) at which the chunk is hidden")]
	[SerializeField] private float chunkDeletionDistance = -30;
	[Tooltip("Distance (x) at which the Level Generator and chunks positions reset")]
	[SerializeField] private float resetPositionDistance = -100;

	private GameObject previousChunk;

	void Start()
	{
		activeChunks = new List<GameObject>(activeChunksLenght);
		PreloadChunks();
		GenerateStartChunks();
	}

	void Update()
	{
		ResetPosition();
		InfiniteChunkGeneration();
	}

	private void ResetPosition()
	{
		if (transform.position.x <= resetPositionDistance)
		{
			Vector3 deltaPosition = new Vector3 (transform.position.x, 0, 0);
			transform.position -= deltaPosition;
			for (int i = 0; i < activeChunks.Count; i++)
			{
				activeChunks[i].transform.position += deltaPosition;
			}
		}
	}

	private void PreloadChunks()
	{
		hiddenChunks = new List<GameObject>(ChunkList.Count * 2);
		for (int i = 0; i < ChunkList.Count; i++)
		{
			for (int x = 0; x < 2; x++)
			{
				GameObject newChunk = Instantiate(ChunkList[x]);
				newChunk.SetActive(false);
				hiddenChunks.Add(newChunk);			
			}
		}
	}

	private void InfiniteChunkGeneration()
	{
		transform.Translate(Vector3.right * LevelManager.Instance.GetCurrentLevelVelocity() * -1 * Time.deltaTime);
		for (int i = 0; i < activeChunks.Count; i++)
		{
			if (activeChunks[i].transform.position.x <= chunkDeletionDistance)
			{
				RemoveActiveChunk(activeChunks[i]);
				GenerateChunk();
			}
		}
	}

	private void GenerateChunk()
	{
		GameObject newChunk = PickRandomHiddenChunk();
		AddActiveChunk(newChunk);
		PlaceNewChunk(newChunk);
	}

	private void GenerateStartChunks()
	{
		while (activeChunks.Count < activeChunksLenght && hiddenChunks.Count > 1)
		{
			GenerateChunk();
		}
	}

	private void AddActiveChunk(GameObject chunk)
	{
		chunk.transform.SetParent(transform);
		chunk.SetActive(true);
		activeChunks.Add(chunk);
		hiddenChunks.Remove(chunk);
	}

	private void RemoveActiveChunk(GameObject chunk)
	{
		chunk.SetActive(false);
		hiddenChunks.Add(chunk);
		activeChunks.Remove(chunk);
	}

	private void PlaceNewChunk(GameObject newChunk)
	{
		if (activeChunks.Count <= 1)
		{
			newChunk.transform.position = Vector3.zero;
		}
		else
		{
			newChunk.transform.position = previousChunk.GetComponent<Chunk>().GetEndPoint();
		}
		newChunk.gameObject.SetActive(true);
		previousChunk = newChunk;
	}

	private GameObject PickRandomHiddenChunk()
	{
		int randomIndex = Random.Range(0, hiddenChunks.Count);
		GameObject pickedChunk = hiddenChunks[randomIndex];
		hiddenChunks.RemoveAt(randomIndex);
		return pickedChunk;
	}
}
