using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour 
{


	public Camera mCam;
	float camMin, camMax;
	float screenWidth, screenHeight;

	[SerializeField]
	private int level;


	public GameObject[] puzzlePieces;

	public GameObject selectedObject;

	public GameObject[] targets;

	public AudioClip thatsWrong;

	private Material[] materials;

	private AudioClip[] clips;

	private AudioSource audioComponent;

	private List<PuzzlePiece> pieces = new List<PuzzlePiece>();

	private bool isZoomedIn;
	private bool isAlmostDone;

	private string pathForThisLevelSounds;
	private string pathForThisLevelImages;
	private string pathForThisLevelFullAudio;
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

		screenWidth = 18.5f * 2;
		screenHeight = 30;

		isAlmostDone = false;
		isZoomedIn = false;

		selectedObject = null;
		audioComponent = GetComponent<AudioSource> ();
		Init ();
	}

	void FixedUpdate()
	{
		if (Input.GetMouseButtonDown (0) && !isAlmostDone) 
		{
			if (selectedObject != null) 
			{
				selectedObject.transform.position = AlignToTarget (selectedObject);
				selectedObject = null;
			}
			else 
			{
				RaycastHit hit;
				if (Physics.Raycast (mCam.ScreenPointToRay (Input.mousePosition), out hit))
				{
					selectedObject = hit.collider.gameObject;
					//selectedObject.GetComponent<AudioSource> ().Play ();
					foreach (PuzzlePiece p in pieces)
						if (p.piece.Equals (selectedObject))
						{
							audioComponent.clip = p.audio;
							audioComponent.Play();
						}
				}

			
			}
		}

		if (selectedObject != null) 
			selectedObject.transform.position = new Vector3 (mCam.ScreenToWorldPoint (Input.mousePosition).x, mCam.ScreenToWorldPoint (Input.mousePosition).y, selectedObject.transform.position.z);

		if (selectedObject != null && Mathf.Abs (selectedObject.transform.position.x) > screenWidth / 2 )
		{
			selectedObject.transform.position = new Vector3 (0, selectedObject.transform.position.y, selectedObject.transform.position.z); 
			selectedObject = null;
		}

		if (selectedObject != null && Mathf.Abs (selectedObject.transform.position.y) > screenHeight / 2)
		{
			selectedObject.transform.position = new Vector3 (selectedObject.transform.position.x, 0, selectedObject.transform.position.z); 
			selectedObject = null;
		}
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

		if (Input.GetKeyUp (KeyCode.K))
			Debug.Log(CheckForCorrect ());
	}

	void Init()
	{
		selectedObject = null;
		pathForThisLevelImages = "Pictures/L" + level+"/";
		pathForThisLevelSounds = "Sounds/L" + level + "/";
		pathForThisLevelFullAudio = "Audio/L" + level + "/";

		//Set all active
		foreach (GameObject go in targets)
			go.SetActive (true);

		foreach (GameObject go in puzzlePieces)
			go.SetActive (true);

		materials = Resources.LoadAll<Material> (pathForMaterials);
		Texture[] textures = Resources.LoadAll<Texture> (pathForThisLevelImages);
		clips = Resources.LoadAll<AudioClip> (pathForThisLevelSounds);

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
		
		//Add pieces to list of PuzzlePieces game objects
		for (int i = 0; i < numPieces; i++)
			//puzzlePieces [i].GetComponent<AudioSource> ().clip = clips [i];
			pieces.Add (new PuzzlePiece (puzzlePieces [i], clips[i]));

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

	public void TryToAdvance()
	{
		if (CheckForCorrect ())
		{
			audioComponent.clip = Resources.LoadAll<AudioClip>(pathForThisLevelFullAudio)[0];
			audioComponent.Play ();
			isAlmostDone = true;
		}
		else
		{
			audioComponent.clip = thatsWrong;
			audioComponent.Play ();
		}
		
			
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

	public void CameraZoom()
	{
		if (isZoomedIn)
			StartCoroutine (CameraZoomOut ());
		else
			StartCoroutine (CameraZoomIn ());
	}

	IEnumerator waitUntilPoemEndThenAdvance()
	{
		yield return null;
	}

	IEnumerator CameraZoomIn()
	{
		while (mCam.orthographicSize > camMin) 
		{
			mCam.orthographicSize--;
			yield return null;
		}

		isZoomedIn = true;
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

		isZoomedIn = false;
	}
}


public class PuzzlePiece
{
	public GameObject piece;
	public AudioClip audio;

	public PuzzlePiece(GameObject pp, AudioClip clip)
	{
		piece = pp;
		audio = clip;
	}

	string toString()
	{
		return piece.name + " with clip of " + audio.name;
	}
}
