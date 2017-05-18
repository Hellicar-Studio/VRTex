using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.kupio.FlowControl;

public class NetworkedPlayer : Photon.MonoBehaviour {

    public GameObject avatar;
    public AudioEmitter emitter;
    public Transform playerGlobal;
    public Transform playerLocal;
    public float lerpSpeed;
    public string colorName;

	// Use this for initialization
	void Start () {
        Debug.Log("I'm instantiated!");

        if (photonView.isMine)
        {
            Debug.Log("Player is Mine");
            playerGlobal = this.transform;
            playerLocal = GameObject.Find("OVRCameraRig" + colorName + "/TrackingSpace/CenterEyeAnchor").transform;

            this.transform.SetParent(playerLocal);

            transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

            avatar.SetActive(false);
        } else
        {
            Debug.Log("Player is not Mine!");
            emitter = GetComponentInChildren<AudioEmitter>();
            emitter.networked = true;
        }

        ParticleFlowController flowControl = emitter.GetComponent<ParticleFlowController>();
        flowControl.flowControlRegion = FindObjectOfType<FlowControlRegion>();

    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            Vector3 pos = playerGlobal.position;
            //Debug.Log("Sending Global Position: " + pos);
            stream.SendNext(pos);
            Quaternion rot = playerGlobal.rotation;
            //Debug.Log("Sending Global Rotation: " + rot);
            stream.SendNext(rot);
            stream.SendNext(emitter.loudness);
        } else
        {
            Vector3 pos = (Vector3)stream.ReceiveNext();
            //Debug.Log("Recieving Global Position: " + pos);
            this.transform.position = Vector3.Lerp(transform.position, pos, lerpSpeed);
            Quaternion rot = (Quaternion)stream.ReceiveNext();
            //Debug.Log("Recieving Global Rotation: " + rot);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, rot, lerpSpeed); ;
            emitter.setLoudness((float)stream.ReceiveNext());
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
