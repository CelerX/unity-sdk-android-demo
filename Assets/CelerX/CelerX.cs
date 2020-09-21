using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Json;
using System;
using System.IO;
using System.Text;

public class CelerX : MonoBehaviour
{

    AndroidJavaObject androidJavaObject;


    CelerXMatchListener listener;

    public void setCelerXGameLifeCycle(CelerXMatchListener listener) {
        this.listener = listener;
    }

    void Awake()
    {
        Debug.Log("CelerXBehaviour Awake");
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        androidJavaObject = jc.GetStatic<AndroidJavaObject>("currentActivity");
    }


    public void submitScore(long score) {
        Debug.Log("CelerXBehaviour submitScore:" + score);
        androidJavaObject.Call("submitScore", score);
    }

    public void launchCelerXUI() {
        Debug.Log("CelerXBehaviour launchCelerXUI");
        androidJavaObject.Call("launchCelerXUI");
    }

    public void ready() {
        Debug.Log("CelerXBehaviour ready");
        androidJavaObject.Call("ready");
    }

    public void onMatchJoined(string matchInfoJson) {
        Debug.Log("onMatchJoined,json:"+matchInfoJson);
        MatchInfo matchInfo = fromJson<MatchInfo>(matchInfoJson);
        if (listener != null) {
            listener.onMatchJoined(matchInfo);
        }
    }


    public void onMatchReadyToStart(string matchInfoJson)
    {
        Debug.Log("onMatchReadyToStart");
        MatchInfo matchInfo = fromJson<MatchInfo>(matchInfoJson);
        if (listener != null) {
            listener.onMatchReadyToStart(matchInfo);
        }
    }


    public static T fromJson<T>(string json)
    {
        Debug.Log("Deserialization object");
        T obj = Activator.CreateInstance<T>();
        MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
        obj = (T)serializer.ReadObject(ms);
        ms.Close();
        return obj;
    }
}
