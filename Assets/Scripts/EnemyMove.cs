using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMove : MonoBehaviour
{

    public float speed = 0.15f;
    private int index = 0;
    private float Magic = 0.3f;

    public GameObject[] wayPointsGo;
    private List<Vector3> wayPoints = new List<Vector3>();

    private Vector3 startPos;


    private void Start()
    {
        startPos = transform.position + new Vector3(0, 3, 0);

        LoadApath(wayPointsGo[GameManager.Instance.usingIndex[GetComponent<SpriteRenderer>().sortingOrder - 2]]);

    }

    private void FixedUpdate()
    {
        if (transform.position != wayPoints[index])
        {
            Vector2 temp = Vector2.MoveTowards(transform.position, wayPoints[index], speed);
            GetComponent<Rigidbody2D>().MovePosition(temp);
        }
        else
        {

            index++;
            if (index >= wayPoints.Count)
            {
                index = 0;
                LoadApath(wayPointsGo[Random.Range(0, wayPointsGo.Length)]);
            }
        }

        Vector2 dir = wayPoints[index] - transform.position;

        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    private void LoadApath(GameObject go)
    {
        wayPoints.Clear();

        foreach (Transform t in go.transform)
        {
            wayPoints.Add(t.position);
        }

        //add fist and last path 
        wayPoints.Insert(0, startPos);
        wayPoints.Add(startPos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pacman")
        {
            if (GameManager.Instance.isSuperPacman)
            {
                transform.position = startPos - new Vector3(0, 3, 0);
                index = 0;

                GameManager.Instance.score += 500;
            }
            else
            {
                //hide the Pacman, avoid Destory
                collision.gameObject.SetActive(false);

                //game over, hide Panel
                GameManager.Instance.gamePanel.SetActive(false);
                Instantiate(GameManager.Instance.gameOverPrefab);

#if CELER_ANDROID
                CelerX.SubmitScore(GameManager.Instance.score);

#else
                //restart scence after 3s
                Invoke("ReStart", 3f);
#endif

            }

        }
    }

    private void ReStart()
    {
        
         SceneManager.LoadScene(1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            speed += Magic;
            Magic = -Magic;
        }
    }
}