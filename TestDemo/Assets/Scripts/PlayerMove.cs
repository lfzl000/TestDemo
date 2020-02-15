using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMove : MonoBehaviour
{
    private bool isMove;
    private Transform player;
    private Vector3 targetPos;
    private UnityAction moveComplete;
    private float moveSpeed;
    /// <summary>
    /// 是否正在移动中
    /// </summary>
    public bool IsMoveing
    {
        get
        {
            return isMove;
        }
    }

    /// <summary>
    /// 移动到目标点
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="_pos"></param>
    /// <param name="_speed"></param>
    /// <param name="_callback"></param>
    public void MoveToTarget(PlayerItem _player, Vector3 _pos, float _speed, UnityAction _callback = null)
    {
        isMove = true;
        player = _player.transform;
        targetPos = _pos;
        moveComplete = _callback;
        moveSpeed = _speed;
    }

    private void Update()
    {
        if(isMove)
        {
            if (Vector3.Distance(player.position, targetPos) >= 0.1f)
            {
                Vector3 dis = targetPos - player.position;
                player.Translate((dis.x * moveSpeed * Time.deltaTime), (dis.y * moveSpeed * Time.deltaTime), (dis.z * moveSpeed * Time.deltaTime), Space.World);
            }
            else
            {
                isMove = false;
                moveComplete?.Invoke();
            }
        }
    }
}