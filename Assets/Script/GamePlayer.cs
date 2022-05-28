using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GamePlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    // 共有用プレイヤー位置情報(他プレイヤーの位置情報として)
    private Vector3 receivePosition = Vector3.zero;
    // 画面の左下の座標
    private Vector3 screen_LeftBottom = Vector3.zero;
    // 画面の右上の座標
    private Vector3 screen_RightTop = Vector3.zero;

    //ロボ情報
    [SerializeField]
    private Robo RoboPrefab;//プレハブ
    private Robo Robo;      //ロボインスタンス
    private Vector3 receiveRoboPosition = Vector3.zero;//共有用ロボ位置情報

    // 移動スピード
    private const float SpeedOnGround = 0.015f;
    private const float SpeedInShip = 0.05f;
    private float speed;

    // 地上サイズ倍率
    private Vector3 ScaleOnGround = new Vector3(0.5f,0.5f,0.5f);
    private Vector3 ScaleInShip = new Vector3(1.0f,1.0f,1.0f);

    // 宇宙船
    /*宇宙船の当たり判定をちゃんとする(できればスクリプトから動的に指定したい)*/
    private bool IsGetOffShip = false;
    private bool IsRideOnShip = false;

    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    void Start()
    {
        speed = SpeedInShip;

        // 画面の左下の座標を取得 
        screen_LeftBottom = Camera.main.ScreenToWorldPoint(Vector3.zero);
        // 画面の右上の座標を取得 
        screen_RightTop = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        
        // ロボを生成
        Robo = (Robo)Instantiate(this.RoboPrefab, GameObject.Find("Canvas").GetComponent<Transform>());
    }

    // void Update()//★Unityから実行すると進む距離が異なるため一旦FixedUpdateとした。リリース時にはupdateに戻す
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Vector3 oldPos = transform.position;
            if (Input.GetKey("up") || Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.up * speed;
            }
            if (Input.GetKey("down") || Input.GetKey(KeyCode.S))
            {
                transform.position -= Vector3.up * speed;
            }
            if (Input.GetKey("right") || Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * speed;
            }
            if (Input.GetKey ("left") || Input.GetKey(KeyCode.A))
            {
                transform.position -= Vector3.right * speed;
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
            //自分以外のプレイヤー情報補正
            transform.position = Vector3.Lerp(transform.position, receivePosition, Time.deltaTime );
            //transform.rotation = Quaternion.Lerp(transform.rotation, receiveRotation, Time.deltaTime * 10);
            //rigidbody2D.velocity = Vector2.Lerp(rigidbody2D.velocity, receiveVelocity, Time.deltaTime * 10);
        }

        // ロボの位置
        if (!photonView.IsMine)
        {
            //自分以外のプレイヤーのロボ情報補正
            Robo.transform.position = Vector3.Lerp(Robo.transform.position, receiveRoboPosition, Time.deltaTime );
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
            "Me:"+transform.position+ "\r\n" +
            "Robo:"+Robo.transform.position
            );
        }
    }

    //同期情報のやり取り
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Transformの値をストリームに書き込んで送信する
            stream.SendNext(transform.position);
            // stream.SendNext(transform.localRotation);
            // stream.SendNext(transform.localScale);

            stream.SendNext(Robo.transform.position);
        } else {
            // 受信したストリームを読み込んでTransformの値を更新する
            receivePosition = (Vector3)stream.ReceiveNext();
            // transform.localRotation = (Quaternion)stream.ReceiveNext();
            // transform.localScale = (Vector3)stream.ReceiveNext();

            receiveRoboPosition = (Vector3)stream.ReceiveNext();
        }
    }
   
    // パネルへ侵入
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        switch (other.gameObject.name)
        {
            case "UpPanel":
                Robo.isUp = true;
            break;

            case "DownPanel":
                Robo.isDown = true;
            break;

            case "RightPanel":
                Robo.isRight = true;
            break;

            case "LeftPanel":
                Robo.isLeft = true;
            break;

            case "Gate":
            if(IsRideOnShip == false)
            {
                GetOffSpaceship();
            }
            break;

            case "UFOGate":
            if(IsGetOffShip == false)
            {
                RideOnSpaceship();
            }
            break;
            
            default:
            break;
        }
    }

    // 宇宙船から降りる
    void GetOffSpaceship()
    {
        // 宇宙船の座標へ移動
        IsGetOffShip = true;
        this.transform.position = Robo.transform.position;
        // 地上サイズの大きさとなる
        this.transform.localScale = ScaleOnGround;
        // 地上移動スピードとなる
        this.speed = SpeedOnGround;
    }

    // 宇宙船に乗る
    void RideOnSpaceship()
    {
        // 宇宙船の座標へ移動
        IsRideOnShip = true;
        this.transform.position = GameObject.Find("Gate").GetComponent<Transform>().position ;
        // 地上サイズの大きさとなる
        this.transform.localScale = ScaleInShip;
        // 地上移動スピードとなる
        this.speed = SpeedInShip;
    }

    // パネルから抜けた
    void OnTriggerExit2D(Collider2D other)
    {
        switch (other.gameObject.name)
        {
            case "UpPanel":
                Robo.isUp = false;
            break;

            case "DownPanel":
                Robo.isDown = false;
            break;

            case "RightPanel":
                Robo.isRight = false;
            break;

            case "LeftPanel":
                Robo.isLeft = false;
            break;

            case "Gate":
                IsRideOnShip = false;

            break;

            case "UFOGate":
                IsGetOffShip = false;

            break;
            
            default:
            break;
        }
    }
}
