using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.kupio.FlowControl;

public class NetworkedPlayer : Photon.MonoBehaviour {

    public GameObject avatar;
    public GameObject emitter;
    public Transform playerGlobal;
    public Transform playerLocal;

	// Use this for initialization
	void Start () {
        Debug.Log("I'm instantiated!");

        if(photonView.isMine)
        {
            Debug.Log("Player is Mine");
            playerGlobal = this.transform;
            playerLocal = GameObject.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").transform;

            this.transform.SetParent(playerLocal);

            avatar.SetActive(false);
        }
        ParticleFlowController flowControl = emitter.GetComponent<ParticleFlowController>();

        flowControl.flowControlRegion = FindObjectOfType<FlowControlRegion>();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(playerGlobal.position);
            stream.SendNext(playerGlobal.rotation);
            stream.SendNext(playerLocal.localPosition);
            stream.SendNext(playerLocal.localRotation);
        } else
        {
            this.transform.position = (Vector3)stream.ReceiveNext();
            this.transform.rotation = (Quaternion)stream.ReceiveNext();
            avatar.transform.localPosition = (Vector3)stream.ReceiveNext();
            avatar.transform.localRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
