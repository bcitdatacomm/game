/*------------------------------------------------------------------------------
-- PROGRAMMING:	c4981_Game.exe
--
-- FUNCTIONS: 	bool connect()
--              bool login(string name, string password)
--              bool register(string name, string password)
--              bool report_match(string token, string name, int shots_fired, int kills, int deaths, int if_game_won, int time_played)
--
-- DATE:		March 13th, 2018
--
-- DESIGNER:	Mac Craig
--
-- PROGRAMMER:	Mac Craig & Morgan Ariss
--
-- NOTES:
--	This class operates as the manager for conections between the UNITY application
--  and the highnoon server. It contains functions for login and registration to
--  allowing individual players to be tracked.
--
------------------------------------------------------------------------------*/

﻿using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace HighnoonTools
{
    class HighnoonManager
    {

        [System.Serializable]
        public class Response
        {
            public string Success { get; set; }
            public string Token { get; set; }
        }

		public class UserResponse
		{
			public string name { get; set; }
			public bool admin { get; set; }
			public int shots_fired { get; set; }
			public int kills { get; set; }
			public int deaths { get; set; }
			public int games_played { get; set; }
			public int games_won { get; set; }
			public int time_played { get; set; }
		}

        private static string UserAgent = "highnoonTools C#";
        private string _url;

        public HighnoonManager(string url)
        {
            _url = url;
        }

        string _token;
        HighnoonPlayer _player;

        public string Token { get { return _token; } }
        public HighnoonPlayer Player { get { return _player; } }

        /*----------------------------------------------------------------------
        -- FUNCTION:	Connect()
        --
        -- DATE:		March 13th, 2018
        --
        -- DESIGNER:	Morgan Ariss
        --
        -- PROGRAMMING:	Morgan Ariss
        --
        -- INTERFACE:	Connect()
        --
        -- ARGUMENTS:
        --
        -- RETURNS:
        --
        -- NOTES:
        --	This function forms a conenction with the highnoon server allowing
        --  further requests to be made. It returns a token to the client so
        --  so that they are able to complete further requests.
        --
        ----------------------------------------------------------------------*/
        public bool Connect()
        {
            try
            {
                WebRequest request = WebRequest.Create(_url + "/api/connect");
                //request.Credentials = CredentialCache.DefaultCredentials;

                ((HttpWebRequest)request).UserAgent = UserAgent;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = reader.ReadToEnd();

                Response res = JsonConvert.DeserializeObject<Response>(responseFromServer);

                reader.Close();
                dataStream.Close();
                response.Close();

                if (res.Success == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (JsonException) {
                Console.Write("Error parsing JSON");
                return false;
            }
            catch(Exception e) {
                Console.Write(e);
                return false;
            }
        }
        /*----------------------------------------------------------------------
        -- FUNCTION:	Login()
        --
        -- DATE:		March 13th, 2018
        --
        -- DESIGNER:	Mac Craig
        --
        -- PROGRAMMING:	Mac Craig & Morgan Ariss
        --
        -- INTERFACE:	Login()
        --
        -- ARGUMENTS:   string name; user's name
        --              string password; user's password
        --
        -- RETURNS:     bool; success of the registration
        --
        -- NOTES:
        --	This function attemps to login a client to an account with the highnoon
        --  server. It needs a token for the request to be handled, otherwise it
        --  will be ignored. it returns the success of the request.
        --
        ----------------------------------------------------------------------*/
        public bool Login(string name, string password)
        {
            try
            {
                WebRequest request = WebRequest.Create(_url + "/api/login");
                //request.Credentials = CredentialCache.DefaultCredentials;

                ((HttpWebRequest)request).UserAgent = UserAgent;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                string postData = "name=" + name + "&password=" + password;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);

                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = reader.ReadToEnd();

                Response res = JsonConvert.DeserializeObject<Response>(responseFromServer);

                reader.Close();
                dataStream.Close();
                response.Close();

                if (res.Success == "true")
                {
                    _token = res.Token;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (JsonException)
            {
                Console.Write("Error parsing JSON");
                return false;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return false;
            }

        }

        /*----------------------------------------------------------------------
        -- FUNCTION:	Register()
        --
        -- DATE:		March 13th, 2018
        --
        -- DESIGNER:	Mac Craig
        --
        -- PROGRAMMING:	Mac Craig
        --
        -- INTERFACE:	Register()
        --
        -- ARGUMENTS:   string name; user's name
        --              string password; user's password
        --
        -- RETURNS:     bool; success of the registration
        --
        -- NOTES:
        --	This function attemps to register a new account with the highnoon
        --  server. It needs a token for the request to be handled, otherwise it
        --  will be ignored. It returns the success or failure of the request.
        --
        ----------------------------------------------------------------------*/
        public bool Register(string name, string password)
        {
            try
            {
                WebRequest request = WebRequest.Create(_url + "/api/register");
                // request.Credentials = CredentialCache.DefaultCredentials;

                ((HttpWebRequest)request).UserAgent = UserAgent;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                string postData = "name=" + name + "&password=" + password;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);

                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = reader.ReadToEnd();

                Response res = JsonConvert.DeserializeObject<Response>(responseFromServer);

                reader.Close();
                dataStream.Close();
                response.Close();

                if (res.Success == "true")
                {
                    _token = res.Token;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (JsonException)
            {
                Console.Write("Error parsing JSON");
                return false;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return false;
            }
        }

        // Not in use currently
        public bool Report_Match(string token, string name, int shots_fired, int kills, int deaths, int if_game_won, int time_played)
        {
            try
            {
                WebRequest request = WebRequest.Create(_url + "/api/report_match");
                // request.Credentials = CredentialCache.DefaultCredentials;

                ((HttpWebRequest)request).UserAgent = UserAgent;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                string postData =
                      "token=" + token
                    + "&name=" + name
                    + "&shots_fired=" + shots_fired
                    + "&kills=" + kills
                    + "&deaths=" + deaths
                    + "&if_game_won=" + if_game_won
                    + "&time_played=" + time_played;

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);

                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = reader.ReadToEnd();

                Response res = JsonConvert.DeserializeObject<Response>(responseFromServer);

                Console.Write(responseFromServer);

                reader.Close();
                dataStream.Close();
                response.Close();

                if (res.Success == "true")
                {
                    _token = res.Token;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (JsonException)
            {
                Console.Write("Error parsing JSON");
                return false;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return false;
            }
        }

        // Not in use currently
        public bool User(string token, string name)
        {
            try
            {
                WebRequest request = WebRequest.Create(_url + "/api/user?token=" + token + "&name=" + name);
                // request.Credentials = CredentialCache.DefaultCredentials;

                ((HttpWebRequest)request).UserAgent = UserAgent;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = reader.ReadToEnd();

				Console.WriteLine(responseFromServer);

                UserResponse res = JsonConvert.DeserializeObject<UserResponse>(responseFromServer);

                Console.Write(responseFromServer);

                reader.Close();
                dataStream.Close();
                response.Close();

                if (res != null)
                {
                    _player = new HighnoonPlayer();

					Console.WriteLine(res);

					_player.Name = res.name;
					_player.Admin = res.admin;
					_player.Shots_Fired = res.shots_fired;
					_player.Kills = res.kills;
					_player.Deaths = res.deaths;
					_player.Games_Played = res.games_played;
					_player.Games_Won = res.games_won;
					_player.Time_Played = res.time_played;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (JsonException)
            {
                Console.Write("Error parsing JSON");
                return false;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return false;
            }
        }
    }
}
