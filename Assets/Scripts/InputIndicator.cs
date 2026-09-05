using UnityEngine;
using System.Collections;

public class InputIndicator : MonoBehaviour
{
    [Header("Configuración del Sprite")]
    public SpriteRenderer hintSpriteRenderer;
    [Tooltip("Distancia relativa a la que flotará el indicador sobre el prefab")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);

    [Header("Detección")]
    [Tooltip("Tag del objeto que activa la señal (ej: Player)")]
    [SerializeField] private string triggerTag = "Indication";

    [Header("Tiempos de Animación")]
    [SerializeField] private float blinkInterval = 0.12f;  // Frecuencia del parpadeo
    [SerializeField] private int blinkCount = 3;           // Veces que parpadea
    [SerializeField] private float displayDuration = 2.5f; // Tiempo que permanece visible

    private Camera mainCamera;
    private bool isShowing = false;
    private Coroutine activeCoroutine;

    private void Start()
    {
        mainCamera = Camera.main;
        hintSpriteRenderer = GetComponent<SpriteRenderer>();

        if (hintSpriteRenderer != null)
        {
            // Ocultamos el indicador al arrancar
            SetSpriteAlpha(0f);
        }
    }

    private void LateUpdate()
    {
        // Solo ejecutamos el seguimiento y orientación mientras la señal esté visible
        if (!isShowing || hintSpriteRenderer == null) return;

        // 1. Posicionamiento dinámico sobre el prefab
        hintSpriteRenderer.transform.position = transform.position + offset;

        // 2. Efecto Billboard: Mira hacia la cámara en todo momento
        if (mainCamera != null)
        {
            hintSpriteRenderer.transform.rotation = mainCamera.transform.rotation;
        }
    }

    // Detección para Físicas 3D
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag) && !isShowing)
        {
            TriggerIndicator();
        }
    }

    private void TriggerIndicator()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartCoroutine(ShowIndicatorSequence());
    }

    private IEnumerator ShowIndicatorSequence()
    {
        isShowing = true;

        // 1. Parpadeo inicial
        for (int i = 0; i < blinkCount; i++)
        {
            SetSpriteAlpha(1f);
            yield return new WaitForSeconds(blinkInterval);
            SetSpriteAlpha(0f);
            yield return new WaitForSeconds(blinkInterval);
        }

        // 2. Mantener totalmente visible
        SetSpriteAlpha(1f);
        yield return new WaitForSeconds(displayDuration);

        // 3. Desaparición suave (Fade Out)
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
