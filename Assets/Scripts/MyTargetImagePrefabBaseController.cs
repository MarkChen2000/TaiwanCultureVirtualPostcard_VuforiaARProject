using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MyTargetImagePrefabBaseController : DefaultObserverEventHandler
{
    [SerializeField]
    List<PostCardBehaviour> loadedPostcardList = new List<PostCardBehaviour>();

    bool isTrackingActivated = false; // 要等待基地卡片被掃描到才開始可以解鎖其中的卡片

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        MainManager.Instance.ChangeGameState(GameState.PostCardScaning); // 改變遊戲狀態為明信片掃描中
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();

        MainManager.Instance.ChangeGameState(GameState.StandByForBaseScan); // 改變遊戲狀態為等待基地掃描
    }

    public void ToggleCanUnlockCards(bool enable)
    {
        isTrackingActivated = enable;

        if ( enable) {
            foreach(PostCardBehaviour postcard in loadedPostcardList) {
                postcard.OnEnterScanPostcardMode(); // 進入掃描明信片模式
            }
        }
    }

    public void UnlockPostcard(string postcardID)
    {
        if ( !isTrackingActivated) {
            Debug.LogWarning("Tracking is not activated. Cannot unlock postcards.");
            return;
        }

        //在loadedPostcardList中找出id與postcardID相同的PostCardBehaviour
        PostCardBehaviour postcardBehaviour = loadedPostcardList.Find(p => p.postcardID == postcardID);

        if (postcardBehaviour != null) {
            //如果找到了，就解鎖該PostCard
            postcardBehaviour.ActivatePostcard();
        }
        else {
            Debug.LogWarning("Postcard with ID " + postcardID + " not found.");
        }
    }
}
