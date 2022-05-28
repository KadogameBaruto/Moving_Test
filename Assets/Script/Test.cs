using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Test : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    

    //ルームに入室前に呼び出される
    public override void OnConnectedToMaster()
    {
        // "room"という名前のルームに参加する（ルームが無ければ作成してから参加する）
        PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions(), TypedLobby.Default);
        // PhotonNetwork.JoinLobby();
    }

    //ルームに入室後に呼び出される
    public override void OnJoinedRoom()
    {
        //プレイヤーを生成
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 属性の設定
[RuntimeInitializeOnLoadMethod]
static void OnRuntimeMethodLoad() {
Debug.Log("After Scene is loaded and game is running");
// スクリーンサイズの指定
Screen.SetResolution(1024, 768, false);
}
}
