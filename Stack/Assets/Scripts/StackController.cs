using UnityEngine;
using System.Collections;

public class StackController : MonoBehaviour {
    private GameObject[] stack;
    private int stackIndex;
    private int scoreCount = 0;
    private const float Bounds_Size = 1f;//frequency
    private float tileTransition = 0.0f;
    private float tileSpeed = 2.0f;
    private bool movingOnX = true;
    private Vector3 desiredPosition;
    private const float STACK_MOVING_SPEED = 5.0f;
    private float secondaryPosition;
    private Vector3 lastTilePosition;
    private const float ERROR_MARGIN = 0.1f;
    private int combo = 0;
    private Vector2 stackBounds = new Vector2(Bounds_Size, Bounds_Size);
    private bool gameOver = false;
	// Use this for initialization
	void Start () {
        stack = new GameObject[transform.childCount];
        stackIndex = stack.Length-1;
        
        //adding objects into the stack array
        for (int i =0;i<transform.childCount;i++)
        {
            stack[i] = transform.GetChild(i).gameObject;
        }
       
	}
	
	// Update is called once per frame
	void Update () {
        
        if (Input.GetMouseButtonDown(0))// left click
        {
            if (PlaceTile())
            {
                SpawnTile();
                scoreCount++;
            }
            else
            {
                EndGame();
            }
        }
        MoveTile();
        //move the stack
        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
	}
    private void AddTile()
    {
        stack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
    }
    private void SpawnTile()
    {
        //obtain position of current tile before spawning new tile
        lastTilePosition = stack[stackIndex].transform.localPosition;
        stackIndex--;
        if (stackIndex < 0)
            stackIndex = transform.childCount - 1;
        desiredPosition = (Vector3.down) * scoreCount;
        stack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
       // stackIndex--;
    }
    private bool PlaceTile()//determines if player has lost or not
    {

        Transform t = stack[stackIndex].transform;

        if (movingOnX)
        {
            float deltaX = lastTilePosition.x-t.position.x;
            if (Mathf.Abs(deltaX) > ERROR_MARGIN)
            {
                //Cut the tile. no combo for you!
                combo = 0;//reset combo
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0)
                {
                    print("lost");
                    return false;
                }
                float middle = lastTilePosition.x + t.localPosition.x / 2;
               t.localScale=new Vector3(stackBounds.x, 1, stackBounds.y);
                t.localPosition = new Vector3((middle - (lastTilePosition.x / 2)),scoreCount,lastTilePosition.z);
            }
        }
        else
        {
            float deltaZ = lastTilePosition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {
                //Cut the tile. no combo for you!
                combo = 0;//reset combo
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0)
                {
                    print("lost");
                    return false;
                }
                float middle = lastTilePosition.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle-(lastTilePosition.z/2));
            }

        }
        secondaryPosition = (movingOnX)
            ? t.localPosition.x
            : t.localPosition.z;
        movingOnX = !movingOnX;
        return true;
    }
    private void EndGame()
    {
        Debug.Log("lose");
        gameOver = true;
        stack[stackIndex].AddComponent<Rigidbody>();
        //quit application or got to main menu
    }
    private void MoveTile()
    {
        if (gameOver)
         return;
        
        tileTransition += Time.deltaTime*tileSpeed;// tileSpeed used for adjusting # oscillations per cycle
        
        if (movingOnX)
        {
            stack[stackIndex].transform.localPosition = new Vector3((Mathf.Sin(tileTransition * Bounds_Size)), scoreCount, 0);
             
        }
        else
        {
            stack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, Mathf.Sin(tileTransition* Bounds_Size));
           
        }
    }
}
