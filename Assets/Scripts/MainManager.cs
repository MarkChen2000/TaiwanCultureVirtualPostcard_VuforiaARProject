using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.Video;
using Vuforia;

public enum GameState
{
    Initialization = 0, // 不可掃描基地狀態
    StandByForBaseScan = 1, // 等待基地被掃描狀態
    PostCardScaning = 2, // 基地已出現，等待明信片被掃描狀態
    PlayingVideo = 3 // 正在撥放影片狀態，不開放明信片掃描
}

public class MainManager : Singleton<MainManager>
{
    [Header("References")]

    [SerializeField]
    VuforiaBehaviour vuforiaBehaviour;

    [SerializeField]
    MyTargetImagePrefabBaseController baseCardTargetController;

    [SerializeField]
    VideoPlayer contentDisplayVideoPlayer; // 展示影片撥放器元件

    [SerializeField]
    Transform PostcardBaseTransform, // 基地整體的Transform
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
        videoDisplayImageObj.SetActive(false); // 隱藏撥放影片的UI

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
        /*// 離開
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

        // 進入
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

        videoDisplayImageObj.SetActive(true); // 顯示撥放影片的UI
        ChangeGameState(GameState.PlayingVideo);

        contentDisplayVideoPlayer.clip = clip;
        if (subtitleFileName != null) VideoPlayerSRTSubtitles_TMP.Instance.SetSRTFileName(subtitleFileName);
        currentPlayingPostCardBehaviour = behaviour;
        currentPlayingPostCardBehaviour.contentDisplayFingerPointInstructionUIObj.SetActive(false);

        contentDisplayVideoPlayer.loopPointReached += OnContentVideoStop; // 註冊當撥放結束時的呼叫。注意這是指正常撥放結束

        contentDisplayVideoPlayer.Play();
    }

    public void StopVideoPlaying(bool isShowInstructionText) // 中途停止撥放影片
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
        VideoPlayerSRTSubtitles_TMP.Instance.ClearSubtitle(); // 清除字幕
        OnContentVideoStop(contentDisplayVideoPlayer);
    }

    void OnContentVideoStop(VideoPlayer player) // 未中斷的情況下，影片結束撥放時所呼叫
    {
        //Debug.Log(":1");
        videoDisplayImageObj.SetActive(false); // 隱藏撥放影片的UI
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
    float baseAdjustmentMultiplier = 0.1f; // 基地大小調整的倍率

    public void AdjustBaseSize(bool bigOrSmall)
    {
        if (PostcardBaseTransform == null) {
            Debug.LogWarning("PostcardBaseTransform is not assigned.");
            return;
        }

        // 計算調整的大小
        float adjustment = bigOrSmall ? baseAdjustmentMultiplier : -baseAdjustmentMultiplier;

        // 確保縮小時不會讓大小變成負值
        Vector3 newScale = PostcardBaseTransform.localScale + new Vector3(1f, 1f, 1f)*adjustment;
        
        // 因為預設縮放是xyz同步的，所以這裡只要確認x。
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
        // 注意！ 此方法未考慮到table尚在加載，或是正在初始化的可能，
        // 所以以後若是改為可以動態更改語言，這樣勢必是不夠的。
        var result = LocalizationSettings.StringDatabase.
            GetTableEntry(tableName, tableEntry);
        return result.Entry.GetLocalizedString();
    }


}
