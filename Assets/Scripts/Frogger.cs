using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Frogger : MonoBehaviour
{
    public Sprite idleSprite;
    public Sprite leapSprite;
    public Sprite deadSprite;

    private SpriteRenderer spriteRenderer;
    private Vector3 spawnPosition;
    private float farthestRow;
    private bool isCooldownActive;

    private static readonly float LeapDuration = 0.125f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPosition = transform.position;
    }

    private void Update()
    {
        if (!isCooldownActive) HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) Move(Vector3.up, 0f);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector3.left, 90f);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector3.right, -90f);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) Move(Vector3.down, 180f);
    }

    private void Move(Vector3 direction, float rotationZ)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        Vector3 destination = transform.position + direction;

        if (IsMovementBlocked(destination)) return;

        if (destination.y > farthestRow)
            farthestRow = destination.y;

        StopAllCoroutines();
        StartCoroutine(Leap(destination));
    }

    private bool IsMovementBlocked(Vector3 destination)
    {
        Collider2D platform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
        Collider2D obstacle = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Obstacle"));
        Collider2D barrier = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Barrier"));

        if (barrier != null) return true;

        if (platform != null) transform.SetParent(platform.transform);
        else transform.SetParent(null);

        if (obstacle != null && platform == null)
        {
            Death();
            return true;
        }

        return false;
    }

    private IEnumerator Leap(Vector3 destination)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        spriteRenderer.sprite = leapSprite;
        isCooldownActive = true;

        while (elapsed < LeapDuration)
        {
            transform.position = Vector3.Lerp(startPosition, destination, elapsed / LeapDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;
        spriteRenderer.sprite = idleSprite;
        isCooldownActive = false;
    }

    public void Respawn()
    {
        StopAllCoroutines();

        transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        farthestRow = spawnPosition.y;

        spriteRenderer.sprite = idleSprite;
        gameObject.SetActive(true);
        enabled = true;
        isCooldownActive = false;
    }

    public void Death()
    {
        StopAllCoroutines();

        enabled = false;
        transform.rotation = Quaternion.identity;
        spriteRenderer.sprite = deadSprite;

        GameManager.Instance.Died();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enabled && other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && transform.parent == null)
        {
            Death();
        }
    }
}
