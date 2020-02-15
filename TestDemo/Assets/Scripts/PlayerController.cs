using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ZLMsg;

public class PlayerController : MonoBehaviour, IMsgSender, IMsgReceiver
{
    public GameObject m_Point;
    public LayerMask m_LayerMask;

    private Ray mInputRay;
    private Vector3 mMousePos;
    private Camera mCamera;
    private RaycastHit mInputHit;
    private Transform mCurHitTarget;
    private MsgParam<GameObject> playerParam;
    private MsgParam<Vector3> posParam;
    private Vector3 mHitPos;
    private bool isHavePlayer;

    private void Start()
    {
        playerParam = new MsgParam<GameObject>();
        posParam = new MsgParam<Vector3>();
        this.RegisterLogicMsg(MsgName.MSG_PLAYER_MOVE_END, PlayerMoveEnd);
    }

    private void GetMouseTarget()
    {
        if (mCamera == null)
        {
            mCamera = Camera.main;
        }
        mMousePos = Input.mousePosition;
        mInputRay = mCamera.ScreenPointToRay(mMousePos);
        if (Physics.Raycast(mInputRay, out mInputHit, float.MaxValue, m_LayerMask))
        {
            mHitPos = mInputHit.point;
            mCurHitTarget = mInputHit.transform;

            if (mCurHitTarget.gameObject.tag == "Player")
            {
                playerParam.SetParam(mCurHitTarget.gameObject);
                this.SendLogicMsg(MsgName.MSG_MOUSE_STAY_PLAYER, playerParam);
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    if (mCurHitTarget.gameObject.tag == "Player")
                    {
                        this.SendLogicMsg(MsgName.MSG_RESET_AREA, null);
                        isHavePlayer = true;
                    }
                    this.SendLogicMsg(MsgName.MSG_SELECT_PLAYER, playerParam);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    if (mCurHitTarget.gameObject.tag == "Terrain")
                    {
                        m_Point.SetActive(true);
                        m_Point.transform.position = mHitPos;
                        mHitPos = new Vector3(mHitPos.x, 0.1f, mHitPos.z);
                        posParam.SetParam(mHitPos);
                        this.SendLogicMsg(MsgName.MSG_SET_AREA, posParam);
                    }
                    isHavePlayer = false;
                    playerParam.SetParam(null);
                    this.SendLogicMsg(MsgName.MSG_SELECT_PLAYER, playerParam);
                }
            }
            //if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            //{
            //    if (mCurHitTarget.gameObject.tag == "Player")
            //    {
            //        this.SendLogicMsg(MsgName.MSG_RESET_AREA, null);
            //        isHavePlayer = true;
            //        playerParam.SetParam(mCurHitTarget.gameObject);
            //    }
            //    else
            //    {
            //        if(mCurHitTarget.gameObject.tag == "Terrain")
            //        {
            //            m_Point.SetActive(true);
            //            m_Point.transform.position = mHitPos;
            //            mHitPos = new Vector3(mHitPos.x, 0.1f, mHitPos.z);
            //            posParam.SetParam(mHitPos);
            //            this.SendLogicMsg(MsgName.MSG_SET_AREA, posParam);
            //        }
            //        isHavePlayer = false;
            //        playerParam.SetParam(null);
            //    }
            //    this.SendLogicMsg(MsgName.MSG_SELECT_PLAYER, playerParam);
            //}


            if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (mCurHitTarget.gameObject.tag == "Terrain" && isHavePlayer)
                {
                    m_Point.SetActive(true);
                    m_Point.transform.position = mHitPos;
                    mHitPos = new Vector3(mHitPos.x, 0.4f, mHitPos.z);
                    posParam.SetParam(mHitPos);
                    this.SendLogicMsg(MsgName.MSG_PLAYER_MOVE, posParam);
                }
            }
        }
    }

    private void PlayerMoveEnd(IMsgParam obj)
    {
        m_Point.SetActive(false);
    }

    private void Update()
    {
        GetMouseTarget();
    }
}