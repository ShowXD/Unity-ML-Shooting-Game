﻿using UnityEngine;

public class Bullet : MonoBehaviour
{
    /// <summary>
    /// 子彈是否射中標靶
    /// </summary>
    public static bool getShot;

    /// <summary>
    /// 觸發事件：碰到Is Trigger的物件會執行
    /// </summary>
    /// <param name="other">碰到的物件資訊</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "target-sensor")
        {
            getShot = true;
        }
    }
}
