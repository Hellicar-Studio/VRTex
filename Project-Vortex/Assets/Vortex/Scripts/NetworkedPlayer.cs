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
    public OVRCameraRig rig;
    private GameObject dummy;

	// Use this for initialization
	void Start () {
        Debug.Log("I'm instantiated! And my color Name is: " + colorName + " And the player is mine: " + photonView.isMine);

        playerGlobal = this.transform;
        playerLocal = GameObject.Find("Anchor" + colorName).transform;

        this.transform.SetParent(playerLocal);

        transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

        dummy = GameObject.Find("Anchor" + colorName + "/DummyPlayer" + colorName);
        dummy.SetActive(false);

        if (photonView.isMine)
        {
            OVRCameraRig gameRig = Instantiate(rig, GameObject.Find("Anchor" + colorName).transform);

            this.transform.SetParent(gameRig.transform.GetChild(0).GetChild(1).transform);

            avatar.SetActive(false);
        } else
        {
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

    private void OnDestroy()
    {
        dummy.SetActive(true);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
