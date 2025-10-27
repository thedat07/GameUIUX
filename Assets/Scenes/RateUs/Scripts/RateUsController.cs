using Creator;
#if UNITY_ANDROID 
using Google.Play.Review;
#endif
using UnityEngine;
using System.Collections;

public class RateUsController : Controller
{
    public const string RATEUS_SCENE_NAME = "RateUs";

    public override string SceneName()
    {
        return RATEUS_SCENE_NAME;
    }

    public StarRateView[] starRateViews;

    [SerializeField] int m_Rate = 5;

#if UNITY_ANDROID
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
#endif

    void Start()
    {
        foreach (var item in starRateViews)
        {
            item.eventClick.AddListener(OnDown);
        }
    }

    public void OnDown(int index)
    {
        m_Rate = index + 1;
        for (int i = 0; i < starRateViews.Length; i++)
        {
            if (starRateViews[i].index <= index)
            {
                starRateViews[i].On();
            }
            else
            {
                starRateViews[i].Off();
            }
        }
    }

    public void OnRate()
    {
        if (m_Rate >= 4)
        {
            StaticData.RateUs = true;
#if UNITY_ANDROID
        StartCoroutine(RequestReviewForAndroid());
#endif

#if UNITY_IOS
        RequestReviewForiOS();
#endif   
        }
        else
        {
            OnKeyBack();
        }
    }

#if UNITY_ANDROID 
    IEnumerator RequestReviewForAndroid()
    {
        _reviewManager = new ReviewManager();

        //Request a review info object
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();

        //Launch the in app review flow
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
        OnKeyBack();
    }
#endif

    void RequestReviewForiOS()
    {
#if UNITY_IOS
        UnityEngine.iOS.Device.RequestStoreReview();
#endif
        OnKeyBack();
    }

}