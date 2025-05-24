using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterWiggleAnimator : MonoBehaviour
{
    public float minDelay = 1f;
    public float maxDelay = 3f;
    public float jumpHeight = 0.5f;
    public float jumpDuration = 0.5f;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    Coroutine routine;
    
    void OnEnable()
    {
        if (originalPosition == Vector3.zero)
            originalPosition = transform.localPosition;

        routine = StartCoroutine(RandomMovementRoutine());
    }

    void OnDisable()
    {
        if (routine != null) {
            StopCoroutine(routine);
            routine = null;
        }
    }

    IEnumerator RandomMovementRoutine()
    {
        while (true) {
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            // 隨機選一種動作：跳、左右搖擺、點頭等等
            int action = Random.Range(0, 3);
            switch (action) {
                case 0:
                    yield return StartCoroutine(Jump());
                    break;
                case 1:
                    yield return StartCoroutine(Wiggle());
                    break;
                case 2:
                    yield return StartCoroutine(Nod());
                    break;
            }
        }
    }

    IEnumerator Jump()
    {
        float elapsed = 0f;
        while (elapsed < jumpDuration) {
            float yOffset = Mathf.Sin((elapsed / jumpDuration) * Mathf.PI) * jumpHeight;
            transform.localPosition = originalPosition + new Vector3(0, yOffset, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }

    IEnumerator Wiggle()
    {
        float elapsed = 0f;
        float duration = 0.4f;
        while (elapsed < duration) {
            float xOffset = Mathf.Sin(elapsed * 20f) * 0.1f;
            transform.localRotation = Quaternion.Euler(0, 0, xOffset * 20);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = Quaternion.identity;
    }

    IEnumerator Nod()
    {
        float elapsed = 0f;
        float duration = 0.4f;
        while (elapsed < duration) {
            float zOffset = Mathf.Sin(elapsed * 20f) * 0.05f;
            transform.localRotation = Quaternion.Euler(zOffset * 40, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = Quaternion.identity;
    }
    }
