using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    [SerializeField]
    private float _MovementSpeed = 1.5f;

    private SpriteRenderer _SpriteRenderer;

    private void Awake()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        var currentColor = _SpriteRenderer.material.color;
        do
        {
            currentColor.a -= 0.1f;
            _SpriteRenderer.material.color = currentColor;
            transform.position = new Vector2(transform.position.x, transform.position.y + Vector2.up.y * _MovementSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.1f);
        } while (currentColor.a >= 0);

        Destroy(gameObject, 0.1f);
        yield return null;
    }
}
