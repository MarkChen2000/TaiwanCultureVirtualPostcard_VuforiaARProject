using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class PostCardBehaviour : MonoBehaviour
{
    public string postcardID = "";

    bool isUnlocked = false;

    [SerializeField]
    MyTargetImagePrefabBaseController targetImagePrefabBaseController;

    [SerializeField]
    Transform postcardKeyObjectStoreTrans; // ��Transform�s��Ұʮɤ~�|�X�{��Sprite����

    [SerializeField]
    Animator animator;

    [SerializeField]
    GameObject contentDisplayTriggerObj, contentDisplayFingerPointInstructionUIObj;

    [SerializeField]
    UnityEvent OnActivateEvent, OnTriggerEvent;

    [SerializeField]
    VideoClip contentDisplayVideoClip; // �i�ܪ��v��

    [SerializeField]
    string contentDisplaySRTFileName = ""; // �i�ܪ��r���ɮצW��

    List<SpriteRenderer> postcardKeyObjectSRList = new List<SpriteRenderer>(); // �x�s�Ҧ������䪫��SpriteRenderer

    [SerializeField]
    bool DEBUG_UnlockPostcard = false; // �ΨӴ��լO�_�i�H������H�����}��

    void Awake()
    {

        postcardKeyObjectSRList =
            postcardKeyObjectStoreTrans.GetComponentsInChildren<SpriteRenderer>().
            ToList<SpriteRenderer>(); // ���o�Ҧ��l����SpriteRenderer
    }

    void Start()
    {
        contentDisplayTriggerObj.SetActive(false); // ������Ĳ�o����
        contentDisplayFingerPointInstructionUIObj.SetActive(false); // ���������ܪ���

        foreach (SpriteRenderer sr in postcardKeyObjectSRList) {
            sr.gameObject.SetActive(false); // �������Ҧ������䪫��
        }
    }

    public void OnEnterScanPostcardMode()
    {
        if (DEBUG_UnlockPostcard) {
            targetImagePrefabBaseController.UnlockPostcard(postcardID); // �p�G�ODebug�Ҧ��A�N��������
        }
    }

    public void ActivatePostcard() // ����
    {
        if (isUnlocked) return; // �p�G�w�g����L�F�A�N���A����

        isUnlocked = true;

        contentDisplayTriggerObj.SetActive(true); // �Ұʥi�I��Ĳ�o���\��
        animator.Play("PostcardAni_Open");

        contentDisplayFingerPointInstructionUIObj.SetActive(true);

        Debug.Log("Postcard " + postcardID + " activated.");

        OnActivateEvent?.Invoke();

        RamdomlyFadeInKeyObjectOneByOne();
    }

    public void TriggerLoadedContent()
    {
        Debug.Log("Triggered display object activation.");
        OnTriggerEvent?.Invoke();

        targetImagePrefabBaseController.StartPlayingVideo(contentDisplayVideoClip, contentDisplaySRTFileName);
    }

    void RamdomlyFadeInKeyObjectOneByOne()
    {
        StartCoroutine(FadeInKeyObjectsCoroutine(1f,0.1f));
    }

    IEnumerator FadeInKeyObjectsCoroutine(float fadeDuration, float WaitTime)
    {
        // �T�O������i�H�B�z
        if (postcardKeyObjectSRList == null || postcardKeyObjectSRList.Count == 0) {
            Debug.LogWarning("No key objects to fade in.");
            yield break;
        }

        // �H�����æC��
        List<SpriteRenderer> shuffledList = postcardKeyObjectSRList.OrderBy(x => Random.value).ToList();

        foreach (SpriteRenderer sr in shuffledList) {
            sr.gameObject.SetActive(true); // �ҥΪ���

            // �}�l�H�J�ĪG
            float elapsedTime = 0f;
            Color originalColor = sr.color;
            originalColor.a = 0f; // ��l�z���׬� 0
            sr.color = originalColor;

            while (elapsedTime < fadeDuration) {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null; // ���ݤU�@�V
            }

            // �T�O�̲׳z���׬� 1
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

            // �C�Ӫ���H�J������A���ݤ@�q�ɶ��A�B�z�U�@��
            yield return new WaitForSeconds(WaitTime); // ���j�ɶ�
        }
    }
}
