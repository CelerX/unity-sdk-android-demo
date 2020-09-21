using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public class StartScene : MonoBehaviour, CelerXMatchListener
{

    public Button startButton;

    public CelerX celerX;
    // Start is called before the first frame update

    void Awake()
    {
        Debug.Log("StartScene Awake");
        celerX.setCelerXGameLifeCycle(this);
    }

    public void startGame()
    {
        celerX.launchCelerXUI();
    }

    public void onMatchJoined(MatchInfo matchInfo) {
        Debug.Log("OnMatchJoined, Start to render game ui with matchInfo");

        // todo render game ui, then call ready function
        Debug.Log("sharedRandomSeed:" + matchInfo.sharedRandomSeed);
        Thread.Sleep(1000);

        //Notity celerx render finish. show game ready button
        celerX.ready();
    }

    public void onMatchReadyToStart(MatchInfo mathInfo) {
        // ready status has confirmed by celerX, then you can start your game
        SceneManager.LoadScene("Main");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
