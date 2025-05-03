using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class PostCardBehaviour : MonoBehaviour
{
    public string postcardID = "";

    bool isUnlocked = false;

    [SerializeField]
    Animator animator;

    [SerializeField]
    GameObject contentDisplayTriggerObj; // 觸發物件（掛載Event Trigger）

    [SerializeField]
    UnityEvent OnActivateEvent;

    [SerializeField]
    VideoClip contentDisplayVideoClip; // 展示的影片

    MyTargetImagePrefabBaseController targetImagePrefabBaseController;

    void Awake()
    {
        targetImagePrefabBaseController = 
            transform.parent.GetComponent<MyTargetImagePrefabBaseController>();
    }

    void Start()
    {
        contentDisplayTriggerObj.SetActive(false); // 先關閉觸發物件
    }

    public void ActivatePostcard()
    {
        if ( isUnlocked ) return; // 如果已經解鎖過了，就不再執行

        isUnlocked = true;

        contentDisplayTriggerObj.SetActive(true); // 啟動可點選觸發的功能
        animator.Play("PostcardAni_Open");

        Debug.Log("Postcard " + postcardID + " activated.");

        OnActivateEvent?.Invoke();
    }

    public void TriggerLoadedContent()
    {
        Debug.Log("Triggered display object activation.");

        targetImagePrefabBaseController.StarrPlayingViedo(contentDisplayVideoClip);
    }
}
