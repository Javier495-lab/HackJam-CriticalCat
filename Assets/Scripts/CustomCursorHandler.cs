using UnityEngine;

public class CustomCursorHandler : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Objeto hijo con el SpriteRenderer de tu ratón custom para este prefab")]
    [SerializeField] private SpriteRenderer customCursorRenderer;

    [Header("Ajustes de Profundidad")]
    [Tooltip("Distancia hacia adelante (hacia la cámara) para evitar que se meta dentro del modelo 3D")]
    [SerializeField] private float zOffset = 0.5f;

    [Header("Ajustes para el Modo Círculo")]
    [Tooltip("Radio en píxeles alrededor del objeto para el modo giratorio")]
    [SerializeField] private float circleRadiusPixels = 80f;

    private DragableAnimation interactiveAnim;
    private Camera mainCamera;

    private Vector3 initialClickOffsetFromPrefab;
    private float depthFromCamera;
    private bool isDraggingCursor = false;

    private void Awake()
    {
        interactiveAnim = GetComponent<DragableAnimation>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (customCursorRenderer != null)
        {
            customCursorRenderer.gameObject.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (interactiveAnim == null || customCursorRenderer == null) return;
        if (interactiveAnim.IsCompleted || interactiveAnim.IsAutocompleting) return;

        Cursor.visible = false;
        isDraggingCursor = true;

        depthFromCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        if (depthFromCamera == 0) depthFromCamera = 10f;

        // Calculamos la distancia relativa en pantalla entre el ratón y el prefab que se está moviendo
        Vector3 prefabScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        initialClickOffsetFromPrefab = Input.mousePosition - prefabScreenPos;

        customCursorRenderer.gameObject.SetActive(true);
        UpdateCustomCursorPosition();
    }

    private void OnMouseDrag()
    {
        if (!isDraggingCursor || customCursorRenderer == null) return;

        if (interactiveAnim.IsAutocompleting || interactiveAnim.IsCompleted)
        {
            ResetCursor();
            return;
        }

        UpdateCustomCursorPosition();
    }

    private void OnMouseUp()
    {
        ResetCursor();
    }

    private void OnDisable()
    {
        ResetCursor();
    }

    public void ResetCursor()
    {
        isDraggingCursor = false;
        Cursor.visible = true;

        if (customCursorRenderer != null)
        {
            customCursorRenderer.gameObject.SetActive(false);
        }
    }

    private void UpdateCustomCursorPosition()
    {
        if (mainCamera == null || customCursorRenderer == null) return;

        // 1. Obtenemos la posición actual del prefab en pantalla (se mueve hacia la izquierda en tiempo real)
        Vector3 currentPrefabScreenPos = mainCamera.WorldToScreenPoint(transform.position);

        // 2. Calculamos el origen dinámico del cursor sumando el offset inicial del clic
        Vector3 dynamicStartScreenPos = currentPrefabScreenPos + initialClickOffsetFromPrefab;

        Vector3 currentMouse = Input.mousePosition;
        Vector3 constrainedScreenPos = dynamicStartScreenPos;

        // 3. Calculamos la restricción de movimiento usando el origen en movimiento
        switch (interactiveAnim.CurrentInteractionType)
        {
            case DragableAnimation.InteractionType.Drag:
                constrainedScreenPos = CalculateDragPosition(currentMouse, dynamicStartScreenPos);
                break;

            case DragableAnimation.InteractionType.Circle:
                constrainedScreenPos = CalculateCirclePosition(currentMouse, currentPrefabScreenPos);
                break;

            case DragableAnimation.InteractionType.LeftClick:
            case DragableAnimation.InteractionType.RightClick:
                constrainedScreenPos = dynamicStartScreenPos; // Sigue al prefab exacto en su movimiento
                break;
        }

        // 4. Convertimos las coordenadas restringidas de pantalla a posición en el mundo
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(
            constrainedScreenPos.x,
            constrainedScreenPos.y,
            depthFromCamera
        ));

        // Offset en Z para proyectar hacia la cámara
        worldPos -= mainCamera.transform.forward * zOffset;

        customCursorRenderer.transform.position = worldPos;
        customCursorRenderer.transform.rotation = mainCamera.transform.rotation;
    }

    private Vector3 CalculateDragPosition(Vector3 currentMouse, Vector3 dynamicStartScreenPos)
    {
        Vector3 constrained = dynamicStartScreenPos;
        float maxPixels = interactiveAnim.MaxDragPixels;

        switch (interactiveAnim.CurrentDragDirection)
        {
            case DragableAnimation.DragDirection.Down:
                float deltaYDown = Mathf.Clamp(dynamicStartScreenPos.y - currentMouse.y, 0f, maxPixels);
                constrained.y = dynamicStartScreenPos.y - deltaYDown;
                break;

            case DragableAnimation.DragDirection.Up:
                float deltaYUp = Mathf.Clamp(currentMouse.y - dynamicStartScreenPos.y, 0f, maxPixels);
                constrained.y = dynamicStartScreenPos.y + deltaYUp;
                break;

            case DragableAnimation.DragDirection.Right:
                float deltaXRight = Mathf.Clamp(currentMouse.x - dynamicStartScreenPos.x, 0f, maxPixels);
                constrained.x = dynamicStartScreenPos.x + deltaXRight;
                break;

            case DragableAnimation.DragDirection.Left:
                float deltaXLeft = Mathf.Clamp(dynamicStartScreenPos.x - currentMouse.x, 0f, maxPixels);
                constrained.x = dynamicStartScreenPos.x - deltaXLeft;
                break;
        }

        return constrained;
    }

    private Vector3 CalculateCirclePosition(Vector3 currentMouse, Vector3 currentPrefabScreenPos)
    {
        Vector2 dir = (Vector2)currentMouse - (Vector2)currentPrefabScreenPos;
        float angle = Mathf.Atan2(dir.y, dir.x);

        return currentPrefabScreenPos + new Vector3(
            Mathf.Cos(angle) * circleRadiusPixels,
            Mathf.Sin(angle) * circleRadiusPixels,
            0f
        );
    }
}
