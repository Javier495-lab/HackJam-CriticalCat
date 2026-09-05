using TMPro;
using UnityEngine;
using System.Collections;

public class DragableAnimation : MonoBehaviour
{
    public enum InteractionType
    {
        Drag,
        LeftClick,
        RightClick,
        Circle
    }

    public enum DragDirection
    {
        Down,
        Up,
        Right,
        Left
    }

    public enum CircleDirection
    {
        Clockwise,
        CounterClockwise
    }

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private string parameterName = "Progress";
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Tipo de Interacción")]
    [SerializeField] private InteractionType interactionType = InteractionType.Drag;
    public InteractionType Type => interactionType;
    public DragDirection Direction => dragDirection;

    [Header("Configuración de Arrastre (Modo Drag)")]
    [SerializeField] private DragDirection dragDirection = DragDirection.Down;
    [SerializeField] private float maxDragPixels = 300f;

    [Header("Configuración de Círculo (Modo Circle)")]
    [SerializeField] private CircleDirection circleDirection = CircleDirection.Clockwise;
    [Tooltip("Grados totales de giro para llegar al 100% (360 = 1 vuelta completa)")]
    [SerializeField] private float totalDegreesForCompletion = 360f;

    [Header("Autocompletado Decelerado")]
    [Range(0f, 0.99f)]
    [SerializeField] private float autoSnapThreshold = 0.8f;

    [Tooltip("Velocidad inicial en clics y velocidad antes del umbral")]
    [SerializeField] private float startAutoCompleteSpeed = 1.5f;

    [Tooltip("Velocidad final al alcanzar el 100% de la animación")]
    [SerializeField] private float finalAutoCompleteSpeed = 0.05f;

    private Vector2 startMousePosition;
    private float startProgress;
    private float currentProgress = 0f;

    private float lastCircleAngle;
    private bool isAutocompleting = false;
    public bool isCompleted = false;
    private Renderer targetRenderer;

    private Collider objectCollider;

    private void Awake()
    {
        objectCollider = GetComponent<Collider>();
        targetRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        UpdateAnimationAndUI();
    }

    private void OnMouseDown()
    {
        if (isCompleted || isAutocompleting) return;

        // Clic Izquierdo Directo: arranca la animación completa
        if (interactionType == InteractionType.LeftClick)
        {
            StartCoroutine(AutoCompleteCoroutine());
            return;
        }

        startMousePosition = Input.mousePosition;
        startProgress = currentProgress;

        if (interactionType == InteractionType.Circle)
        {
            InitCircleAngle();
        }
    }

    private void OnMouseOver()
    {
        if (isCompleted || isAutocompleting) return;

        // Clic Derecho Directo (Botón 1)
        if (interactionType == InteractionType.RightClick && Input.GetMouseButtonDown(1))
        {
            StartCoroutine(AutoCompleteCoroutine());
        }
    }

    private void OnMouseDrag()
    {
        if (isCompleted || isAutocompleting) return;

        switch (interactionType)
        {
            case InteractionType.Drag:
                HandleDragInteraction();
                break;

            case InteractionType.Circle:
                HandleCircleInteraction();
                break;
        }
    }

    private void HandleDragInteraction()
    {
        Vector2 currentMousePosition = Input.mousePosition;
        float deltaPixels = 0f;

        switch (dragDirection)
        {
            case DragDirection.Down:
                deltaPixels = startMousePosition.y - currentMousePosition.y;
                break;
            case DragDirection.Up:
                deltaPixels = currentMousePosition.y - startMousePosition.y;
                break;
            case DragDirection.Right:
                deltaPixels = currentMousePosition.x - startMousePosition.x;
                break;
            case DragDirection.Left:
                deltaPixels = startMousePosition.x - currentMousePosition.x;
                break;
        }

        float progressDelta = deltaPixels / maxDragPixels;
        currentProgress = Mathf.Clamp01(startProgress + progressDelta);

        UpdateAnimationAndUI();
        CheckThreshold();
    }

    private void InitCircleAngle()
    {
        Vector3 screenPos = Camera.main != null
            ? Camera.main.WorldToScreenPoint(transform.position)
            : new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        Vector2 dir = (Vector2)Input.mousePosition - (Vector2)screenPos;
        lastCircleAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private void HandleCircleInteraction()
    {
        Vector3 screenPos = Camera.main != null
            ? Camera.main.WorldToScreenPoint(transform.position)
            : new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        Vector2 dir = (Vector2)Input.mousePosition - (Vector2)screenPos;
        float currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float angleDelta = Mathf.DeltaAngle(lastCircleAngle, currentAngle);

        if (circleDirection == CircleDirection.CounterClockwise)
        {
            angleDelta = -angleDelta;
        }

        lastCircleAngle = currentAngle;

        float progressDelta = angleDelta / totalDegreesForCompletion;
        currentProgress = Mathf.Clamp01(currentProgress + progressDelta);

        UpdateAnimationAndUI();
        CheckThreshold();
    }

    private void CheckThreshold()
    {
        if (currentProgress >= autoSnapThreshold)
        {
            StartCoroutine(AutoCompleteCoroutine());
        }
    }

    private IEnumerator AutoCompleteCoroutine()
    {
        isAutocompleting = true;

        // Desactivamos el Collider al pulsar para bloquear clics adicionales
        if (objectCollider != null)
        {
            objectCollider.enabled = false;
        }

        while (currentProgress < 1.0f)
        {
            float currentSpeed;

            // Fase 1: Antes del 80%, avanza a velocidad constante
            if (currentProgress < autoSnapThreshold)
            {
                currentSpeed = startAutoCompleteSpeed;
            }
            // Fase 2: Del 80% al 100%, frena progresivamente hasta finalAutoCompleteSpeed
            else
            {
                float t = Mathf.InverseLerp(autoSnapThreshold, 1.0f, currentProgress);
                currentSpeed = Mathf.Lerp(startAutoCompleteSpeed, finalAutoCompleteSpeed, t);
            }

            currentProgress += currentSpeed * Time.deltaTime;
            currentProgress = Mathf.Clamp01(currentProgress);

            UpdateAnimationAndUI();

            yield return null;
        }

        CompleteAnimation();
    }

    private void CompleteAnimation()
    {
        isCompleted = true;
        isAutocompleting = false;
        currentProgress = 1.0f;
        targetRenderer.material.color = Color.white;

        UpdateAnimationAndUI();

        Debug.Log($"¡Objeto {gameObject.name} completado!");
    }

    private void UpdateAnimationAndUI()
    {
        if (animator != null)
        {
            animator.SetFloat(parameterName, currentProgress);
        }

        if (progressText != null)
        {
            progressText.text = $"Apertura: {(currentProgress * 100f):F0}%";
        }
    }

    public InteractionType CurrentInteractionType => interactionType;
    public DragDirection CurrentDragDirection => dragDirection;
    public float MaxDragPixels => maxDragPixels;
    public bool IsAutocompleting => isAutocompleting;
    public bool IsCompleted => isCompleted;
}
