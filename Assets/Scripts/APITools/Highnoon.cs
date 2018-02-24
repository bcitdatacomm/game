using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighnoonTools
{
    public class Highnoon : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            HighnoonManager api = new HighnoonManager("http://localhost:8888");
            if(api.Connect()) {
                Debug.Log("Connected to API.");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}