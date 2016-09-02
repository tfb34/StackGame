using UnityEngine;
using System.Collections;

public class StackController : MonoBehaviour {
    private GameObject[] stack;
    private int stackIndex;
    private int scoreCount = 0;
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
            AddTile();
        }
	}
    private void AddTile()
    {
        stack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
    }
}
