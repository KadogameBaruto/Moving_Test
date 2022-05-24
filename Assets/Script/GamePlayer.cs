using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;

public class GamePlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    // シングルトンの生成
    public static GamePlayer Instance;

    private Vector3 receivePosition = Vector3.zero;

    private float speed = 0.1f;

    void Start()
    {
        if (photonView.IsMine)
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

    }
    void OnGUI()
    {
        if (photonView.IsMine)
        {
            //ログインの状態を画面上に出力
            GUILayout.Label(PhotonNetwork.NetworkClientState.ToString() + "\r\n" +
            PhotonNetwork.LocalPlayer.ActorNumber + "\r\n" +
            PhotonNetwork.LocalPlayer.IsMasterClient + "\r\n" +
            transform.position
            );
        }
    }

    // void Update()//★Unityから実行すると進む距離が異なるため一旦FixedUpdateとした。リリース時にはupdateに戻す
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKey("up")) {
                transform.position += transform.up * speed;
            }
            if (Input.GetKey("down")) {
                transform.position -= transform.up * speed;
            }
            if (Input.GetKey("right")) {
                transform.position += transform.right * speed;
            }
            if (Input.GetKey ("left")) {
                transform.position -= transform.right * speed;
            }
        }
        else
        {
        // //自分以外のプレイヤーの補正
            // transform.position = receivePosition;
            transform.position = Vector3.Lerp(transform.position, receivePosition, Time.deltaTime );
        //     transform.rotation = Quaternion.Lerp(transform.rotation, receiveRotation, Time.deltaTime * 10);
        //     rigidbody2D.velocity = Vector2.Lerp(rigidbody2D.velocity, receiveVelocity, Time.deltaTime * 10);
        }
        

    }

    //同期情報のやり取り
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting)
        {
            // Transformの値をストリームに書き込んで送信する
            stream.SendNext(transform.position);
            // stream.SendNext(transform.localRotation);
            // stream.SendNext(transform.localScale);
        } else {
            // 受信したストリームを読み込んでTransformの値を更新する
            receivePosition = (Vector3)stream.ReceiveNext();
            // transform.localRotation = (Quaternion)stream.ReceiveNext();
            // transform.localScale = (Vector3)stream.ReceiveNext();
        }
    }



    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TekiTekiTekiTekiTeki");
        if (other.gameObject.CompareTag("Teki"))
        {
            other.gameObject.SetActive(false);
        }
    }
   
}
