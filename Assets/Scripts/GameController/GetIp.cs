using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetIp : MonoBehaviour
{

	public InputField MainInputField;
	public string Ip { get; set; }

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		Ip = MainInputField.text;
		Debug.Log (Ip);
	}
}
