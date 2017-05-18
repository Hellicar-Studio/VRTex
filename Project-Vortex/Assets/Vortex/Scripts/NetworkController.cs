using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.kupio.FlowControl;

public class NetworkController : MonoBehaviour {

    public string _room = "Test_Room";
    public string _ColorName;

	// Use this for initialization
	void Start () {
        bool connected = PhotonNetwork.ConnectUsingSettings("0.1");
        OVRCameraRig[] rigs = FindObjectsOfType<OVRCameraRig>();
        for(int i = 0; i < rigs.Length; i++)
        {
            string name = rigs[i].gameObject.name;
            string colOfObject = name.Substring(name.Length - _ColorName.Length);
            if(colOfObject != _ColorName)
            {
                rigs[i].gameObject.SetActive(false);
            }
        }
	}

    void OnJoinedLobby()
    {
        Debug.Log("joined lobby");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;
        PhotonNetwork.JoinOrCreateRoom(_room, roomOptions, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        PhotonPlayer[] players = PhotonNetwork.playerList;

        for(int i = 0; i < players.Length; i++)
        {
            Debug.Log(players[i].UserId);
            Debug.Log(players[i].IsLocal);
        }

        GameObject player = PhotonNetwork.Instantiate("NetworkedPlayer" + _ColorName, new Vector3(0.0f, 0.0f, 0.0f), new Quaternion(0, 0, 0, 0), 0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
