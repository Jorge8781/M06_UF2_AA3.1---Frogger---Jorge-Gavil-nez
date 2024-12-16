using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites = new Sprite[0];
    [SerializeField] private float animationTime = 0.5f;
    [SerializeField] private bool loop = true;

    private SpriteRenderer spriteRenderer;
    private int animationFrame;

    private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();

    private void Start() => InvokeRepeating(nameof(Advance), 0f, animationTime);

    private void Advance()
    {
        if (!spriteRenderer.enabled || sprites.Length == 0) return;

        animationFrame = loop
            ? (animationFrame + 1) % sprites.Length
            : Mathf.Min(animationFrame + 1, sprites.Length - 1);

        spriteRenderer.sprite = sprites[animationFrame];
    }

    public void Restart()
    {
        animationFrame = 0;
        Advance();
    }
}

