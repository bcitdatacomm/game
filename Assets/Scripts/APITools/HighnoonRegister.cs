/*------------------------------------------------------------------------------
-- PROGRAM:		c4981_Game.exe
--
-- FILE:		HighnoonRegister.cs
--
-- FUNCTIONS: 	void Start()
--				void Update()
--				void registerFunction(string name, string pass)
--
-- DATE:		March 15th, 2018
--
-- DESIGNER:	Morgan Ariss & Mac Craig
--
-- PROGRAMMER:	Morgan Ariss
--
-- NOTES:
--	This function handles the connection to the server from UNITY and allows the
--	user to create a new account on the Highnoon web server. A check is made to
--	see if the server is available, then a connection is made. This class makes
--	use of the UNITY start() and update() functions.
--
------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HighnoonTools
{
	public class HighnoonRegister : MonoBehaviour {

		public Button registerButton;

		HighnoonManager api;

		public InputField usernameText;
		public InputField passwordText;

		string username = "";
		string password = "";

		/*----------------------------------------------------------------------
		-- FUNCTION:	Start()
		--
		-- DATE:		March 15th, 2018
		--
		-- DESIGNER:	Morgan Ariss
		--
		-- PROGRAMMING:	Morgan Ariss
		--
		-- INTERFACE:	Start()
		--
		-- ARGUMENTS:
		--
		-- RETURNS:
		--
		-- NOTES:
		--	This function performs the intitial check to see if the server is
		--	available. If yes, it tries to connect. It listens to the login
		--	button, and if it is clicked it runs loginFunction() to form a
		--	connection with the highnoon server. It runs immediately.
		--
		----------------------------------------------------------------------*/
		void Start()
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

			registerButton.onClick.AddListener( () => {registerFunction(username, password);} );
		}

		/*----------------------------------------------------------------------
		-- FUNCTION:	Update()
		--
		-- DATE:		March 15th, 2018
		--
		-- DESIGNER:	Morgan Ariss
		--
		-- PROGRAMMING:	Morgan Ariss
		--
		-- INTERFACE:	Update()
		--
		-- ARGUMENTS:
		--
		-- RETURNS:
		--
		-- NOTES:
		--	This function runs one every second; all it does is update the
		--	username and password with the input given by the user.
		--
		----------------------------------------------------------------------*/
		void Update ()
		{
			if(usernameText != null)
			{
				username = usernameText.text.ToString();
				password = passwordText.text.ToString();
			}
		}

		/*----------------------------------------------------------------------
		-- FUNCTION:	registerFunction()
		--
		-- DATE:		March 15th, 2018
		--
		-- DESIGNER:	Morgan Ariss
		--
		-- PROGRAMMING:	Morgan Ariss
		--
		-- INTERFACE:	registerFunction(string name, string pass)
		--
		-- ARGUMENTS:
		--
		-- RETURNS:
		--
		-- NOTES:
		--	This function is responsible for passing the users entered information
		--	and sending it to the server to create an account. The server will
		--	send back a boolean to indicate success of failure and the function
		--	will respond accordingly.
		--
		----------------------------------------------------------------------*/
		void registerFunction(string name, string pass)
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
