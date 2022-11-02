
using MPayNameSpace;
using NeteaseLogin;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace Unisdk
{
    public class MPay : CppCliUnisdkMPay
    {
        private const int NT_LOGIN_OK = 0;
        private const int NT_LOGIN_CANCEL = 1;
        private const int NT_LOGIN_WRONG_PASSWD = 2;
        private const int NT_LOGIN_NET_UNAVAILABLE = 3;
        private const int NT_LOGIN_SDK_SERV_ERR = 4;
        private const int NT_LOGIN_NET_TIME_OUT = 5;
        private const int NT_LOGIN_SDK_NOT_INIT = 6;
        private const int NT_LOGIN_UNKNOWN_ERR = 10;
        private const int NT_LOGIN_NEED_GS_CONFIRM = 11;
        private const int NT_LOGIN_NEED_RELOGIN = 12;
        private const int NT_LOGIN_BIND_OK = 13;
        private const int NT_CHECKORDER_PREPARING = 0;
        private const int NT_CHECKORDER_CHECKING = 1;
        private const int NT_CHECKORDER_CHECK_OK = 2;
        private const int NT_CHECKORDER_CHECK_ERR = 3;
        private string Sauth;



        protected override void onCheckOrderFinish(int errorCode, int orderStatus, string productId, uint productCount, string orderId, string errReason)
        {
            throw new NotImplementedException();
        }

        protected override void onCompactViewClosed(int code)
        {
            MessageBox.Show("test");
        }

        protected override void onExtendFuncFinish(string json)
        {
            throw new NotImplementedException();
        }

        protected override void onInitFinish(int code)
        {
            Call.Log += "[MPay][onInitFinish]" + (object)code + "\r\n";
            if (code == 0)
            {

            }
            else
            {
                Call.Log += "[MPay]mpay init error" + "\r\n";
            }
        }

        protected override void onLog(string log)
        {
            if(log.Contains("Login success"))
            {
                Thread thread = new Thread((ThreadStart)delegate
                {
                    Sauth = this.GetSAuthPropStr();
                    while(Sauth==null || Sauth.Trim().Length == 0)
                    {
                        RunLoop(500);
                        Sauth = this.GetSAuthPropStr();
                        Thread.Sleep(500);
                    }
                    onLoginFinish(0);
                    log = "";
                });
                thread.IsBackground = true;
                thread.Start(); 

            }
        }

        protected override void onLoginFinish(int code)
        {
            if (code == 0)
            {
                Call.Log += "[MPay][sauthJson]" + this.GetSAuthPropStr()+ "\r\n";
                Clean();

            }

        }
        protected new void ShowRealName()
        {
            ShowRealName();
        }
        protected override void onLogoutFinish(int code)
        {
            Call.Log += "[MPay][onLogoutFinish]" + (object)code + "\r\n";
            if (code != 0)
                return;
            Login();
        }
    }
}
