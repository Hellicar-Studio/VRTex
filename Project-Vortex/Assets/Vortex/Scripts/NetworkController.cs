using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.kupio.FlowControl;

public class NetworkController : MonoBehaviour {

    public string _room = "Test_Room";

	// Use this for initialization
	void Start () {
        bool connected = PhotonNetwork.ConnectUsingSettings("0.1");
	}

    void OnJoinedLobby()
    {
        Debug.Log("joined lobby");
        RoomOptions roomOptions = new RoomOptions();
        PhotonNetwork.JoinOrCreateRoom(_room, roomOptions, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate("NetworkedPlayer", new Vector3(0.0f, 0.0f, 0.0f), new Quaternion(0, 0, 0, 0), 0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
