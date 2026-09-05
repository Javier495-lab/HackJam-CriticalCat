using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class Chunk : MonoBehaviour
{
	[SerializeField] private Transform startPoint;
	[SerializeField] private Transform endPoint;
	[SerializeField] private SplineContainer splineContainer;
    public List<DragableAnimation> listaDeInteractuables = new List<DragableAnimation>();

    private void Awake()
	{
		if (splineContainer == null)
		{
			splineContainer = GetComponentInChildren<SplineContainer>();
		}
	}
    private void Start()
    {
        DragableAnimation[] componentes = GetComponentsInChildren<DragableAnimation>();

        listaDeInteractuables = new List<DragableAnimation>(componentes);
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

	public void OnEnable()
	{
		foreach (DragableAnimation interactivo in listaDeInteractuables)
		{
			interactivo.ResetState();
		}
	}
}
