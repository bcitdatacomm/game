using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HighnoonTools
{
	public class HighnoonRegister : MonoBehaviour {

		public Button loginButton;

		HighnoonManager api;

		public InputField usernameText;
		public InputField passwordText;

		string username = "";
		string password = "";

		// Use this for initialization
		void Start ()
		{
			api = new HighnoonManager("http://159.65.109.194/");

			if(api.Connect())
			{
				print("Connected to API.");
			}
			else
			{
				print("Error: Connection to API failed.");
			}

			Debug.Log(usernameText.text);
			Debug.Log(passwordText.text);

			Debug.Log(usernameText.text.ToString());
			Debug.Log(passwordText.text.ToString());

			loginButton.onClick.AddListener( () => {loginFunction(username, password);} );
		}

		// Update is called once per frame
		void Update ()
		{
			if(usernameText != null)
			{
				username = usernameText.text.ToString();
				password = passwordText.text.ToString();
			}
		}

		void loginFunction(string name, string pass)
		{
			// your code goes here
			print(name + ", " + pass);

			if(api.Register(name, pass))
			{
				print("Registration Successful");
			}
			else
			{
				print("Error: Name in use.");
			}
		}

	}
}
