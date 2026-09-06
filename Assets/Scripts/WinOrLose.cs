using UnityEngine;

public class WinOrLose : MonoBehaviour
{
    [SerializeField] private string triggerTag = "Interactuable";
    private DragableAnimation interactuableScript;
    public PlayerSplineFollower splineFollower;
    private bool canSurpass;
    private Animator animator;
    public LevelManager levelManager;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            Debug.Log("ColisionaBien");
            interactuableScript = other.GetComponent<DragableAnimation>();
            canSurpass = interactuableScript.isCompleted;
            CheckIfCorrect();
        }
    }

    private void CheckIfCorrect()
    {
        if (!canSurpass)
        {
            splineFollower.isDead = true;
            animator.SetTrigger("Dead");
            Debug.Log("GameOver");
            Invoke(nameof(RestartGame), 2f);
        }
        else
        {
            interactuableScript.animator.SetTrigger("Interaction");
        }
    }

    private void Update()
    {
        if (!splineFollower.isDead) return;
        transform.Translate(Vector3.right * LevelManager.Instance.GetCurrentLevelVelocity() * -1 * Time.deltaTime);
    }

    private void RestartGame()
    {
        levelManager.ReloadCurrentScene();
    }
}
