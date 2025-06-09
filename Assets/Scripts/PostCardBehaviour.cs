using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class PostCardBehaviour : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    MyTargetImagePrefabBaseController targetImagePrefabBaseController;

    [SerializeField]
    Transform postcardKeyObjectStoreTrans, // 此Transform存放啟動時才會出現的Sprite物件集
        contentDisplayCanvasPositionTrans; 

    [SerializeField]
    Animator animator;

    [SerializeField]
    GameObject contentDisplayTriggerObj, 
        postcardPlaceHintObj;

    [SerializeField]
    public GameObject contentDisplayFingerPointInstructionUIObj;

    [SerializeField]
    AudioSource audioSource_BG;

    [SerializeField]
    UnityEvent OnActivateEvent, OnTriggerEvent;

    public string postcardID = "";

    [SerializeField]
    VideoClip contentDisplayVideoClip; // 展示的影片

    [SerializeField]
    string contentDisplaySRTFileName = ""; // 展示的字幕檔案名稱

    [SerializeField]
    bool DEBUG_UnlockPostcard = false; // 用來測試是否可以解鎖明信片的開關

    bool isUnlocked = false;

    List<SpriteRenderer> postcardKeyObjectSRList = new List<SpriteRenderer>(); // 儲存所有的關鍵物件SpriteRenderer

    void Awake()
    {

        postcardKeyObjectSRList =
            postcardKeyObjectStoreTrans.GetComponentsInChildren<SpriteRenderer>().
            ToList<SpriteRenderer>(); // 取得所有子物件的SpriteRenderer
    }

    void Start()
    {
        contentDisplayTriggerObj.SetActive(false); // 先關閉觸發物件
        contentDisplayFingerPointInstructionUIObj.SetActive(false); // 先關閉指示物件

        audioSource_BG.gameObject.SetActive(false); // 先關閉背景

        foreach (SpriteRenderer sr in postcardKeyObjectSRList) {
            sr.gameObject.SetActive(false); // 先關閉所有的關鍵物件
        }
    }

    public void OnEnterScanPostcardMode()
    {
#if UNITY_EDITOR
        if (DEBUG_UnlockPostcard) {
            targetImagePrefabBaseController.UnlockPostcard(postcardID); // 如果是Debug模式，就直接解鎖
        }
#endif
    }

    public void ActivatePostcard() // 解鎖
    {
        if (isUnlocked) return; 

        isUnlocked = true;

        contentDisplayTriggerObj.SetActive(true); 
        animator.Play("PostcardAni_Open");

        contentDisplayFingerPointInstructionUIObj.SetActive(true);
        postcardPlaceHintObj.SetActive(false);

        audioSource_BG.gameObject.SetActive(true);
        audioSource_BG.Play();

        Debug.Log("Postcard " + postcardID + " activated.");

        OnActivateEvent?.Invoke();

        RamdomlyFadeInKeyObjectOneByOne();
    }

    public void TriggerLoadedContent()
    {
        Debug.Log("Triggered display object activation.");
        OnTriggerEvent?.Invoke();

        if ( MainManager.Instance.IsDisplayVideoBehindEveryPostcard ) {
            MainManager.Instance.SetContentDisplayCanvasObjTo(contentDisplayCanvasPositionTrans);
        }

        MainManager.Instance.StartPlayingVideoFromBehaviour
            (contentDisplayVideoClip, contentDisplaySRTFileName, this);
    }

    void RamdomlyFadeInKeyObjectOneByOne()
    {
        StartCoroutine(FadeInKeyObjectsCoroutine(0.5f,0f));
    }

    IEnumerator FadeInKeyObjectsCoroutine(float fadeDuration, float WaitTime)
    {
        // 確保有物件可以處理
        if (postcardKeyObjectSRList == null || postcardKeyObjectSRList.Count == 0) {
            Debug.LogWarning("No key objects to fade in.");
            yield break;
        }

        // 隨機打亂列表
        List<SpriteRenderer> shuffledList = postcardKeyObjectSRList.OrderBy(x => Random.value).ToList();

        foreach (SpriteRenderer sr in shuffledList) {
            sr.gameObject.SetActive(true); // 啟用物件

            // 開始淡入效果
            float elapsedTime = 0f;
            Color originalColor = sr.color;
            originalColor.a = 0f; // 初始透明度為 0
            sr.color = originalColor;

            while (elapsedTime < fadeDuration) {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null; // 等待下一幀
            }

            // 確保最終透明度為 1
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

            // 每個物件淡入完成後，等待一段時間再處理下一個
            yield return new WaitForSeconds(WaitTime); // 間隔時間
        }
    }
}
