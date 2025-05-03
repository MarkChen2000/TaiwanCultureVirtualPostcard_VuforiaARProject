using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    GameObject instrctionTextObj;
    [SerializeField]
    TMP_Text instrctionText;

    [SerializeField]
    Transform messageUIPrefabSpawnTrans;

    [SerializeField]
    GameObject messageDisplayUIPrefab;

    public void ToggleInstructionTextObj(bool isEnable)
    {
        instrctionTextObj.SetActive(isEnable);
    }

    public void SetInstructionText(string text)
    {
        StartCoroutine(ChangeInstructionText(text));
    }

    IEnumerator ChangeInstructionText(string text)
    {
        CanvasGroup instructionTextCG = instrctionText.GetComponent<CanvasGroup>();

        yield return StartCoroutine(FadeCanvasGroup(instructionTextCG, 1f, 0f, 0.5f)); // 淡出
        instrctionText.text = text;

        yield return StartCoroutine(FadeCanvasGroup(instructionTextCG, 0f, 1f, 0.5f)); // 淡入
    }

    public void DisplayMessage(string message)
    {
        StartCoroutine(DisplayMessageFadeInOut(message, 0.1f, 2f, 1f));
    }

    IEnumerator DisplayMessageFadeInOut(string message, float fadeInTime, float duration, float fadeOutTime)
    {
        GameObject msgPrefab = Instantiate(messageDisplayUIPrefab, messageUIPrefabSpawnTrans);
        CanvasGroup msgCG = msgPrefab.GetComponent<CanvasGroup>();
        TMP_Text msgText = msgPrefab.GetComponentInChildren<TMP_Text>();
        msgText.text = message;

        // 等待 FadeCanvasGroup 完成淡入
        yield return StartCoroutine(FadeCanvasGroup(msgCG, 0f, 1f, fadeInTime));
        yield return new WaitForSeconds(duration);
        // 等待 FadeCanvasGroup 完成淡出
        yield return StartCoroutine(FadeCanvasGroup(msgCG, 1f, 0f, fadeOutTime));

        Destroy(msgPrefab);
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration) {
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }
}
