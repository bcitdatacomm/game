using System;
using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;



public class connectionData
{
	public EndPoint ep { get; set; }
	public byte[] buffer { get; set; }
	public byte id { get; set; }
	
	public float x { get; set; }
	public float z { get; set; }
	public float r { get; set; }

	public int h { get; set; }
}
