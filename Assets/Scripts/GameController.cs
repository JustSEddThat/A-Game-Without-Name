using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour 
{


	public Camera mCam;
	float camMin, camMax;

	[SerializeField]
	private int level;


	public GameObject[] puzzlePieces;

	public GameObject selectedObject;

	public GameObject[] targets;

	private Material[] materials;

	private string pathForThisLevelImages;
	private string pathForMaterials = "Material/";

	#region For Spawning Targets
	public int numPieces;
	float width = 10, height = 10;
	Vector3 originPoint;
	int rows = 2;
	int cols;
	float pWidth, pHeight;
	#endregion

	// Use this for initialization
	void Start () 
	{
		camMin = 5;
		camMax = 15;
		selectedObject = null;
		Init ();
	}

	void FixedUpdate()
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			if (selectedObject != null) 
			{
				selectedObject.transform.position = AlignToTarget (selectedObject);
				selectedObject = null;
			}
			else 
			{
				RaycastHit hit;
				if (Physics.Raycast(mCam.ScreenPointToRay (Input.mousePosition), out hit))
					selectedObject = hit.collider.gameObject;
			}
		}

		if (selectedObject != null) 
			selectedObject.transform.position = new Vector3 (mCam.ScreenToWorldPoint (Input.mousePosition).x, mCam.ScreenToWorldPoint (Input.mousePosition).y, selectedObject.transform.position.z);

	}


	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.H))
			SceneManager.LoadScene ("Test 2");
		if (Input.GetKeyDown (KeyCode.G))
			SceneManager.LoadScene ("Test");
		
		if (Input.GetKeyDown (KeyCode.Alpha1)) 
		{
			StartCoroutine (CameraZoomIn ());
		} 
		else if (Input.GetKeyDown (KeyCode.Alpha2))
			StartCoroutine(CameraZoomOut());

		if (Input.GetKeyDown (KeyCode.N))
			Debug.Log(CheckForCorrect ());
	}

	void Init()
	{

		pathForThisLevelImages = "Pictures/L" + level+"/";

		//Set all active
		foreach (GameObject go in targets)
			go.SetActive (true);

		foreach (GameObject go in puzzlePieces)
			go.SetActive (true);

		materials = Resources.LoadAll<Material> (pathForMaterials);
		Texture[] textures = Resources.LoadAll<Texture> (pathForThisLevelImages);
		//numPieces should be equal to the number of textures in path
		numPieces = textures.Length;

		//Give the materials the appropriate textures
		for (int i = 0; i < numPieces; i++)
			materials [i].mainTexture = textures [i];
		

		cols = numPieces / rows;
		pWidth = width / cols;
		pHeight = height / 2;
		originPoint = transform.position;

		//Change Puzzle pieces size
		foreach (GameObject go in puzzlePieces)
			go.transform.localScale = new Vector3(pWidth, pHeight, go.transform.localScale.z);



		//Deactivate extra targets and pieces
		for (int i = targets.Length-1; numPieces <= i; i--)
		{
			targets [i].SetActive (false);
			Debug.Log ("one");
		}

		for (int i = puzzlePieces.Length-1; numPieces <= i; i--)
			puzzlePieces [i].SetActive (false);

		SpawnTargets ();

	}

	bool CheckForCorrect()
	{
		bool allCorrect = true;
		for (int i = 0; i < numPieces; i++)
			if ((Vector2)puzzlePieces [i].transform.position != (Vector2)targets [i].transform.position)
				allCorrect = false;

		return allCorrect;
	}

	void SpawnTargets()
	{

		int arrayIndex = 0;
		switch (numPieces) 
		{
			case 4:
			for (int row = 1; row >= -1; row-=2)
				for (int col = -1; col <= 1; col+=2)
				{
					targets [arrayIndex].transform.position = new Vector3 (originPoint.x + pWidth / 2 * col, originPoint.y + pHeight/2* row,
																			targets [arrayIndex].transform.position.z);
						arrayIndex++;
				}
				 
				break;

			case 6:
			for (int row = 1; row >= -1; row-=2)
				for (int col = -1; col <= 1; col++)
				{
					targets [arrayIndex].transform.position = new Vector3 (originPoint.x + pWidth * col, originPoint.y + pHeight/2* row,
						targets [arrayIndex].transform.position.z);
					arrayIndex++;
				}
				break;
			default:
				break;
		
		}
	}

	Vector3 AlignToTarget(GameObject item)
	{

		foreach (GameObject go in targets) 
		{
			if (Vector2.Distance (item.transform.position, go.transform.position) < pWidth/2)
				return go.transform.position;
		}

		return item.transform.position;
	}

	IEnumerator CameraZoomIn()
	{
		while (mCam.orthographicSize > camMin) 
		{
			mCam.orthographicSize--;
			yield return null;
		}

		foreach (GameObject go in targets)
			go.GetComponent<SpriteRenderer> ().enabled = false;
	}

	IEnumerator CameraZoomOut()
	{
		foreach (GameObject go in targets)
			go.GetComponent<SpriteRenderer> ().enabled = true;
		while (mCam.orthographicSize < camMax) 
		{
			mCam.orthographicSize++;
			yield return null;
		}
	}
}
