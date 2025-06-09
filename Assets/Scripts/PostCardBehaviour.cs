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
    Transform postcardKeyObjectStoreTrans, // ��Transform�s��Ұʮɤ~�|�X�{��Sprite����
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
    VideoClip contentDisplayVideoClip; // �i�ܪ��v��

    [SerializeField]
    string contentDisplaySRTFileName = ""; // �i�ܪ��r���ɮצW��

    [SerializeField]
    bool DEBUG_UnlockPostcard = false; // �ΨӴ��լO�_�i�H������H�����}��

    bool isUnlocked = false;

    List<SpriteRenderer> postcardKeyObjectSRList = new List<SpriteRenderer>(); // �x�s�Ҧ������䪫��SpriteRenderer

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

        audioSource_BG.gameObject.SetActive(false); // �������I��

        foreach (SpriteRenderer sr in postcardKeyObjectSRList) {
            sr.gameObject.SetActive(false); // �������Ҧ������䪫��
        }
    }

    public void OnEnterScanPostcardMode()
    {
#if UNITY_EDITOR
        if (DEBUG_UnlockPostcard) {
            targetImagePrefabBaseController.UnlockPostcard(postcardID); // �p�G�ODebug�Ҧ��A�N��������
        }
#endif
    }

    public void ActivatePostcard() // ����
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
