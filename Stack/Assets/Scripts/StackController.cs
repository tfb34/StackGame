using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StackController : MonoBehaviour {
    private GameObject[] stack;
    private int stackIndex;
    private int scoreCount = 0;
    private const float Bounds_Size = 3.5f;//frequency
    private float tileTransition = 0.0f;
    private float tileSpeed = 2f;
    private bool movingOnX = true;
    private Vector3 desiredPosition;
    private const float STACK_MOVING_SPEED = 5.0f;
    private float secondaryPosition;
    private Vector3 lastTilePosition;
    private const float ERROR_MARGIN = 0.25f;
    private int combo = 0;
    private Vector2 stackBounds = new Vector2(Bounds_Size, Bounds_Size);
    private bool gameOver = false;
    private const float STACK_BOUNDS_GAIN = 0.25f;
    private const int COMBO_START_GAIN = 3;
    public Color32[] gameColors = new Color32[4];
    public Material stackMat;
    public Text scoreText;
    public GameObject EndPanel;
	private Color32[] currentColor;

	// Use this for initialization
	void Start () {
        stack = new GameObject[transform.childCount];
        stackIndex = stack.Length-1;
        
        //adding objects into the stack array
        for (int i =0;i<transform.childCount;i++)
        {
            stack[i] = transform.GetChild(i).gameObject;
            ColorMesh(stack[i].GetComponent<MeshFilter>().mesh);
        }
		colorCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
        
        if (Input.GetMouseButtonDown(0))// left click
        {
            if (PlaceTile())
            {
                SpawnTile();
                scoreCount++;
                scoreText.text = scoreCount.ToString();
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
        stack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        ColorMesh(stack[stackIndex].GetComponent<MeshFilter>().mesh);
    }
	int colorCount = 0;
    private void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];//24 
		if (colorCount>15) {
			print ("called");
			print ("colorCount:  "+colorCount);
			print (scoreCount / 15);
			colorCount = 0;
		}
		colorCount+=1;
		float f = Mathf.Sin( colorCount* 0.25f);
        for (int i = 0; i < vertices.Length; i++)
            colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3],f);
		currentColor = colors;
        mesh.colors32 = colors;
    }


    private bool PlaceTile()
    {//determines if player has lost or not

        Transform t = stack[stackIndex].transform;

        if (movingOnX)
        {
            float deltaX = lastTilePosition.x-t.position.x;
            //if player does not do a good job of placing tile on stack
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
                CreateRubble
                (
                    new Vector3((t.position.x>0)
                        ?t.position.x + (t.localScale.x / 2)
                        :t.position.x-(t.localScale.x/2)
                        ,t.position.y
                        , t.position.z),
                    new Vector3(Mathf.Abs(deltaX), 1, t.localScale.z)//localScaleY?
                    );
                t.localPosition = new Vector3(middle - (lastTilePosition.x / 2),scoreCount,lastTilePosition.z);
            }//else player has done well, increase combo, no rescaling
            else
            {
                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.x += STACK_BOUNDS_GAIN;

                    //rescale blocks for player's advantage unless
                    if (stackBounds.x > Bounds_Size)
                        stackBounds.x = Bounds_Size;
                    float middle = lastTilePosition.x + t.localPosition.x / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);//lastTileposition.y?
                }
                combo++;
                // t.localPosition = lastTilePosition + Vector3.up;
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
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
                CreateRubble(
                    new Vector3(
                        t.position.x
                    , t.position.y
                    , (t.position.z > 0)
                    ? t.position.z + (t.localScale.z / 2)
                    : t.position.z - (t.localScale.z / 2)),
                    new Vector3(t.localScale.x, 1, Mathf.Abs(deltaZ))//!!!!!!!!!!
                   
                    );
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle-(lastTilePosition.z/2));
            }
            else
            {
                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.y += STACK_BOUNDS_GAIN;
                    //rescale blocks for player's advantage 
                    if (stackBounds.y > Bounds_Size)
                        stackBounds.y = Bounds_Size;
                    float middle = lastTilePosition.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
                }
                combo++;
                //t.localPosition = lastTilePosition + Vector3.up; 
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
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
        // Debug.Log("lose");
        if (PlayerPrefs.GetInt("score") < scoreCount)
            PlayerPrefs.SetInt("score", scoreCount);
        gameOver = true;
        EndPanel.SetActive(true);
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
            stack[stackIndex].transform.localPosition = new Vector3((Mathf.Sin(tileTransition)*Bounds_Size), scoreCount, secondaryPosition);
             
        }
        else
        {
            stack[stackIndex].transform.localPosition = new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition)*Bounds_Size);
           
        }
    }
    private void CreateRubble(Vector3 pos, Vector3 scale)
    {//independent cubes
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();
        go.GetComponent<MeshRenderer>().material = stackMat;
        //ColorMesh(go.GetComponent<MeshFilter>().mesh);
		Mesh m = go.GetComponent<MeshFilter>().mesh;
		m.colors32 = currentColor;
    }
    private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t){
        if (t < 0.33f)
            return Color.Lerp(a, b, t / 0.33f);
        else if (t < 0.66f)
            return Color.Lerp(b, c, (t - 0.33f) / 0.33f);
        else
            return Color.Lerp(c, d, (t - 0.66f) / 0.66f);
    }
    public void OnButtonClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
