using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MyTargetImagePrefabBaseController : DefaultObserverEventHandler
{
    [SerializeField]
    List<PostCardBehaviour> loadedPostcardList = new List<PostCardBehaviour>();

    bool isTrackingActivated = false; // �n���ݰ�a�d���Q���y��~�}�l�i�H����䤤���d��

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        MainManager.Instance.ChangeGameState(GameState.PostCardScaning); // ���ܹC�����A�����H�����y��
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();

        MainManager.Instance.ChangeGameState(GameState.StandByForBaseScan); // ���ܹC�����A�����ݰ�a���y
    }

    public void ToggleCanUnlockCards(bool enable)
    {
        isTrackingActivated = enable;

        if ( enable) {
            foreach(PostCardBehaviour postcard in loadedPostcardList) {
                postcard.OnEnterScanPostcardMode(); // �i�J���y���H���Ҧ�
            }
        }
    }

    public void UnlockPostcard(string postcardID)
    {
        if ( !isTrackingActivated) {
            Debug.LogWarning("Tracking is not activated. Cannot unlock postcards.");
            return;
        }

        //�bloadedPostcardList����Xid�PpostcardID�ۦP��PostCardBehaviour
        PostCardBehaviour postcardBehaviour = loadedPostcardList.Find(p => p.postcardID == postcardID);

        if (postcardBehaviour != null) {
            //�p�G���F�A�N�����PostCard
            postcardBehaviour.ActivatePostcard();
        }
        else {
            Debug.LogWarning("Postcard with ID " + postcardID + " not found.");
        }
    }
}
