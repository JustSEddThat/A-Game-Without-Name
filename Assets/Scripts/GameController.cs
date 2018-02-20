using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour 
{


	public Camera mCam;
	float camMin, camMax;

	[SerializeField]
	public int level;

	public Material[] materials;

	public GameObject selectedObject;

	private GameObject[] targets;


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
		if (level > 4)
			pathForThisLevelImages = "Pictures/6Piece/L" + level+ "/";
		else
			pathForThisLevelImages = "Pictures/4Piece/L" + level+"/";
		
		camMin = 5;
		camMax = 15;
		selectedObject = null;
		targets = GameObject.FindGameObjectsWithTag("Target");

		if (numPieces < targets.Length)
			for (int i = 0; i < 3; i++)
			{
				Destroy (targets [targets.Length - 1]);
				Debug.Log ("one");
			}
		
		cols = numPieces / rows;
		pWidth = width / cols;
		pHeight = height / 2;
		originPoint = transform.position;
		SpawnTargets ();

		materials = Resources.LoadAll<Material> (pathForMaterials);
		Texture[] textures = Resources.LoadAll<Texture> ("Pictures/6Piece/Test2/");

		for (int i = 0; i < numPieces; i++)
			materials [i].mainTexture = textures [i];

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
