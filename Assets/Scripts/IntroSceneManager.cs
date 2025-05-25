using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

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

    //
    // 控制語言切換按鈕與本地化系統，非常土炮，只有中英兩種，要擴大使用必須修改！
    [SerializeField]
    TMP_Text languageButtonText;
    string languageButtonText_ZH = "中"; // index 預設 0
    string languageButtonText_EN = "EN"; // index 1

    int currentLanguageIndex = 0;
    public void Button_SwitchNextLanguage()
    {
        currentLanguageIndex++;
        if (currentLanguageIndex >= LocalizationSettings.AvailableLocales.Locales.Count) {
            currentLanguageIndex = 0;
        }

        Locale newLocale = LocalizationSettings.AvailableLocales.Locales[currentLanguageIndex];
        switch ( newLocale.LocaleName ) {
            case "Chinese (Traditional) (zh-TW)":
                languageButtonText.text = languageButtonText_ZH;
                break;
            case "English (United States) (en-US)":
                languageButtonText.text = languageButtonText_EN;
                break;
            default:
                break;
        }   

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[currentLanguageIndex];
    }
}
