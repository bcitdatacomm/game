/*------------------------------------------------------------------------------
-- Simple code used to load a scene from the build.
------------------------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {

	public void LoadByIndex(int sceneIndex)
	{
		SceneManager.LoadScene (sceneIndex);
	}
}
