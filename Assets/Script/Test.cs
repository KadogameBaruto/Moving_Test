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
        //if(PhotonNetwork.LocalPlayer.IsMasterClient)
        //{
        //    System.Random r = new System.Random();
        //    Vector3 spawnPosition = new Vector3(r.Next(0, 400), r.Next(0, 400), 0); //生成位置

        //    GameObject image = PhotonNetwork.Instantiate("MyImage", spawnPosition, Quaternion.identity, 0);
        //}

        // if (PhotonNetwork.IsMasterClient)
        // {
        //     var hashTable = new ExitGames.Client.Photon.Hashtable();
        //     hashTable["TurnPlayerID"] = PhotonNetwork.LocalPlayer.ActorNumber;
        //     PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable);
        // }

        //プレイヤーを生成
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
