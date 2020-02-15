using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZLMsg;

public class PlayerItem : MonoBehaviour, IMsgReceiver, IMsgSender
{
    public float m_MoveSpeed = 5;
    public bool isCurPlayer;
    private Color bak_color;
    private PlayerMove mPlayerMove;
    private Renderer m_CurPlayerRenderer;
    private bool mIsInside;

    public Renderer CurPlayerRenderer
    {
        get
        {
            return m_CurPlayerRenderer;
        }
    }

    /// <summary>
    /// 是否在范围内
    /// </summary>
    public bool IsInside
    {
        get
        {
            return mIsInside;
        }
    }

    private void Start()
    {
        this.RegisterLogicMsg(MsgName.MSG_SELECT_PLAYER, GetPlayer);
        this.RegisterLogicMsg(MsgName.MSG_PLAYER_MOVE, PlayerMoveToTarget);
        this.RegisterLogicMsg(MsgName.MSG_JUDGE_PLAYER_IN_INSIDE, SetArea);
        this.RegisterLogicMsg(MsgName.MSG_MOUSE_STAY_PLAYER, SelectCurPlayer);
        if (m_CurPlayerRenderer == null)
        {
            m_CurPlayerRenderer = GetComponent<Renderer>();
        }
        bak_color = m_CurPlayerRenderer.material.color;
        if (mPlayerMove == null)
        {
            mPlayerMove = gameObject.AddComponent<PlayerMove>();
        }

        m_EnterArea = EnterArea;
        m_ExitArea = ExitArea;
        m_StayArea = StayArea;
    }

    private void SelectCurPlayer(IMsgParam obj)
    {
    }

    private MsgParam<string> mssageParam = new MsgParam<string>();
    private void EnterArea()
    {
        Debug.Log(gameObject.name + "已经走进区域");
        mssageParam.SetParam(gameObject.name + "已经走进区域");
        this.SendLogicMsg(MsgName.MSG_MESSAGE, mssageParam);
    }

    private void ExitArea()
    {
        Debug.Log(gameObject.name + "已经走出区域");
        m_CurPlayerRenderer.material.color = Color.red;

        mssageParam.SetParam(gameObject.name + "已经走出区域");
        this.SendLogicMsg(MsgName.MSG_MESSAGE, mssageParam);
    }

    private void StayArea()
    {
        //Debug.Log("在范围中");
        m_CurPlayerRenderer.material.color = Color.blue;
    }

    private void PlayerMoveToTarget(IMsgParam obj)
    {
        MsgParam<Vector3> msgParam = obj as MsgParam<Vector3>;
        if (isCurPlayer)
        {
            mPlayerMove.MoveToTarget(this, msgParam.param, m_MoveSpeed, MoveEnd);
        }
    }

    private void MoveEnd()
    {
        this.SendLogicMsg(MsgName.MSG_PLAYER_MOVE_END, null);
    }

    private void GetPlayer(IMsgParam obj)
    {
        MsgParam<GameObject> msgParam = obj as MsgParam<GameObject>;
        if (msgParam.param != null && msgParam.param == gameObject)
        {
            Debug.Log("当前选中的 Player 为:" + gameObject.name);
            isCurPlayer = true;
            m_CurPlayerRenderer.material.color = Color.red;
        }
        else
        {
            isCurPlayer = false;
            m_CurPlayerRenderer.material.color = bak_color;
        }
    }

    //进入范围,出来范围,在范围中待着
    public UnityAction m_EnterArea, m_ExitArea, m_StayArea;
    private bool isInsideBak;

    /// <summary>
    /// 判断玩家是否在范围内
    /// </summary>
    private void JudgePlayerIsInside()
    {
        mIsInside = AreaController.IsPointInsidePolygon(transform.position);
        if (mIsInside != isInsideBak)
        {
            if (mIsInside)
            {
                m_EnterArea?.Invoke();
            }
            else
            {
                m_ExitArea?.Invoke();
            }
        }
        
        isInsideBak = mIsInside;
    }

    private void SetArea(IMsgParam obj)
    {
        JudgePlayerIsInside();
    }

    private void Update()
    {
        if (mPlayerMove && mPlayerMove.IsMoveing)
        {
            JudgePlayerIsInside();
        }
        if (mIsInside)
        {
            m_StayArea?.Invoke();
        }
    }
}