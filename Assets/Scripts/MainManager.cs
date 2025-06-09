using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.Video;
using Vuforia;

public enum GameState
{
    Initialization = 0, // ���i���y��a���A
    StandByForBaseScan = 1, // ���ݰ�a�Q���y���A
    PostCardScaning = 2, // ��a�w�X�{�A���ݩ��H���Q���y���A
    PlayingVideo = 3 // ���b����v�����A�A���}����H�����y
}

public class MainManager : Singleton<MainManager>
{
    [Header("References")]

    [SerializeField]
    VuforiaBehaviour vuforiaBehaviour;

    [SerializeField]
    MyTargetImagePrefabBaseController baseCardTargetController;

    [SerializeField]
    VideoPlayer contentDisplayVideoPlayer; // �i�ܼv�����񾹤���

    [SerializeField]
    Transform PostcardBaseTransform, // ��a���骺Transform
        contentDisplayCanvasObjTrans; 

    [SerializeField]
    GameObject buttonsAdjustBaseSizeObj, buttonStopVideoPlayingObj, videoDisplayImageObj;

    [SerializeField]
    Animator instructionTextAnimator;

    [Header("Configuration")]

    [SerializeField]
    GameState currentGameState = GameState.Initialization;

    [SerializeField]
    public bool IsDisplayVideoBehindEveryPostcard = true;

    void Awake()
    {
        /*foreach (GameObject target in registeredTargetImageList) {

            DefaultObserverEventHandler imageTragetBehaviour = 
                target.GetComponent<DefaultObserverEventHandler>();
            
            // Initialize.
        }*/

        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    private void Start()
    {
        videoDisplayImageObj.SetActive(false); // ���ü���v����UI

        ChangeGameState(GameState.Initialization);
    }

    [SerializeField]
    RectTransform _postCardBaseTrans;

    public void SetPostCardBaseTransToNull()
    {
        _postCardBaseTrans.SetParent(null);
    }

    public void EnterScanBaseState()
    {
        ChangeGameState(GameState.StandByForBaseScan);
    }

    public void ChangeGameState(GameState nextState)
    {
        /*// ���}
        switch ( currentGameState) {
            case GameState.Initialization:
                break;
            case GameState.StandByForBaseScan:
                break;
            case GameState.PostCardScaning:
                break;
            case GameState.PlayingVideo:
                break;
        }*/

        string resultText = string.Empty;

        // �i�J
        switch (nextState) {
            case GameState.Initialization: 
                baseCardTargetController.gameObject.SetActive(false);
                baseCardTargetController.ToggleCanUnlockCards(false);

                instructionTextAnimator.Play("InstructionTextFlipVertialToHorizonal");

                resultText =
                    GetLocalizationResult("UITable", "InstructionText_StateInitialization");

                UIManager.Instance.SetInstructionText(resultText);
                break;
            case GameState.StandByForBaseScan: 
                baseCardTargetController.gameObject.SetActive(true);
                buttonsAdjustBaseSizeObj.SetActive(false);

                instructionTextAnimator.Play("InstructionTextIdle");

                resultText =
                    GetLocalizationResult("UITable", "InstructionText_StandByForBaseScan");

                UIManager.Instance.SetInstructionText(resultText);
                break;
            case GameState.PostCardScaning:

                baseCardTargetController.ToggleCanUnlockCards(true);
                buttonsAdjustBaseSizeObj.SetActive(true);

                AudioManager.Instance.SetPostcardBGAudioVolume(- 20f);

                resultText =
                    GetLocalizationResult("UITable", "InstructionText_PostcardScaning");

                UIManager.Instance.SetInstructionText(resultText);
                break;
            case GameState.PlayingVideo:

                baseCardTargetController.ToggleCanUnlockCards(false);
                buttonStopVideoPlayingObj.SetActive(true);

                AudioManager.Instance.SetPostcardBGAudioVolume(-80f);

                resultText =
                    GetLocalizationResult("UITable", "InstructionText_PlayingVideo");

                UIManager.Instance.SetInstructionText(resultText);
                break;
        }
        
        currentGameState = nextState;
    }

    PostCardBehaviour currentPlayingPostCardBehaviour = null;
    public void StartPlayingVideoFromBehaviour(VideoClip clip, string subtitleFileName, PostCardBehaviour behaviour)
    {
        //Debug.Log(":-1");
        if (contentDisplayVideoPlayer.isPlaying) {

            StopVideoPlaying(false);
        }

        videoDisplayImageObj.SetActive(true); // ��ܼ���v����UI
        ChangeGameState(GameState.PlayingVideo);

        contentDisplayVideoPlayer.clip = clip;
        if (subtitleFileName != null) VideoPlayerSRTSubtitles_TMP.Instance.SetSRTFileName(subtitleFileName);
        currentPlayingPostCardBehaviour = behaviour;
        currentPlayingPostCardBehaviour.contentDisplayFingerPointInstructionUIObj.SetActive(false);

        contentDisplayVideoPlayer.loopPointReached += OnContentVideoStop; // ���U���񵲧��ɪ��I�s�C�`�N�o�O�����`���񵲧�

        contentDisplayVideoPlayer.Play();
    }

    public void StopVideoPlaying(bool isShowInstructionText) // ���~�����v��
    {
        //Debug.Log(":0");
        if (contentDisplayVideoPlayer == null) {
            Debug.LogWarning("No video player is currently playing.");
            return;
        }

        if (isShowInstructionText) {
            string resultText = GetLocalizationResult
                ("UITable", "InstructionText_VideoStopped");
            UIManager.Instance.DisplayMessage(resultText);
        }

        contentDisplayVideoPlayer.Stop();
        VideoPlayerSRTSubtitles_TMP.Instance.ClearSubtitle(); // �M���r��
        OnContentVideoStop(contentDisplayVideoPlayer);
    }

    void OnContentVideoStop(VideoPlayer player) // �����_�����p�U�A�v����������ɩҩI�s
    {
        //Debug.Log(":1");
        videoDisplayImageObj.SetActive(false); // ���ü���v����UI
        ChangeGameState(GameState.PostCardScaning);

        player.loopPointReached -= OnContentVideoStop;

        if (player.targetTexture != null) {
            player.targetTexture.Release();
        }

        buttonStopVideoPlayingObj.SetActive(false);

        currentPlayingPostCardBehaviour.contentDisplayFingerPointInstructionUIObj.SetActive(true);
        currentPlayingPostCardBehaviour = null;
    }

    [SerializeField]
    float baseAdjustmentMultiplier = 0.1f; // ��a�j�p�վ㪺���v

    public void AdjustBaseSize(bool bigOrSmall)
    {
        if (PostcardBaseTransform == null) {
            Debug.LogWarning("PostcardBaseTransform is not assigned.");
            return;
        }

        // �p��վ㪺�j�p
        float adjustment = bigOrSmall ? baseAdjustmentMultiplier : -baseAdjustmentMultiplier;

        // �T�O�Y�p�ɤ��|���j�p�ܦ��t��
        Vector3 newScale = PostcardBaseTransform.localScale + new Vector3(1f, 1f, 1f)*adjustment;
        
        // �]���w�]�Y��Oxyz�P�B���A�ҥH�o�̥u�n�T�{x�C
        if (newScale.x > 0 ) {
            PostcardBaseTransform.localScale = newScale;

            string resultText =
                    GetLocalizationResult("UITable", "InstructionText_SizeAdjusted");
            UIManager.Instance.DisplayMessage(resultText + newScale.x.ToString("F2"));
        }
        else {

            string resultText =
                    GetLocalizationResult("UITable", "InstructionText_SizeAdjustedFailed");
            UIManager.Instance.DisplayMessage(resultText);

            Debug.LogWarning("Cannot scale the base to a negative or zero size.");
        }
    }

    public void SetContentDisplayCanvasObjTo(Transform parentTranform)
    {
        contentDisplayCanvasObjTrans.SetParent(parentTranform);
        contentDisplayCanvasObjTrans.localPosition = Vector3.zero;
        contentDisplayCanvasObjTrans.localRotation = Quaternion.identity;
    }

    string GetLocalizationResult(string tableName, string tableEntry)
    {
        // �`�N�I ����k���Ҽ{��table�|�b�[���A�άO���b��l�ƪ��i��A
        // �ҥH�H��Y�O�אּ�i�H�ʺA���y���A�o�˶ե��O�������C
        var result = LocalizationSettings.StringDatabase.
            GetTableEntry(tableName, tableEntry);
        return result.Entry.GetLocalizedString();
    }


}
