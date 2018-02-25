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

                Response res = JsonConvert.DeserializeObject<Response>(responseFromServer);

                Console.Write(responseFromServer);

                reader.Close();
                dataStream.Close();
                response.Close();

                if (res != null)
                {
                    _player = new HighnoonPlayer();
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