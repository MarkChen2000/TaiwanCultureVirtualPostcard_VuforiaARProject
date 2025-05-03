using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField]
    VuforiaBehaviour vuforiaBehaviour;

    [SerializeField]
    MyTargetImagePrefabBaseController baseCardTargetController; 

    /*[SerializeField]
    List<GameObject> registeredTargetImageList = new List<GameObject>();*/

    [SerializeField]
    GameState currentGameState = GameState.Initialization;

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

        // 進入
        switch ( nextState) {
            case GameState.Initialization:

                baseCardTargetController.gameObject.SetActive(false);
                baseCardTargetController.ToggleCanUnlockCards(false);

                UIManager.Instance.SetInstructionText("請點擊下一步開始掃描。");
                break;
            case GameState.StandByForBaseScan:
                
                baseCardTargetController.gameObject.SetActive(true);

                UIManager.Instance.SetInstructionText("請開始掃描基地卡片。");
                break;
            case GameState.PostCardScaning:

                baseCardTargetController.ToggleCanUnlockCards(true);
                UIManager.Instance.SetInstructionText("可以開始掃描解鎖卡片。");
                break;
            case GameState.PlayingVideo:

                baseCardTargetController.ToggleCanUnlockCards(false);
                break;
        }
        
        currentGameState = nextState;
    }

    VideoPlayer currentPlayer = null;
    public void StartPlayingVideo(VideoPlayer player)
    {
        if (isPlayingContentVideo) {

            currentPlayer.Stop();
            OnContentVideoStop(currentPlayer);
        }

        isPlayingContentVideo = true;
        player.loopPointReached += OnContentVideoStop; // 註冊當撥放結束時的呼叫。
        currentPlayer = player;
    }

    void OnContentVideoStop(VideoPlayer player)
    {
        isPlayingContentVideo = false;
        player.loopPointReached -= OnContentVideoStop;
        currentPlayer = null;
    }
}
