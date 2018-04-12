using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameStats : MonoBehaviour
{
	public Text timerLabel;
	public Text playersLeft;

	private float time;

	// Use this for initialization
	void Start () {

	}

	void Update() {
		time += Time.deltaTime;

		var minutes = time / 60; //Divide the guiTime by sixty to get the minutes.
		var seconds = time % 60;//Use the euclidean division for the seconds.
		var fraction = (time * 100) % 100;

		//update the label value
		timerLabel.text = string.Format ("{0:00} : {1:00} : {2:000}", minutes, seconds, fraction);

		playersLeft.text = string.Format ("Players Left: {0}", GameObject.Find("GameController").GetComponent<GameController>().NumberOfPlayers);
	}
}
