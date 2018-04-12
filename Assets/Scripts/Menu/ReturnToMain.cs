using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReturnToMain : MonoBehaviour {

	public Button returnButton;
	public int sceneIndex;

	// Use this for initialization
	void Start () {


		returnButton.onClick.AddListener( () => {returnFunction();} );
	}

	// Update is called once per frame
	void Update () {

	}

	void returnFunction()
	{
		SceneManager.LoadScene (sceneIndex);
	}

}
