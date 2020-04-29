using UnityEngine;

public class Bullet : MonoBehaviour
{
    /// <summary>
    /// 子彈射中標靶的次數
    /// </summary>
    public static int getOne = 0;

    /// <summary>
    /// 觸發事件：碰到Is Trigger的物件會執行
    /// </summary>
    /// <param name="other">碰到的物件資訊</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "target-sensor")
        {
            getOne = getOne + 1;
        }
    }
}
