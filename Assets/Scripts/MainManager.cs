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

                UIManager.Instance.SetInstructionText("���I���U�@�B�}�l���y�C");
                break;
            case GameState.StandByForBaseScan:
                
                baseCardTargetController.gameObject.SetActive(true);

                UIManager.Instance.SetInstructionText("�ж}�l���y��a�d���C");
                break;
            case GameState.PostCardScaning:

                baseCardTargetController.ToggleCanUnlockCards(true);
                UIManager.Instance.SetInstructionText("�i�H�}�l���y����d���C");
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
        player.loopPointReached += OnContentVideoStop; // ���U���񵲧��ɪ��I�s�C
        currentPlayer = player;
    }

    void OnContentVideoStop(VideoPlayer player)
    {
        isPlayingContentVideo = false;
        player.loopPointReached -= OnContentVideoStop;
        currentPlayer = null;
    }
}
