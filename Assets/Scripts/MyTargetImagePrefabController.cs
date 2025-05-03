using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Video;
using Vuforia;

public class MyTargetImagePrefabController : DefaultObserverEventHandler
{
    [Header("Costom Controller Reference")]

    /*[SerializeField]
    ContentDisplayManager contentDisplayManager;*/

    [SerializeField]
    GameObject displayObjectPrefab; // 卡片物件

    [SerializeField]
    GameObject contentDisplayTriggerObj; // 展示影片的觸發物件（掛載Event Trigger）

    [SerializeField]
    VideoPlayer contentDisplayVideoPlayer; // 展示影片撥放器元件

    [SerializeField]
    VideoClip contentDisplayVideoClip; // 展示的影片

    GameObject displayObject; // 卡片物件實體化後的物件
    Animator displayObjectAnimator; // 卡片本身移動動畫動畫器

    [Header("Content Display Configuration")]

    [Header("Configuration")]

    [SerializeField]
    float displayObjectSizeMultiplier = 1f;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        if ( displayObject == null ) SpawnDisplayhObject();

        contentDisplayTriggerObj.SetActive(true); // 啟動可點選觸發影片撥放的功能

        displayObjectAnimator.Play("PostcardAni_Open");
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();

        Destroy(displayObject);

        contentDisplayTriggerObj.SetActive(false); // 關閉可點選觸發影片撥放的功能
    }

    void SpawnDisplayhObject()
    {
        if (displayObjectPrefab != null) {
            displayObject = Instantiate(displayObjectPrefab, transform);
            displayObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            displayObject.transform.localRotation = Quaternion.identity;
            displayObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f) * displayObjectSizeMultiplier;

            displayObjectAnimator = displayObject.GetComponent<Animator>();
        }
    }

    public void TriggerLoadedContent()
    {
        Debug.Log("Triggered display object activation.");

        // displayObjectAnimator.Play("PostcardAni_Close");

        contentDisplayVideoPlayer.clip = contentDisplayVideoClip;
        contentDisplayVideoPlayer.Play();

        MainManager.Instance.StartPlayingVideo(contentDisplayVideoPlayer);
    }

}
