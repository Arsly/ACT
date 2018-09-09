using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _02_UnityChanControl : MonoBehaviour {

    //宣告角色控制器
    private CharacterController UnityChan;
    //指定角色
    public GameObject sos;
    //宣告指示路徑控制器
    private Vector3 moveDirection = Vector3.zero;
    //宣告開始遊戲計時器
    private float StartGameTimer;

    //宣告主要攝影機
    public GameObject MainCamera;
    //宣告UnityChan的所在位置
    private Vector3 UnityChanPosition;
    //宣告攝影機的跟隨向量
    private Vector3 CameraFollowVector;

    //宣告開始遊戲布林值
    private bool StartGameBool;
    //宣告結束遊戲布林值
    private bool EndGameBool;

    //宣告跑步速度
    public float RunSpeed;
    //宣告跳躍速度
    public float JumpSpeed;
    //宣告角色承受的重力值
    public float Gravity;

    //宣告[是否向左移動]布林值
    private bool LeftMoveBool;
    //宣告[是否向右移動]布林值
    private bool RightMoveBool;
    //宣告[是否跳躍]布林值
    private bool JumpBool;
    //宣告水平移動用計時器
    private float HorizontalMove_Timer;

    //宣告遊戲關卡陣列
    public GameObject[] MissionArea;
    //宣告終點區域遊戲物件
    public GameObject EndArea;
    //宣告[玩家是否已進入新關卡]的布林值
    private bool PlayerEnterMissionBool;
    //宣告生成關卡的當前索引值
    private int MissionAreaIndex;
    //宣告生成關卡的最大上限值
    private int MissionAreaMax;

    // 初始化設定
    void Start () {
        //定義角色控制器 = UnityChan自身角色的角色控制器
        UnityChan = this.GetComponent<CharacterController>();
        //玩家角色的跑步動畫布林值預設為false
        this.GetComponent<_01_AnimControl>().RunBool = false;
        //玩家角色的往左跑步動畫布林值預設為false
        this.GetComponent<_01_AnimControl>().RunLeftBool = false;
        //玩家角色的往右跑步動畫布林值預設為false
        this.GetComponent<_01_AnimControl>().RunRightBool = false;
        //玩家角色的跳躍動畫布林值預設為false
        this.GetComponent<_01_AnimControl>().JumpBool = false;
        //玩家角色的勝利動畫布林值預設為false
        this.GetComponent<_01_AnimControl>().WinBool = false;
        //玩家角色的敗北動畫布林值預設為false
        this.GetComponent<_01_AnimControl>().LoseBool = false;




        //攝影機位置回歸預設地點&角度
        MainCamera.GetComponent<Transform>().position = new Vector3(-2.1f, 23.2f, 35.1f);
        MainCamera.GetComponent<Transform>().rotation = Quaternion.Euler(15, 180, 0);

        //開始遊戲布林值預設為false
        StartGameBool = false;
        //結束遊戲布林值預設為false
        EndGameBool = false;
        //開始遊戲計時器預設為0秒
        StartGameTimer = 0;

        //宣告初始關卡1隨機值。骰關卡種類：從關卡A~關卡C 隨機其1
        int RandomMissionInitl = Random.Range(0, 3);
        //生成隨機初始關卡1：設定生成關卡位置(-2.1f, 12.4f, -30.7f)，旋轉角度為(-89.98f,0,0)
        Instantiate(MissionArea[RandomMissionInitl], new Vector3(-2.1f, 12.4f, -30.7f), Quaternion.Euler(-89.98f,0,0));
        //宣告初始關卡2隨機值。骰關卡種類：從關卡A~關卡C 隨機其1
        int RandomMissionInit2 = Random.Range(0, 3);
        //生成隨機初始關卡2：設定生成關卡位置，旋轉角度為初始值
        Instantiate(MissionArea[RandomMissionInit2], new Vector3(-2.1f, 12.4f, -66.7f), Quaternion.Euler(-89.98f, 0, 0));

        //[玩家是否已進入新關卡]布林值預設為false
        PlayerEnterMissionBool = false;
        //生成關卡的當前索引值預設為2 (因為已生成隨機初始關卡1跟2，所以索引值是 0 + 2 = 2)
        MissionAreaIndex = 2;
        //生成關卡的最大上限值預設為6 (搭配起點與終點，合計共8個關卡區域)
        MissionAreaMax = 6;
    }

    // 持續更新
    void Update()
    {
        //若開始遊戲的布林值預設為true，執行以下動作
        if (StartGameBool == true)
        {
            //若UnityChan踩在地板上，執行以下動作
            if (UnityChan.isGrounded)
            {
                //指示路徑向量：只管向前走，不往左右或上下，也不往後方
                moveDirection = new Vector3(0, 0, 1);
                moveDirection = transform.TransformDirection(moveDirection);
                
                //若「是否向左移動」布林值為true，執行以下動作
                if(LeftMoveBool == true)
                {
                    //指示路徑向量：向左移動2單位，其他方向不作用
                    moveDirection = new Vector3(-2, 0, 1);
                    moveDirection = transform.TransformDirection(moveDirection);
                    //水平移動用計時器每秒+1
                    HorizontalMove_Timer += Time.deltaTime;
                    //若水平移動經過0.3秒，執行以下動作
                    if(HorizontalMove_Timer >= 0.3f)
                    {
                        //將水平移動用計時器歸零，留待下次使用
                        HorizontalMove_Timer = 0;
                        //將「是否向左移動」布林值設為false，停止向左移動
                        LeftMoveBool = false;
                        //將玩家角色的往左跑步動畫布林值設為false
                        this.GetComponent<_01_AnimControl>().RunLeftBool = false;
                    }
                }

                //若「是否向右移動」布林值為true，執行以下動作
                if (RightMoveBool == true)
                {
                    //指示路徑向量：向右移動2單位，其他方向不作用
                    moveDirection = new Vector3(2, 0, 1);
                    moveDirection = transform.TransformDirection(moveDirection);
                    //水平移動用計時器每秒+1
                    HorizontalMove_Timer += Time.deltaTime;
                    //若水平移動經過0.3秒，執行以下動作
                    if (HorizontalMove_Timer >= 0.3f)
                    {
                        //將水平移動用計時器歸零，留待下次使用
                        HorizontalMove_Timer = 0;
                        //將「是否向右移動」布林值設為false，停止向右移動
                        RightMoveBool = false;
                        //將玩家角色的往右跑步動畫布林值設為false
                        this.GetComponent<_01_AnimControl>().RunRightBool = false;
                    }
                }

                //若「是否跳躍」布林值為true，執行以下動作
                if (JumpBool == true)
                {
                    //指示路徑向量：只管往上方&前方走，其他方向不作用
                    moveDirection = new Vector3(0, JumpSpeed, 1);
                    moveDirection = transform.TransformDirection(moveDirection);
                }
                //定義指示路徑 = 向量*移動速度
                moveDirection *= RunSpeed;
            }
            //指示路徑的Y向量會受到重力影響
            moveDirection.y -= Gravity * Time.deltaTime;
            //命令玩家角色(UnityChan)，根據定義好的指示路徑進行移動
            UnityChan.Move(moveDirection * Time.deltaTime);
        }
        //若開始遊戲的布林值預設為true，執行以下動作
        if (StartGameBool == true)
        {
            //更新玩家角色(UnityChan)的現在位置
            UnityChanPosition = this.GetComponent<Transform>().position;
            //定義攝影機跟隨向量，X = UnityChan所在位置的X，Y = 攝影機所在高度，Z = UnityChan所在位置的Z + 10.0
            //f為浮點數，不可刪除
            CameraFollowVector = new Vector3(UnityChanPosition.x, 23.48f, UnityChanPosition.z + 10f);
            //更新攝影機的位置，讓攝影機跟隨玩家，保持一定距離移動
            MainCamera.GetComponent<Transform>().position = CameraFollowVector;
        }
        //若結束遊戲的布林值預設為false，執行以下動作
        if (EndGameBool == false)
        {
            //開始遊戲計時器，每秒+1
            StartGameTimer += Time.deltaTime;
            //若開始遊戲計時器的累計秒數在1以上，執行以下動作
            if (StartGameTimer >= 1)
            {
                //將開始遊戲布林值設為true
                StartGameBool = true;
                //將開始遊戲計時器重設為0秒
                StartGameTimer = 0;
                //將玩家角色的跑步動畫布林值設為true
                this.GetComponent<_01_AnimControl>().RunBool = true;
            }
        }
        //若[玩家是否已進入新關卡]布林值為true，執行以下動作
        if (PlayerEnterMissionBool == true)
        {
            //關卡隨機值。骰關卡種類：從關卡A~關卡C 隨機其1
            int RandomMissionIndex = Random.Range(0, 3);
            //生成隨機關卡1：設定生成關卡位置，旋轉角度為初始值
            Instantiate(MissionArea[RandomMissionIndex], new Vector3(-2.1f, 12.4f, -31.5f * MissionAreaIndex), Quaternion.Euler(-89.98f, 0, 0));

            //將[玩家是否已進入新關卡]布林值重設為false
            PlayerEnterMissionBool = false;
        }
    }
 
    //向左移動按鈕事件
    public void LeftMoveButtonEvent()
    {
        //若玩家角色 X 位置 > 0.1 ，且「是否向右移動」布林值為false，執行以下動作
        if (UnityChanPosition.x > 0.1f && RightMoveBool == false)
        {
            //將「是否向左移動」布林值設為true
            LeftMoveBool = true;
            //將玩家角色的往左跑步動畫布林值設為true
            this.GetComponent<_01_AnimControl>().RunLeftBool = true;
        }
    }

    //向右移動按鈕事件
    public void RightMoveButtonEvent()
    {
        //若玩家角色 X 位置 < 1.1 ，且「是否向左移動」布林值為false，執行以下動作
        if (UnityChanPosition.x < 1.1f && LeftMoveBool == false)
        {
            //將「是否向右移動」布林值設為true
            RightMoveBool = true;
            //將玩家角色的往右跑步動畫布林值設為true
            this.GetComponent<_01_AnimControl>().RunRightBool = true;
        }
    }

    //跳躍按鈕事件
    public void JumpButtonEvent()
    {
        //將「是否跳躍」布林值設為true
        JumpBool = true;
        //將玩家角色的跳躍動畫布林值設為true
        this.GetComponent<_01_AnimControl>().JumpBool = true;
    }

    //角色碰撞器專用的[碰撞事件](Collider Script)
    void OnControllerColliderHit(ControllerColliderHit UnityChanHit)
    {
        //根據玩家角色碰撞到的物件tag，執行不同動作
        switch (UnityChanHit.gameObject.tag)
        {
            case "GoalGate":
                //若玩家碰撞終點，會打印出"You are Win!"(你贏了)訊息
                Debug.Log("You are Win!");

                //將開始遊戲布林值設為false
                StartGameBool = false;
                //將結束遊戲布林值設為true
                EndGameBool = true;
                //將玩家角色的移動到起點，留待下次闖關
                sos.transform.position = new Vector3();
                sos.transform.rotation = Quaternion.Euler(0, 270, 0);
                //this.gameObject.GetComponent<Transform>().position = new Vector3(-2.2f,16.5f,23.63f);
                //將玩家角色的勝利動畫布林值設為true
                this.GetComponent<_01_AnimControl>().WinBool = true;
                //切換攝影機位置&角度：轉至拍攝UnityChan的正面
                MainCamera.GetComponent<Transform>().position = new Vector3(-2.1f, 21.57f, 17.77f);
                MainCamera.GetComponent<Transform>().rotation = Quaternion.Euler(15, 0, 0);
                //MainCamera.GetComponent<Transform>().position = new Vector3(UnityChanPosition.x,UnityChanPosition.y + 5.5f,UnityChanPosition.z - 6f);
                //MainCamera.GetComponent<Transform>().rotation = Quaternion.Euler(15, 0, 0);
                break;
            case "SeaGround":
                //若玩家碰撞海面，會打印出"You are Lose!"訊息
                Debug.Log("You are Lose!");

                //將開始遊戲布林值設為false
                StartGameBool = false;
                //將結束遊戲布林值設為true
                EndGameBool = true;
                //將玩家角色的移動到起點，留待下次闖關
                this.gameObject.GetComponent<Transform>().position = new Vector3(-2.2f,16.5f,23.63f);
                //將玩家角色的勝利動畫布林值設為true
                this.GetComponent<_01_AnimControl>().LoseBool = true;
                //切換攝影機位置&角度：轉至拍攝UnityChan的正面
                MainCamera.GetComponent<Transform>().position = new Vector3(-2.1f, 21.57f, 17.77f);
                MainCamera.GetComponent<Transform>().rotation = Quaternion.Euler(15, 0, 0);
                //MainCamera.GetComponent<Transform>().position = new Vector3(UnityChanPosition.x, UnityChanPosition.y + 5.5f, UnityChanPosition.z - 6f);
                //MainCamera.GetComponent<Transform>().rotation = Quaternion.Euler(15, 0, 0);
                break;
        }
    }

    //角色碰撞器專用的[觸發事件](Trigger script)
    void OnTriggerEnter(Collider UnityChanTrigger)
    {

        //根據玩家角色觸發到的物件tag，執行不同動作
        switch (UnityChanTrigger.tag)
        {
            case "NormalGround":
                //將「是否跳躍」布林值設為false
                JumpBool = false;
                //將玩家角色的跳躍動畫布林值設為false
                this.GetComponent<_01_AnimControl>().JumpBool = false;
                break;
            case "MissionTrigger":
                //若生成關卡的當前索引值 < 生成關卡的最大上限值，執行以下動作
                if (MissionAreaIndex < MissionAreaMax)
                {
                    //生成關卡的當前索引值+1
                    MissionAreaIndex += 1;
                    //將[玩家是否已進入新關卡]布林值設為true
                    PlayerEnterMissionBool = true;
                }
                else if (MissionAreaIndex == MissionAreaMax)
                {
                    //若生成關卡的當前索引值 = 生成關卡的最大上限值
                    //生成終點區域遊戲物件：設定生成的位置，旋轉角度為(-89.98,0,0)
                    Quaternion EndAreaRotation = Quaternion.Euler(-89.98f, 0, 0);
                    Instantiate(EndArea, new Vector3(-2.1f, 12.4f, -31.5f * MissionAreaIndex - 31.5f), EndAreaRotation);
                    // 將生成關卡的當前索引值+1，避免重複生成終點區域遊戲物件
                    MissionAreaIndex += 1;
                }
                break;  
        }
    }
}
 