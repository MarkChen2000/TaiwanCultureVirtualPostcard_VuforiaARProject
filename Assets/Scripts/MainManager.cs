using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    bool isPlayingContentVideo = false;

    void Awake()
    {
        /*foreach (GameObject target in registeredTargetImageList) {

            DefaultObserverEventHandler imageTragetBehaviour = 
                target.GetComponent<DefaultObserverEventHandler>();
            
            // Initialize.
        }*/
    }

    private void Start()
    {
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

        // �i�J
        switch ( nextState) {
            case GameState.Initialization:

                baseCardTargetController.gameObject.SetActive(false);
                baseCardTargetController.ToggleCanUnlockCards(false);

                instructionTextAnimator.Play("InstructionTextFlipVertialToHorizonal");

                UIManager.Instance.SetInstructionText("��V�ϥθ˸m�H��o��n����I" +
                    "\n��������I���U�@�B�~��C");
                break;
            case GameState.StandByForBaseScan:
                
                baseCardTargetController.gameObject.SetActive(true);
                buttonsAdjustBaseSizeObj.SetActive(false);

                instructionTextAnimator.Play("InstructionTextIdle");

                UIManager.Instance.SetInstructionText("�б��y��a�d���C");
                break;
            case GameState.PostCardScaning:

                baseCardTargetController.ToggleCanUnlockCards(true);
                buttonsAdjustBaseSizeObj.SetActive(true);
                UIManager.Instance.SetInstructionText("�i�H�}�l���y�d���Ӹ���C\n�άO�I���U�Ӱϰ��[�ݼv����I");
                break;
            case GameState.PlayingVideo:

                baseCardTargetController.ToggleCanUnlockCards(false);
                UIManager.Instance.SetInstructionText("���b����v���C");
                buttonStopVideoPlayingObj.SetActive(true);
                break;
        }
        
        currentGameState = nextState;
    }

    VideoPlayer currentPlayer = null;
    public void StartPlayingVideo(VideoPlayer player)
    {
        videoDisplayImageObj.SetActive(true); // ��ܼ���v����UI
        ChangeGameState(GameState.PlayingVideo);

        if (isPlayingContentVideo) {

            currentPlayer.Stop();
        }

        currentPlayer = player;

        isPlayingContentVideo = true;
        currentPlayer.loopPointReached += OnContentVideoStop; // ���U���񵲧��ɪ��I�s�C�`�N�o�O�����`���񵲧�

        currentPlayer.Play();
    }

    public void StopVideoPlaying() // ���~�����v��
    {
        if ( currentPlayer == null) {
            Debug.LogWarning("No video player is currently playing.");
            return;
        }

        UIManager.Instance.DisplayMessage("�w�����v���C");

        currentPlayer.Stop();
        VideoPlayerSRTSubtitles_TMP.Instance.ClearSubtitle(); // �M���r��
        OnContentVideoStop(currentPlayer);
    }

    void OnContentVideoStop(VideoPlayer player)
    {
        videoDisplayImageObj.SetActive(false); // ���ü���v����UI
        ChangeGameState(GameState.PostCardScaning);

        isPlayingContentVideo = false;
        player.loopPointReached -= OnContentVideoStop;

        if (player.targetTexture != null) {
            player.targetTexture.Release();
        }

        currentPlayer = null;
        buttonStopVideoPlayingObj.SetActive(false);
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

            UIManager.Instance.DisplayMessage("��a�j�p�w�վ�I\n" +
                "�ثe�j�p�G x" + newScale.x.ToString("F2"));
        }
        else {

            UIManager.Instance.DisplayMessage("��a�j�p����p�󵥩�0�I");
            Debug.LogWarning("Cannot scale the base to a negative or zero size.");
        }
    }

    public void SetContentDisplayCanvasObjTo(Transform parentTranform)
    {
        contentDisplayCanvasObjTrans.SetParent(parentTranform);
        contentDisplayCanvasObjTrans.localPosition = Vector3.zero;
        contentDisplayCanvasObjTrans.localRotation = Quaternion.identity;
    }
}
