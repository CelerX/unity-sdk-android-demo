using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Timers;

public class CelerX:MonoBehaviour
{
    static AndroidJavaClass _jc;
    static AndroidJavaClass jc
    {
        get
        {
            if (_jc == null)
            {
                Debug.Log("CelerXBehaviour Awake");
                _jc = new AndroidJavaClass("celerx.sdk.plugin.UnityMessagePlugin");
                _CallbackInit(onCallBack);

            }
            return _jc;
        }
    }

    private static Timer timer;

    [DllImport("celerx-jni")]
    private static extern void _CallbackInit(CelerXCallback callback);

    static CelerXMatchListener listener;

    public static void SetCallback(CelerXMatchListener listener)
    {
        CelerX.listener = listener;
    }

    public static void SubmitScore(long score)
    {
        Debug.Log("CelerXBehaviour submitScore:" + score);
        jc.CallStatic("submitScore", score);

        if (timer != null)
        {
            Debug.Log("submit score success, stop timer");
            timer.Enabled = false;
        }
    }

    public static void PresentSDKUI()
    {
        Debug.Log("CelerXBehaviour launchCelerXUI");
        jc.CallStatic("launchCelerXSplashUI");
    }

    public static void Ready()
    {
        Debug.Log("CelerXBehaviour ready");
        jc.CallStatic("ready");
    }

    internal delegate void CelerXCallback(string eventName, string msg);


    [AOT.MonoPInvokeCallback(typeof(CelerXCallback))]
    private static void onCallBack(string eventName, string msg)
    {
        if (eventName == "onMatchJoined")
        {
            //do onMatchJoined
            Debug.Log("onMatchJoined,json:" + msg);
            MatchInfo matchInfo = JsonUtility.FromJson<MatchInfo>(msg);
            Debug.Log("listener is " + listener);
            if (listener != null)
            {
                listener.onMatchJoined(matchInfo);
            }
        } else if (eventName == "onMatchReadyToStart") {
            Debug.Log("onMatchReadyToStart");
            MatchInfo matchInfo = JsonUtility.FromJson<MatchInfo>(msg);

            if (listener != null)
            {
                listener.onMatchReadyToStart(matchInfo);

                if (matchInfo.isNeedRecordScreen)
                {
                    Debug.Log("start to record screen for this game");
                    
                    timer = new Timer();
                    timer.Elapsed += delegate {
                        var timestamp = ((DateTimeOffset)DateTime.UtcNow.ToLocalTime()).ToUnixTimeMilliseconds();
                        string picName = "screenshot-" + matchInfo.matchId + "-" + timestamp + ".jpeg";
                        TakeScreenshot(picName);
                	};
                    //timer.Elapsed += new ElapsedEventHandler(TakeScreenshot);
                    timer.Interval = 1000;
                    timer.Enabled = true;
                }
            }
        }
    }

    public static void TakeScreenshot(string picName)
    {
        ScreenCapture.CaptureScreenshot(picName);

        var picPath = Application.persistentDataPath + picName;

        Debug.Log("CelerXBehaviour didTakeSnapshot path is "+ picPath);

        //jc.CallStatic("didTakeSnapshot", picPath);

    }
}
