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
    GameObject contentDisplayTriggerObj; // Ĳ�o����]����Event Trigger�^

    [SerializeField]
    UnityEvent OnActivateEvent;

    [SerializeField]
    VideoClip contentDisplayVideoClip; // �i�ܪ��v��

    MyTargetImagePrefabBaseController targetImagePrefabBaseController;

    void Awake()
    {
        targetImagePrefabBaseController = 
            transform.parent.GetComponent<MyTargetImagePrefabBaseController>();
    }

    void Start()
    {
        contentDisplayTriggerObj.SetActive(false); // ������Ĳ�o����
    }

    public void ActivatePostcard()
    {
        if ( isUnlocked ) return; // �p�G�w�g����L�F�A�N���A����

        isUnlocked = true;

        contentDisplayTriggerObj.SetActive(true); // �Ұʥi�I��Ĳ�o���\��
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
