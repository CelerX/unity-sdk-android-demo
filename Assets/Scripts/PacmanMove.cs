using UnityEngine;

public class PacmanMove : MonoBehaviour
{

    //move speed
    public float speed = 0.35f;

    private float Magic = 0.35f;

    //next destination
    private Vector2 dest = Vector2.zero;

    private void Start()
    {
        // initialize position
        dest = this.transform.position;

    }


    private void Update()
    {
        Vector2 temp = Vector2.MoveTowards(transform.position, dest, speed);

        GetComponent<Rigidbody2D>().MovePosition(temp);

        if ((Vector2)transform.position == dest)
        {

#if CELER_ANDROID
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Debug.Log("angle altitude: " + touch.altitudeAngle);
                Debug.Log("angle azimuthAngle: " + touch.azimuthAngle);

                Debug.Log("touch.deltaPosition.x" + touch.deltaPosition.x);
                Debug.Log("touch.deltaPosition.y" + touch.deltaPosition.x);

                if (System.Math.Abs(touch.deltaPosition.x) > System.Math.Abs(touch.deltaPosition.y))
                {
                    if ((touch.deltaPosition.x > 0) && Valid(Vector2.right))
                    {
                        // right
                        dest = (Vector2)transform.position + Vector2.right;
                    }
                    else if ((touch.deltaPosition.x < 0) && Valid(Vector2.left))
                    {
                        dest = (Vector2)transform.position + Vector2.left;
                    }
                }
                else
                {
                    if ((touch.deltaPosition.y > 0) && Valid(Vector2.up))
                    {
                        dest = (Vector2)transform.position + Vector2.up;
                    }
                    else if ((touch.deltaPosition.y < 0) && Valid(Vector2.down))
                    {
                        dest = (Vector2)transform.position + Vector2.down;
                    }
                }

            }
#else

            //key input watch
            if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && Valid(Vector2.up))
            {
                //当前位置加向上一个单位
                dest = (Vector2)transform.position + Vector2.up;
            }
            if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && Valid(Vector2.down))
            {
                dest = (Vector2)transform.position + Vector2.down;
            }
            if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && Valid(Vector2.left))
            {
                dest = (Vector2)transform.position + Vector2.left;

            }
            if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && Valid(Vector2.right))
            {
                dest = (Vector2)transform.position + Vector2.right;
            }
#endif
        }
        // get direction
        Vector2 dir = dest - (Vector2)transform.position;

        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);


        if (Input.GetKeyDown(KeyCode.W))
        {
            speed += Magic;
            Magic = -Magic;
        }
    }

    //validate the destination position
    private bool Valid(Vector2 dir)
    {
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);

        return (hit.collider == GetComponent<Collider2D>());
    }
}
