using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace HighnoonTools
{
	public class HighnoonLogin : MonoBehaviour {

		public Button loginButton;

		HighnoonManager api;

		public InputField usernameText;
		public InputField passwordText;

		public int sceneIndex;

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

			if(api.Login(name, pass))
			{
				print("Login Successful");

				print (api.Token);

				api.User(api.Token, name);


				print (api.Player.Name);
				SceneManager.LoadScene (sceneIndex);
			}
			else
			{
				print("Login Failed");
			}
		}

	}
}
