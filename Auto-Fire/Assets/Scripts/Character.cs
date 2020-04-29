using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class Character : Agent
{
    [Header("旋轉速度"), Range(1, 20)]
    public float torque = 5;

    [Header("子彈數量"), Range(1, 300)]
    public int bulletCount = 25;

    [Header("子彈需要命中的數量"), Range(1, 300)]
    public int bulletLimit = 5;

    [Header("子彈的種類")]
    public GameObject bullet;

    [Header("發射器")]
    public GameObject bullet_emitter;

    [Header("子彈速度"), Range(500, 2200)]
    public float bullet_forward_force = 1500;

    /// <summary>
    /// 剩餘的子彈數量
    /// </summary>
    private int remainBullet;

    /// <summary>
    /// 人物鋼體
    /// </summary>
    private Rigidbody rigChar;

    /// <summary>
    /// 取得視線物件
    /// </summary>
    private GameObject sight;
    private Rigidbody rigSight;

    /// <summary>
    /// 遊戲開始時：取得人物、子彈的剛體
    /// 將剩餘的子彈填充
    /// </summary>
    private void Start()
    {
        rigChar = GetComponent<Rigidbody>();
        sight = GameObject.Find("Sight");
        rigSight = sight.GetComponent<Rigidbody>();
        remainBullet = bulletCount;
    }

    /// <summary>
    /// 事件開始時：重新設定人物跟標靶的位置
    /// </summary>
    public override void OnEpisodeBegin()
    {
        // 重設剛體的旋轉方向
        rigChar.velocity = Vector3.zero;
        rigChar.angularVelocity = Vector3.zero;
        

        // 隨機角色位置
        Vector3 posChar = new Vector3(Random.Range(-8f, 8f), 0.05f, 0f);
        transform.position = posChar;
        Vector3 rotChar = new Vector3(0f, 0f, 0f);
        transform.eulerAngles = rotChar;

        // 子彈數量重置
        remainBullet = bulletCount;

        //摧毀所有留下來的子彈
        var clones = GameObject.FindGameObjectsWithTag("bullet");
        foreach (var clone in clones)
        {
            Destroy(clone);
        }

        // 子彈尚未命中標靶
        Bullet.getOne = 0;
        Sight.get = false;

        // 子彈持續產生
        CancelInvoke("Shoot");
        InvokeRepeating("Shoot", 1.0f, 0.1f);
    }

    /// <summary>
    /// 收集觀測資料
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        // 加入觀測資料：角色位置、角色旋轉方向、是否射擊
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
    }

    /// <summary>
    /// 動作：控制機器人與回饋
    /// </summary>
    /// <param name="vectorAction"></param>
    public override void OnActionReceived(float[] vectorAction)
    {
        # region 使用參數控制角色、視線
        // 視角的rotation
        rigChar.AddTorque(transform.up * torque * vectorAction[0]);
        
        // 設置視線位置
        Vector3 posSight = bullet_emitter.transform.position;
        sight.transform.position = posSight;
        sight.transform.rotation = transform.rotation;
        #endregion

        // 子彈打中目標，成功：加1分並結束
        if (Bullet.getOne == bulletLimit)
        {
            Debug.Log("Success");
            SetReward(1);
            EndEpisode();
        }
        //if (Sight.get)
        //{
        //    Debug.Log("Success");
        //    SetReward(1);
        //    EndEpisode();
        //}

        // 子彈數量用完，失敗：扣1分並結束
        if (remainBullet == 0)
        {
            Debug.Log("Failed");
            SetReward(-1);
            EndEpisode();
        }
    }

    /// <summary>
    /// 讓開發者測試環境
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic()
    {
        // 提供開發者控制的方式
        var action = new float[1];
        action[0] = Input.GetAxis("Horizontal");
        return action;
    }

    /// <summary>
    /// 射擊
    /// </summary>
    private void Shoot()
    {
        // 生成子彈並給予外力移動
        GameObject Temporary_Bullet_Handler;
        Temporary_Bullet_Handler = Instantiate(bullet, bullet_emitter.transform.position, bullet.transform.rotation) as GameObject;

        Rigidbody Temporary_RigidBody;
        Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody>();
        Temporary_RigidBody.AddForce(transform.forward * bullet_forward_force);
        remainBullet = remainBullet - 1;
        Destroy(Temporary_Bullet_Handler, 1.0f);
    }

    private void Update()
    {
        Debug.Log(Bullet.getOne);
    }
}
