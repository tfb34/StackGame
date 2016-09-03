using UnityEngine;
using System.Collections;

public class StackController : MonoBehaviour {
    private GameObject[] stack;
    private int stackIndex;
    private int scoreCount = 0;
    private const float Bounds_Size = 3.5f;
    private float tileTransition = 0.0f;
    private float tileSpeed = 0.5f;
    private bool movingOnX = true;
    private Vector3 desiredPosition;
    private const float STACK_MOVING_SPEED = 5.0f;
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
        stackIndex--;
        if (stackIndex < 0)
            stackIndex = transform.childCount - 1;
        desiredPosition = (Vector3.down) * scoreCount;
        stack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
       // stackIndex--;
    }
    private bool PlaceTile()//determines if player has lost or not
    {
        movingOnX = !movingOnX;
        return true;
    }
    private void EndGame()
    {
        //quit application or got to main menu
    }
    private void MoveTile()
    {
        tileTransition += Time.deltaTime * tileSpeed;
        if (movingOnX)
        {
            stack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition * Bounds_Size), scoreCount, 0);
             
        }
        else
        {
            stack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, Mathf.Sin(tileTransition * Bounds_Size));
           
        }
    }
}
