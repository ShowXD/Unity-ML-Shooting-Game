using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class Character : Agent
{
    [Header("速度"), Range(1, 100)]
    public float speed = 10;

    [Header("子彈數量"), Range(5, 500)]
    public int bulletCount = 30;

    [Header("子彈的種類")]
    public GameObject bullet;

    [Header("發射器")]
    public GameObject bullet_emitter;

    [Header("子彈速度")]
    public float bullet_forward_force = 7000;

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
        rigChar.rotation = Quaternion.identity;

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
        //sensor.AddObservation(isShoot);
    }

    /// <summary>
    /// 動作：控制機器人與回饋
    /// </summary>
    /// <param name="vectorAction"></param>
    public override void OnActionReceived(float[] vectorAction)
    {
        # region 使用參數控制角色
        // 視角的rotation
        // rigChar.transform.Rotate(new Vector3(-vectorAction[0], vectorAction[1], 0f) * Time.deltaTime * 20.0f);

        // 人物位置
        Vector3 control = Vector3.zero;
        control.x = vectorAction[0];
        control.z = vectorAction[1];
        rigChar.AddForce(control * speed);

        // 角色射擊
        isShoot = vectorAction[2];
        shoot(vectorAction[2]);
        #endregion

        // 子彈打中目標，成功：加1分並結束
        if (Bullet.getShot)
        {
            SetReward(1);
            EndEpisode();
        }

        // 子彈數量用完，失敗：扣1分並結束
        if (bulletCount == 0)
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
        var action = new float[3];
        // action[0] = Input.GetAxis("Mouse Y");
        // action[1] = Input.GetAxis("Mouse X");
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        action[2] = shootBtoF(Input.GetMouseButtonDown(0));
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
            print("射擊");
            animator.SetBool("Shoot", true);

            // 生成子彈並給予外力移動
            GameObject Temporary_Bullet_Handler;
            Temporary_Bullet_Handler = Instantiate(bullet, bullet_emitter.transform.position, bullet.transform.rotation) as GameObject;

            Rigidbody Temporary_RigidBody;
            Temporary_RigidBody = Temporary_Bullet_Handler.GetComponent<Rigidbody>();
            Temporary_RigidBody.AddForce(transform.forward * bullet_forward_force);
            Destroy(Temporary_Bullet_Handler, 0.2f);
        }
        else if (temp == 0.0f)
        {
            animator.SetBool("Shoot", false);
        }
    }
}
