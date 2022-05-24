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

    // 位置情報(他プレイヤーの位置情報として)
    private Vector3 receivePosition = Vector3.zero;
    // 画面の左下の座標
    private Vector3 screen_LeftBottom = Vector3.zero;
    // 画面の右上の座標
    private Vector3 screen_RightTop = Vector3.zero;

    // 移動スピード
    private float speed = 0.075f;

    void Start()
    {
        if (photonView.IsMine)
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        // 画面の左下の座標を取得 
        screen_LeftBottom = Camera.main.ScreenToWorldPoint(Vector3.zero);
        // 画面の右上の座標を取得 
        screen_RightTop = Camera.main.ScreenToWorldPoint(
            new Vector3(Screen.width, Screen.height, 0));

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
            Vector3 oldPos = transform.position;
            if (Input.GetKey("up"))
            {
                transform.position += Vector3.up * speed;
            }
            if (Input.GetKey("down"))
            {
                transform.position -= Vector3.up * speed;
            }
            if (Input.GetKey("right"))
            {
                transform.position += Vector3.right * speed;
            }
            if (Input.GetKey ("left"))
            {
                transform.position -= Vector3.right * speed;
            }
            if (Input.GetKeyDown (KeyCode.A))
            {
                Debug.Log(transform.position.x +"\r\n"+
                "screen_LeftBottom"+screen_LeftBottom +"\r\n" +
                "screen_RightTop"+screen_RightTop +"\r\n" );
            }

            //左側制限
            if(transform.position.x < screen_LeftBottom.x)
            {
                oldPos.x = screen_LeftBottom.x;
                transform.position = oldPos;
            }
            //下側制限
            if(transform.position.y < screen_LeftBottom.y)
            {
                oldPos.y = screen_LeftBottom.y;
                transform.position = oldPos;
            }
            //右側制限
            if(screen_RightTop.x < transform.position.x)
            {
                oldPos.x = screen_RightTop.x;
                transform.position = oldPos;
            }
            //上側制限
            if(screen_RightTop.y < transform.position.y)
            {
                oldPos.y = screen_RightTop.y;
                transform.position = oldPos;
            }
        }
        else
        {
            //自分以外のプレイヤーの補正
            transform.position = Vector3.Lerp(transform.position, receivePosition, Time.deltaTime );
            //transform.rotation = Quaternion.Lerp(transform.rotation, receiveRotation, Time.deltaTime * 10);
            //rigidbody2D.velocity = Vector2.Lerp(rigidbody2D.velocity, receiveVelocity, Time.deltaTime * 10);
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

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     Debug.Log("TekiTekiTekiTekiTeki");
    //     if (other.gameObject.CompareTag("Teki"))
    //     {
    //         other.gameObject.SetActive(false);
    //     }
    // }
   
}
