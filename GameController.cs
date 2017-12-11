using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public GameObject cubePrefab, nextCubePrefab;
	Vector3 gridCubePosition, nextCubePosition;
	public GameObject[,] gridCubes;
	public GameObject nextCube;
	float eachTurnTime, turnTimer, eachGameTime, gameTimer;
	int gridCubeMaxX, gridCubeMaxY;
	Color[] cubeColor;
	Color currentNextCubeColor;
	int coloredX;
	public GameObject blackCubes;
	public int blackX, blackY;
	bool detectKeyboardInput;
	public GameObject activeCube;
	public Text nextCubeText, gameOverText, scoreText;


	void Start() 
	{
		gridCubeMaxX = 8;
		gridCubeMaxY = 5;
		gridCubes = new GameObject[gridCubeMaxX, gridCubeMaxY];

		blackX = 0;
		blackY = 0;

		detectKeyboardInput = false;

	

		eachTurnTime = 2.0f;
		turnTimer = eachTurnTime;
		eachGameTime = 60.0f;
		gameTimer = eachGameTime;

		cubeColor = new Color[5];
		cubeColor [0] = Color.blue;
		cubeColor [1] = Color.green;
		cubeColor [2] = Color.red;
		cubeColor [3] = Color.yellow;
		cubeColor [4] = Color.magenta;

		for (int x = 0; x < gridCubeMaxX; x++) 
		{
			for (int y = 0; y < gridCubeMaxY; y++)
			{
				gridCubePosition = new Vector3 (x*2, y*2, 0);
				gridCubes [x,y] = Instantiate (cubePrefab, gridCubePosition, Quaternion.identity);			
				gridCubes [x,y].GetComponent<Renderer>().material.color = Color.white;
			}
		}
		//test this linenextCubenextCube
		NewNextCube ();
	}

	//set for X position, check if it is occupied, change the color there
	public void GetInThereNextCube(int y)
	{

		List<GameObject> whiteCubesInLine = new List<GameObject> ();

		for (int x = 0; x < gridCubeMaxX; x++) {
			if (gridCubes[x, y].GetComponent<Renderer>().material.color == Color.white) {
				whiteCubesInLine.Add(gridCubes[x,y]);
			}
		}

		if (whiteCubesInLine.Count == 0) {
			EndGame (false);
		}

		else {GameObject theChosenOneInTheGrid;
			theChosenOneInTheGrid = whiteCubesInLine [Random.Range (0, whiteCubesInLine.Count)];
			theChosenOneInTheGrid.GetComponent<Renderer>().material.color = currentNextCubeColor;
			theChosenOneInTheGrid.GetComponent<CubeBehavior>().isColored = true;
			Destroy(nextCube);
		}
	}

	//fail to press keyboard on time, blacken cubesselec
	public void BlackenCubes()
	{
		List<GameObject> whiteCubesAll = new List<GameObject> ();

		for (int x = 0; x < gridCubeMaxX; x++) {
			for (int y = 0; y < gridCubeMaxY; y++) {
				if (gridCubes[x, y].GetComponent<Renderer>().material.color == Color.white) 
				{
					whiteCubesAll.Add(gridCubes[x,y]);
				}
			}
		}

		blackCubes = whiteCubesAll [Random.Range (0, whiteCubesAll.Count)];
		blackCubes.GetComponent<Renderer>().material.color = Color.black;
		gridCubes[blackX, blackY].GetComponent<CubeBehavior>().isBlacked = true;
		//cannot be destroyed
		Destroy(nextCube);
	}




	public void KeyboardInput ()
	{
		if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1))
		{
			GetInThereNextCube(4);
			detectKeyboardInput = true;
		}
		else if (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2))
		{
			GetInThereNextCube(3);
			detectKeyboardInput = true;
		}

		else if (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3))
		{
			GetInThereNextCube(2);
			detectKeyboardInput = true;
		}

		else if (Input.GetKey(KeyCode.Alpha4) || Input.GetKey(KeyCode.Keypad4))
		{
			GetInThereNextCube(1);
			detectKeyboardInput = true;
		}
		else if (Input.GetKey (KeyCode.Alpha5) || Input.GetKey (KeyCode.Keypad5)) {
			GetInThereNextCube (0);
			detectKeyboardInput = true;
		} 
	}

	public void NewNextCube()
	{
		nextCubePosition = new Vector3 (20, 10, 0);
		nextCube = Instantiate (nextCubePrefab, nextCubePosition, Quaternion.identity);
		currentNextCubeColor = 	cubeColor [Random.Range (0,5) ];
		nextCube.GetComponent<Renderer>().material.color = currentNextCubeColor;

	}

	public void ProcessClick(GameObject selectedCube, int x, int y, Color selectedCubeColor)
	{
		if (selectedCubeColor != Color.white && selectedCubeColor != Color.black) 
		{
			// if there is an active cube
			if (activeCube != null) {
				if (selectedCube != activeCube) {
					activeCube.transform.localScale /= 1.2f;
					selectedCube.transform.localScale *= 1.2f;
					activeCube = selectedCube;
				} 
				else {
					activeCube.transform.localScale /= 1.2f;
					activeCube = null;
				}
			}

			//if there is no active cube yet
			else {
				selectedCube.transform.localScale *= 1.2f;
				activeCube = selectedCube;
			}

		}
		else if (selectedCubeColor == Color.white && activeCube != null) 
		{
			int xDist = selectedCube.GetComponent<CubeBehavior> ().cubeX - activeCube.GetComponent<CubeBehavior> ().cubeX;
			int yDist = selectedCube.GetComponent<CubeBehavior> ().cubeY - activeCube.GetComponent<CubeBehavior> ().cubeY;

			if (Mathf.Abs (xDist) <= 1 && Mathf.Abs (yDist) <= 1) 
			{
				//set the new selected cube to be active and set the old active cube to be white and inactive
				selectedCube.transform.localScale *= 1.2f;
				activeCube.transform.localScale /= 1.2f;

				selectedCube.GetComponent<Renderer> ().material.color = activeCube.GetComponent<Renderer> ().material.color;
				activeCube.GetComponent<Renderer> ().material.color = Color.white;

				activeCube = null;
				activeCube = selectedCube;
			}					
		} 
	}


	void EndGame(bool win)
		{
			if (win)
			{
			gameOverText.text = "YOU WIN!";
			print ("win");
			}
			else 
			{
			gameOverText.text = "GG!";
			print ("lose");
			}
		}

	void Update()
	{
		if (Time.time < gameTimer) {
			//press key
			if (detectKeyboardInput == false) {
				KeyboardInput ();
			}


			if (Time.time > turnTimer)
			{
				if (detectKeyboardInput == false) {
					BlackenCubes ();
				}

				NewNextCube ();

				detectKeyboardInput = false;
					

				turnTimer += eachTurnTime;
			}

		}
	}
}
