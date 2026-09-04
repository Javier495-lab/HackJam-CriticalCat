using TMPro;
using UnityEngine;

public class DragableAnimation : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private string parameterName = "Progress";
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Configuración de Arrastre")]
    [SerializeField] private float maxDragPixels = 300f;

    [Tooltip("Velocidad de suavizado. Valores más bajos dan más sensación de peso/pesadez.")]
    [SerializeField] private float smoothSpeed = 5f;

    [Tooltip("Umbral (de 0.0 a 1.0) para activar el autocompletado")]
    [Range(0f, 1f)]
    [SerializeField] private float autoSnapThreshold = 0.8f;

    private float startMouseY;
    private float startProgress;

    private float targetProgress = 0f;   // A dónde quiere ir el ratón / autocompletar
    private float currentProgress = 0f;  // El valor suavizado que se aplica al Animator

    private bool isAutocompleting = false;
    private bool isCompleted = false;

    private Collider objectCollider;

    private void Awake()
    {
        objectCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        UpdateTextUI();
    }

    private void OnMouseDown()
    {
        // Si ya completó o está autocompletando, ignoramos clics
        if (isCompleted || isAutocompleting) return;

        startMouseY = Input.mousePosition.y;
        startProgress = targetProgress;
    }

    private void OnMouseDrag()
    {
        if (isCompleted || isAutocompleting) return;

        float deltaY = startMouseY - Input.mousePosition.y;
        float progressDelta = deltaY / maxDragPixels;

        // Fijamos la META según el movimiento del ratón
        targetProgress = Mathf.Clamp01(startProgress + progressDelta);

        // Si la meta llega o supera el 80% (0.8), activamos el autocompletado
        if (targetProgress >= autoSnapThreshold)
        {
            StartAutoComplete();
        }
    }

    private void Update()
    {
        if (isCompleted) return;

        // Movemos el progreso actual suavemente hacia la meta
        // Usamos MoveTowards para una velocidad constante, o Lerp para un frenado elástico al final
        currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, smoothSpeed * Time.deltaTime);

        // Si prefieres un efecto más elástico/inercia, puedes usar esta línea en su lugar:
        // currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * smoothSpeed);

        // Actualizamos Animator y UI
        animator.SetFloat(parameterName, currentProgress);
        UpdateTextUI();

        // Si estaba autocompletando y el progreso suavizado llegó al 100%
        if (isAutocompleting && Mathf.Approximately(currentProgress, 1.0f))
        {
            CompleteAnimation();
        }
    }

    private void StartAutoComplete()
    {
        isAutocompleting = true;
        targetProgress = 1.0f; // Forzamos la meta al máximo (100%)

        // Desactivamos el Collider al instante para bloquear más arrastres del usuario
        if (objectCollider != null)
        {
            objectCollider.enabled = false;
        }
    }

    private void CompleteAnimation()
    {
        isCompleted = true;
        currentProgress = 1.0f;
        targetProgress = 1.0f;

        animator.SetFloat(parameterName, 1.0f);
        UpdateTextUI();

        Debug.Log("¡Toldo completamente desplegado!");
    }

    private void UpdateTextUI()
    {
        if (progressText != null)
        {
            progressText.text = $"Apertura: {(currentProgress * 100f):F0}%";
        }
    }
}
