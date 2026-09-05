using UnityEngine;
using System.Collections;

public class InputIndicator : MonoBehaviour
{
    [Header("Configuración del Sprite")]
    public SpriteRenderer hintSpriteRenderer; // Mantenemos tu variable pública
    //[SerializeField] private Vector3 offset = new Vector3(0, 0, 0);

    [Header("Detección")]
    [SerializeField] private string triggerTag = "Player";

    [Header("Tiempos de Animación")]
    [SerializeField] private float blinkInterval = 0.12f;
    [SerializeField] private int blinkCount = 3;
    [SerializeField] private float displayDuration = 2.5f;

    private Camera mainCamera;
    private bool isShowing = false;
    private Coroutine activeCoroutine;

    private void Awake()
    {
        mainCamera = Camera.main;

        // Si no está asignado en el Inspector, busca el SpriteRenderer en los objetos hijos automáticamente
        if (hintSpriteRenderer == null)
        {
            hintSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Start()
    {
        if (hintSpriteRenderer != null)
        {
            SetSpriteAlpha(0f); // Se oculta correctamente al iniciar
        }
        else
        {
            Debug.LogWarning($"[InteractionIndicator] No se encontró ningún SpriteRenderer en los hijos de {gameObject.name}");
        }
    }

    private void LateUpdate()
    {
        if (!isShowing || hintSpriteRenderer == null) return;

        // Mantener posición y Billboard mirando a la cámara
        //
        //hintSpriteRenderer.transform.position = transform.position + offset;

        if (mainCamera != null)
        {
            hintSpriteRenderer.transform.rotation = mainCamera.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag) && !isShowing)
        {
            TriggerIndicator();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(triggerTag) && !isShowing)
        {
            TriggerIndicator();
        }
    }

    private void TriggerIndicator()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(ShowIndicatorSequence());
    }

    private IEnumerator ShowIndicatorSequence()
    {
        isShowing = true;

        for (int i = 0; i < blinkCount; i++)
        {
            SetSpriteAlpha(1f);
            yield return new WaitForSeconds(blinkInterval);
            SetSpriteAlpha(0f);
            yield return new WaitForSeconds(blinkInterval);
        }

        SetSpriteAlpha(1f);
        yield return new WaitForSeconds(displayDuration);

        float fadeDuration = 0.4f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetSpriteAlpha(Mathf.Lerp(1f, 0f, elapsed / fadeDuration));
            yield return null;
        }

        SetSpriteAlpha(0f);
        isShowing = false;
    }

    private void SetSpriteAlpha(float alpha)
    {
        if (hintSpriteRenderer == null) return;
        Color color = hintSpriteRenderer.color;
        color.a = alpha;
        hintSpriteRenderer.color = color;
    }
}
