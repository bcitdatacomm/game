using System;
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