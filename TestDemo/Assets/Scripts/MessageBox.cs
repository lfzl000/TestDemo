using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZLMsg;

public class MessageBox : MonoBehaviour, IMsgReceiver
{
    public Text message_tex;

    private void Start()
    {
        this.RegisterLogicMsg(MsgName.MSG_MESSAGE, ShowMessage);
    }

    private void ShowMessage(IMsgParam obj)
    {
        StopCoroutine("MessageShow");
        MsgParam<string> msgParam = obj as MsgParam<string>;
        message_tex.text = msgParam.param;
        StartCoroutine("MessageShow");
    }

    private IEnumerator MessageShow()
    {
        yield return new WaitForSeconds(1);
        message_tex.text = "";
    }
}