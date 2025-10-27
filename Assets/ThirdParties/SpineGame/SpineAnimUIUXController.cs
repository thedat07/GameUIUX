using UnityEngine;
using Spine;
using Spine.Unity;
using UnityEngine.Events;

public class SpineAnimUIUXController : MonoBehaviour
{
    SkeletonGraphic m_SkeletonGraphic;

    public SkeletonGraphic GetSkeletonGraphic()
    {
        if (m_SkeletonGraphic == null)
        {
            m_SkeletonGraphic = gameObject.GetComponent<SkeletonGraphic>();
        }
        return m_SkeletonGraphic;
    }

    /// <summary>
    /// Mode 1: Play Idle 5 lần rồi Action — có thể loop
    /// </summary>
    public void PlayIdle5ThenAction()
    {
        var skeletonGraphic = GetSkeletonGraphic();
        if (skeletonGraphic == null) return;

        ResetSkeleton();
        PlayAnimationQueue(new string[] { "Idle", "Idle", "Idle", "Idle", "Action" }, true);
    }

    public void PlayIdleThenAction()
    {
        var skeletonGraphic = GetSkeletonGraphic();
        if (skeletonGraphic == null) return;

        ResetSkeleton();
        PlayAnimationQueue(new string[] { "Idle", "Action", "Idle", "Idle", "Action" }, true);
    }

    public void PlayIdleThenActionClick(UnityAction calBack)
    {
        var skeletonGraphic = GetSkeletonGraphic();
        if (skeletonGraphic == null) return;

        ResetSkeleton();
        PlayAnimationQueue(new string[] { "Action", "Idle", "Idle", "Idle" }, false, callBack: calBack);
    }

    public void PlayIdleHand()
    {
        var skeletonGraphic = GetSkeletonGraphic();
        if (skeletonGraphic == null) return;

        ResetSkeleton();
        PlayAnimationQueue(new string[] { "Tap", "Idle", "Idle", "Tap", "Idle", "Idle", "Idle", "Tap", "Tap" }, true);
    }

    public void PlayActionOnly(bool loop = true)
    {
        var skeletonGraphic = GetSkeletonGraphic();
        if (skeletonGraphic == null) return;

        ResetSkeleton();

        var state = skeletonGraphic.AnimationState;

        state.SetAnimation(0, "Action", loop);
    }

    public void PlayOpen()
    {
        var skeletonGraphic = GetSkeletonGraphic();
        if (skeletonGraphic == null) return;

        ResetSkeleton();

        var state = skeletonGraphic.AnimationState;

        state.SetAnimation(0, "Open", false);
    }

    public void PlayActionIdleLoop()
    {
        var skeletonGraphic = GetSkeletonGraphic();
        if (skeletonGraphic == null) return;

        ResetSkeleton();

        var state = skeletonGraphic.AnimationState;

        state.SetAnimation(0, "Action", false);
        state.AddAnimation(0, "Idle", true, 0);
    }

    public void PlayIdleOnly(bool loop = true)
    {
        var skeletonGraphic = GetSkeletonGraphic();
        if (skeletonGraphic == null) return;

        ResetSkeleton();

        var state = skeletonGraphic.AnimationState;

        state.SetAnimation(0, "Idle", loop);
    }

    private void ResetSkeleton()
    {
        //GetSkeletonGraphic().Initialize(true);
        // Dừng mọi animation đang chạy
        GetSkeletonGraphic().AnimationState.ClearTracks();
        GetSkeletonGraphic().AnimationState.SetEmptyAnimations(0);

        // Reset toàn bộ xương và slot về setup pose
        GetSkeletonGraphic().Skeleton.SetToSetupPose();
        GetSkeletonGraphic().Skeleton.UpdateWorldTransform(Skeleton.Physics.Update);

        GetSkeletonGraphic().LateUpdate();
    }

    public void PlayAnimationQueue(
       string[] animationNames,
       bool loop = false,
       bool separateTracks = false,
       UnityAction callBack = null)
    {
        if (GetSkeletonGraphic() == null || animationNames == null || animationNames.Length == 0)
            return;

        int track = 0;
        TrackEntry lastTrackEntry = null;
        var state = GetSkeletonGraphic().AnimationState;

        foreach (string animationName in animationNames)
        {
            // Thêm animation vào hàng đợi (có delay ngẫu nhiên)
            lastTrackEntry = state.AddAnimation(
                separateTracks ? track++ : 0,
                animationName,
                false,
                0
            );
            lastTrackEntry.Complete += (TrackEntry entry) =>
            {
                if (callBack != null)
                    callBack?.Invoke();
            };
        }

        if (loop && lastTrackEntry != null)
        {
            lastTrackEntry.Complete += (TrackEntry entry) =>
            {
                state.ClearTracks();
                PlayAnimationQueue(animationNames, loop, separateTracks);
            };
        }
    }
}

