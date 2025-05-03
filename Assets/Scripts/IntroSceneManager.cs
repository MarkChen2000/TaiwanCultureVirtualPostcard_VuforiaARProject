using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntroSceneManager : MonoBehaviour
{

    [SerializeField]
    CanvasGroup introBlockerCG;

    [SerializeField]
    UnityEvent introBlockerOnFadeOutEvent;
    
    void Start()
    {
        StartCoroutine(FadeCanvasGroup(introBlockerCG, 1f, 0f, 2f, introBlockerOnFadeOutEvent));
    }

    public void BootIntoMainScene()
    {
       // Load the main scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration, UnityEvent onEndEvent)
    {
        float elapsed = 0f;
        while (elapsed < duration) {
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;

        onEndEvent?.Invoke();
    }
}
