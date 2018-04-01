using UnityEngine;
using System.Collections;

public class TranslateCamera : MonoBehaviour {

	// GameObject camera = GetComponent<GameObject>;

	public void MoveCamera(int y)
	{
		gameObject.transform.Translate (0, y, 0);
	}
}