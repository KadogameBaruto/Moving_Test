using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Robo : MonoBehaviourPunCallbacks
{
    // 位置情報(他プレイヤーの位置情報として)
    private Vector3 receivePosition = Vector3.zero;
    // 画面の左下の座標
    private Vector3 screen_LeftBottom = Vector3.zero;
    // 画面の右上の座標
    private Vector3 screen_RightTop = Vector3.zero;

    // 移動スピード
    private float speed = 0.1f;

    // 移動フラグ
    public bool isUp = false;
    public bool isDown = false;
    public bool isRight = false;
    public bool isLeft = false;

    // Start is called before the first frame update
    void Start()
    {
        // 画面の左下の座標を取得 
        screen_LeftBottom = Camera.main.ScreenToWorldPoint(Vector3.zero);
        // 画面の右上の座標を取得 
        screen_RightTop = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    }

    // Update is called once per frame
    // void Update()//★Unityから実行すると進む距離が異なるため一旦FixedUpdateとした。リリース時にはupdateに戻す
    void FixedUpdate()
    {
        Vector3 oldPos = transform.position;//範囲外の場合移動前に戻すために記憶
        if (isUp)
        {
            transform.position += Vector3.up * speed;
        }
        if (isDown)
        {
            transform.position -= Vector3.up * speed;
        }
        if (isRight)
        {
            transform.position += Vector3.right * speed;
        }
        if (isLeft)
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
}
