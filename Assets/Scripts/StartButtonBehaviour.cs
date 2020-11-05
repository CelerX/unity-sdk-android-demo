using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonBehaviour : MonoBehaviour, CelerXMatchListener
{
    public Canvas canvas;

    public void Awake()
    {
        CelerX.SetCallback(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonPress()
    {
        Debug.Log("Button clicked");

#if CELER_ANDROID
        CelerX.PresentSDKUI();
#else
        SceneManager.LoadScene("SampleScene");
#endif

        //gameObject.SetActive(false);
        ////hide current scence
        //canvas.enabled = false;

    }

    public void onMatchJoined(MatchInfo mathInfo)
    {
        double randomSeed = mathInfo.sharedRandomSeed;
        Debug.Log("random seed " + randomSeed);

        CelerX.Ready();

    }

    public void onMatchReadyToStart(MatchInfo mathInfo)
    {
        // load the game play scene
        SceneManager.LoadScene("SampleScene");
    }

}
