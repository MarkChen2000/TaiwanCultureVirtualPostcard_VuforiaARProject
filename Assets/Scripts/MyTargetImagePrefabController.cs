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
    GameObject displayObjectPrefab; // �d������

    [SerializeField]
    GameObject contentDisplayTriggerObj; // �i�ܼv����Ĳ�o����]����Event Trigger�^

    [SerializeField]
    VideoPlayer contentDisplayVideoPlayer; // �i�ܼv�����񾹤���

    [SerializeField]
    VideoClip contentDisplayVideoClip; // �i�ܪ��v��

    GameObject displayObject; // �d���������ƫ᪺����
    Animator displayObjectAnimator; // �d���������ʰʵe�ʵe��

    [Header("Content Display Configuration")]

    [Header("Configuration")]

    [SerializeField]
    float displayObjectSizeMultiplier = 1f;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        if ( displayObject == null ) SpawnDisplayhObject();

        contentDisplayTriggerObj.SetActive(true); // �Ұʥi�I��Ĳ�o�v�����񪺥\��

        displayObjectAnimator.Play("PostcardAni_Open");
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();

        Destroy(displayObject);

        contentDisplayTriggerObj.SetActive(false); // �����i�I��Ĳ�o�v�����񪺥\��
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
