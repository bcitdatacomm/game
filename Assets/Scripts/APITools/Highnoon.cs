using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighnoonTools
{
    public class Highnoon : MonoBehaviour
    {
        void Start()
        {
            HighnoonManager api = new HighnoonManager("http://localhost:8888");
            if (api.Connect())
            {
                Debug.Log("Connected to API.");
            } else {
                Debug.Log("Error: Connection to API failed.");
            }
        }
    }
}