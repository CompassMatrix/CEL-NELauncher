using System;
using MPayNameSpace;
using Unisdk;

namespace NeteaseLogin
{
	public class MPaySdkManager
	{
		public MPay mpay = null;

		public string sauthJson = "";

		public void Init()
		{
			if (mpay == null)
			{
				mpay = new MPay();
				((CppCliUnisdkMPay)mpay).Init("我的世界启动器", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Netease\\MCLauncher\\config\\mpay\\");
			}
		}
		public void ShowRealName()
		{
			((CppCliUnisdkMPay)mpay).ShowRealName();
		}
		public string GetLog()
		{
			string log = Call.Log;
			Call.Log = "";
			return log;
		}

		public void Login()
		{
			if (mpay != null)
			{
				((CppCliUnisdkMPay)mpay).Login();
			}
		}

		public void Logout()
		{
			if (mpay != null)
			{
				((CppCliUnisdkMPay)mpay).Logout();
			}
		}

		public void RunLoop(float deltaTime)
		{
			if (mpay != null)
			{
				((CppCliUnisdkMPay)mpay).RunLoop(deltaTime);
			}
		}

		public void clean()
		{
			if (mpay != null)
			{
				((CppCliUnisdkMPay)mpay).Clean();
				mpay = null;
			}
		}
	}
}
