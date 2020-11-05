using System.Collections.Generic;
using UnityEngine;
using System.Collections;  
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour, CelerXMatchListener
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public GameObject Pacman;
    public GameObject Blinky;
    public GameObject Cycle;
    public GameObject Inky;
    public GameObject Pinky;

    public GameObject startPanel;
    public GameObject gamePanel;


    public GameObject startCountDownPrefab;
    public GameObject gameOverPrefab;
    public GameObject winPrefab;

    public AudioClip startClip;

    public bool isSuperPacman = false;

    public List<int> usingIndex = new List<int>();
    public List<int> rawIndex = new List<int> { 0, 1, 2, 3 };
    private List<GameObject> pacdotGos = new List<GameObject>();

    private int pacdotNumber = 0;
    private int nowEat = 0;
    public int score = 0;

    public Text remainText;
    public Text nowText;
    public Text scoreText;


    private void Awake()
    {
        _instance = this;
        int tempCount = rawIndex.Count;

        for (int i = 0; i < tempCount; i++)
        {
            int tempIndex = Random.Range(0, rawIndex.Count);
            usingIndex.Add(rawIndex[tempIndex]);
            rawIndex.RemoveAt(tempIndex);
        }

        foreach (Transform t in GameObject.Find("Maze").transform)
        {
            pacdotGos.Add(t.gameObject);
        }

        pacdotNumber = GameObject.Find("Maze").transform.childCount;

        //since the StartScene has destroyed, must reset the callback here.
        CelerX.SetCallback(this);
    }

    private void CreateSuperPacdot()
    {
        if (pacdotGos.Count < 7)
            return;

        int tempIndex = Random.Range(0, pacdotGos.Count);

        //three size of beans
        pacdotGos[tempIndex].transform.localScale = new Vector3(3, 3, 3);

        //super beans
        pacdotGos[tempIndex].GetComponent<Pacdot>().isSupperPacdot = true;
    }

    private void Start()
    {
        // ready to wait start
        SetGameState(false);
    }

    public void OnStartButton()
    {
        StartCoroutine(PlayStartCountDown());

        AudioSource.PlayClipAtPoint(startClip, Vector3.zero);

        startPanel.SetActive(false);
    }

    public void OnExitButton()
    {
#if CELER_ANDROID

        CelerX.SubmitScore(score);
#else
        Application.Quit();
#endif

    }

    IEnumerator PlayStartCountDown()
    {
        GameObject go = Instantiate(startCountDownPrefab);
        yield return new WaitForSeconds(4f);
        Destroy(go);

        SetGameState(true);

        //create super beans interval 10s
        Invoke("CreateSuperPacdot", 10f);

        gamePanel.SetActive(true);

        GetComponent<AudioSource>().Play();
    }

    private void SetGameState(bool state)
    {
        Pacman.GetComponent<PacmanMove>().enabled = state;
        Blinky.GetComponent<EnemyMove>().enabled = state;
        Cycle.GetComponent<EnemyMove>().enabled = state;
        Inky.GetComponent<EnemyMove>().enabled = state;
        Pinky.GetComponent<EnemyMove>().enabled = state;
    }

    public void OnEatPacdot(GameObject go)
    {
        pacdotGos.Remove(go);

        nowEat++;
        score += 100;
    }

    public void OnEatSuperPacdot()
    {
        //add score
        score += 200;

        Invoke("CreateSuperPacdot", 10f);

        isSuperPacman = true;
        FreezeEnemy();

        StartCoroutine(Recover());
    }

    IEnumerator Recover()
    {
        yield return new WaitForSeconds(4f);

        Dis_FreezeEnemy();
        isSuperPacman = false;
    }

    private void FreezeEnemy()
    {
        Blinky.GetComponent<EnemyMove>().enabled = false;
        Cycle.GetComponent<EnemyMove>().enabled = false;
        Inky.GetComponent<EnemyMove>().enabled = false;
        Pinky.GetComponent<EnemyMove>().enabled = false;

        Blinky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        Cycle.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        Inky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        Pinky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
    }

    private void Dis_FreezeEnemy()
    {
        Blinky.GetComponent<EnemyMove>().enabled = true;
        Cycle.GetComponent<EnemyMove>().enabled = true;
        Inky.GetComponent<EnemyMove>().enabled = true;
        Pinky.GetComponent<EnemyMove>().enabled = true;

        Blinky.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        Cycle.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        Inky.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        Pinky.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    public void OnWinButton()
    {
#if CELER_ANDROID 
        CelerX.SubmitScore(score);

#else
        SceneManager.LoadScene(1);
#endif
    }

    public void OnPersonaButton()
    {
    }

    private void Update()
    {
        if (pacdotNumber == nowEat && Pacman.GetComponent<PacmanMove>().enabled != false)
        {
            gamePanel.SetActive(false);
            winPrefab.SetActive(true);

     
            //取消其他所有协程
            StopAllCoroutines();

            SetGameState(false);

#if CELER_ANDROID

            CelerX.SubmitScore(score);
#else
#endif
        }

        if (pacdotNumber == nowEat)
        {
            //
            if (Input.GetKey(KeyCode.Z))
            {
                OnPersonaButton();

                //Invoke("", 5f);
                //SceneManager.LoadScene(0);
                //Application.OpenURL("https://www.cnblogs.com/SouthBegonia");
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                OnWinButton();
            }

        }

        if (gamePanel.activeInHierarchy)
        {
            remainText.text = "Remain:\n\n" + (pacdotNumber - nowEat);
            nowText.text = "Eaten:\n\n" + nowEat;
            scoreText.text = "Score:\n\n" + score;
        }
    }

    void CelerXMatchListener.onMatchJoined(MatchInfo mathInfo)
    {
        double randomSeed = mathInfo.sharedRandomSeed;
        Debug.Log("random seed " + randomSeed);

        CelerX.Ready();
    }

    void CelerXMatchListener.onMatchReadyToStart(MatchInfo mathInfo)
    {
        // load the game play scene
        SceneManager.LoadScene("SampleScene");
    }
}
