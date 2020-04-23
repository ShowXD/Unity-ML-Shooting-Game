using UnityEngine;
using System.Collections;
using MLAgents;
using MLAgents.Sensors;

public class Character : Agent
{
    [Header("速度"), Range(1, 100)]
    public float torque = 10;

    [Header("子彈數量"), Range(5, 500)]
    public int bulletCount = 30;

    [Header("子彈的種類")]
    public GameObject bullet;

    [Header("發射器")]
    public GameObject bullet_emitter;

    [Header("子彈速度")]
    public float bullet_forward_force = 100;

    /// <summary>
    /// 剩餘的子彈數量
    /// </summary>
    private int remainBullet;

    /// <summary>
    /// 是否射擊：0為沒有、1為有
    /// </summary>
    private float isShoot;

    /// <summary>
    /// 人物鋼體
    /// </summary>
    private Rigidbody rigChar;

    /// <summary>
    /// 子彈鋼體
    /// </summary>
    private Rigidbody rigBullet;

    /// <summary>
    /// 取得人物動作控制器
    /// </summary>
    public Animator animator;

    /// <summary>
    /// 遊戲開始時：取得人物、子彈的剛體
    /// 將剩餘的子彈填充
    /// </summary>
    private void Start()
    {
        rigChar = GetComponent<Rigidbody>();
        rigBullet = bullet.GetComponent<Rigidbody>();
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
        Vector3 posChar = new Vector3(Random.Range(-6f, 6f), 0.05f, 0);
        transform.position = posChar;

        // 子彈數量重置
        remainBullet = bulletCount;

        // 子彈尚未命中標靶
        Bullet.getShot = false;
    }

    /// <summary>
    /// 收集觀測資料
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        // 加入觀測資料：角色位置、角色旋轉方向、是否射擊
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(rigBullet.position);
    }

    /// <summary>
    /// 動作：控制機器人與回饋
    /// </summary>
    /// <param name="vectorAction"></param>
    public override void OnActionReceived(float[] vectorAction)
    {
        # region 使用參數控制角色
        // 視角的rotation
        rigChar.AddTorque(transform.up * torque * vectorAction[0]);
        #endregion

        // 子彈打中目標，成功：加1分並結束
        if (Bullet.getShot)
        {
            SetReward(1);
            EndEpisode();
        }

        // 子彈數量用完，失敗：扣1分並結束
        if (bulletCount == 0 || transform.position.y < 0)
        {
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
        Debug.Log(action[0]);
        return action;
    }

    /// <summary>
    /// 將滑鼠按下的類別轉成數值
    /// </summary>
    /// <param name="temp"></param>
    /// <returns></returns>
    private float shootBtoF(bool temp)
    {
        if (temp == true)
        {
            return 1.0f;
        }
        else
        {
            return 0.0f;
        }
    }

    /// <summary>
    /// 因參數而變更角色射擊動畫
    /// </summary>
    private void shoot(float temp)
    {
        if (temp == 1.0f)
        {
            animator.SetBool("Shoot", true);

            // 生成子彈並給予外力移動
            GameObject Temporary_Bullet_Handler;
            Temporary_Bullet_Handler = Instantiate(bullet, new Vector3(bullet_emitter.transform.position.x, bullet_emitter.transform.position.y, bullet_emitter.transform.position.z+0.5f), bullet.transform.rotation) as GameObject;

            Rigidbody Temporary_RigidBody;
            Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody>();
            Temporary_RigidBody.AddForce(transform.forward * bullet_forward_force);
            Destroy(Temporary_Bullet_Handler, 5.2f);
        }
        else if (temp == 0.0f)
        {
            animator.SetBool("Shoot", false);
        }
    }

    /// <summary>
    /// 自動重複射擊子彈
    /// </summary>
    private void Update()
    {
        // 生成子彈並給予外力移動
        GameObject Temporary_Bullet_Handler;
        Temporary_Bullet_Handler = Instantiate(bullet, new Vector3(bullet_emitter.transform.position.x, bullet_emitter.transform.position.y, bullet_emitter.transform.position.z + 0.5f), bullet.transform.rotation) as GameObject;

        Rigidbody Temporary_RigidBody;
        Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody>();
        Temporary_RigidBody.AddForce(transform.forward * bullet_forward_force);
        remainBullet -= 1;
        Destroy(Temporary_Bullet_Handler, 0.5f);
    }
}
