using Microsoft;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeteaseLogin
{
	public class RPC
	{
		private bool NeedChaCha = false;

		private ChaChaX send_ccx;

		private ChaChaX rec_ccx;

		private int m_launchIdx = -1;

		private SocketCallback m_socketCallbackFuc = new SocketCallback();

		private readonly string m_launcherIp = "127.0.0.1";

		private bool m_isNormalExit = false;

		public Dictionary<string, Action> CloseActions = new Dictionary<string, Action>();

		public Dictionary<string, Action> ReadyActions = new Dictionary<string, Action>();

		public int FingerPrint;

		private bool m_isLaunchIdxReady;

		private TcpListener m_mcControlListener;

		private string serverIP;

		public int serverPort;

		public string RoleName;

		public int porcessID;

		private ThreadManager tm = new ThreadManager();

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private BinaryWriter c;

		public int d;

		private bool f = false;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int i;

		private int j = -1;

		private bool k;

		private TcpListener m;

		private readonly string n = "127.0.0.1";

		public Dictionary<string, Action> p = new Dictionary<string, Action>();

		public Dictionary<string, Action> q = new Dictionary<string, Action>();

		private Dictionary<string, Action> r = new Dictionary<string, Action>();

		public TcpClient Client { get; private set; }

		public BinaryReader Reader { get; private set; }
		public struct b
		{
			public int _a;

			public string _b;
		}
		public BinaryWriter Writer { get; private set; }

		public int LauncherControlPort { get; protected set; }

		public SocketCallback SocketCallbackFuc
		{
			get
			{
				return m_socketCallbackFuc;
			}
			protected set
			{
				if (value == null)
				{
					StaticClient.send($"[MainError]The Callback is null!", "");
				}
				else
				{
					m_socketCallbackFuc = value;
				}
			}
		}

		public RPC(TcpClient tcpClient)
		{
			Client = tcpClient;
			NetworkStream stream = Client.GetStream();
			Reader = new BinaryReader(stream);
			Writer = new BinaryWriter(stream);
		}

		public RPC(int port, string serverIP, int serverPort, string RoleName, int fingerprint = 0)
		{
			StaticClient.send($"[Details-RPC]ListeningPort {port} WatingJavaw", "GamingProcess");
			LauncherControlPort = port;
			this.serverIP = serverIP;
			this.serverPort = serverPort;
			this.RoleName = RoleName;
			FingerPrint = fingerprint;
			StartControlConnection();
		}

		private void StartControlConnection()
		{
			try
			{
				m_mcControlListener = new TcpListener(IPAddress.Parse(m_launcherIp), LauncherControlPort);
				m_mcControlListener.Start();
			}
			catch (Exception ex)
			{
				PrintInfo(ex.Message);
				CloseControlConnection();
				return;
			}
			tm.Create(ListenControlConnect);
			StaticClient.send($"[Details-RPC]StartedSuccessed Port", LauncherControlPort.ToString()) ;
			SocketCallbackFuc.RegisterReceiveCallBack(512, HandShake);
			SocketCallbackFuc.RegisterReceiveCallBack(0, PrepareCha);
			SocketCallbackFuc.RegisterReceiveCallBack(517, AuthServer);
			SocketCallbackFuc.RegisterReceiveCallBack(18, ChatCallBack);
		}
		private void ChatCallBack(byte[] obj)
		{
			b b_ = default(b);
			new SimpleUnpack(obj).Unpack(ref b_);
			SendControlData(SimplePack.Pack(new object[] { 18, b_._a, b_._b }));
		}
		private void PrepareCha(byte[] iyb)
		{
			int num = porcessID;
			byte[] array = new byte[16];
			Array.Clear(array, 0, array.Length);
			array[0] = (byte)(num % 256);
			array[1] = (byte)(num / 256);
			CC content = new CC();
			new SimpleUnpack(iyb).Unpack(ref content);
			byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(content.c);
			byte[] array2 = new byte[32];
			Array.Clear(array2, 0, array2.Length);
			Array.Copy(bytes, 16, array2, 0, 16);
			Array.Copy(bytes, 0, array2, 16, 16);
			rec_ccx = new ChaCha(array2);
			send_ccx = new ChaCha(bytes);
			NeedChaCha = true;
		}

		public static string l(byte[] kyw, int kyx = 0, int kyy = 0)
		{
			string text = "";
			if (kyy == 0 || kyy > kyw.Length)
			{
				kyy = kyw.Length;
			}
			for (int i = kyx; i < kyy; i++)
			{
				text += $"{kyw[i]:x2}";
			}
			return text;
		}

		private void ListenControlConnect()
		{
			try
			{
				while (true)
				{
					TcpClient tcpClient = m_mcControlListener.AcceptTcpClient();
					StaticClient.send($"[Details-RPC]Connected:{tcpClient.Client.RemoteEndPoint}", "");
					if (tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0] != "127.0.0.1")
					{
						tcpClient.Close();
						continue;
					}
					NetworkStream stream = tcpClient.GetStream();
					Client = tcpClient;
					Reader = new BinaryReader(stream);
					Writer = new BinaryWriter(stream);
					tm.Create(delegate
					{
						OnRecvControlData();
					});
				}
			}
			catch (Exception ex)
			{
				PrintInfo(ex.Message);
				CloseControlConnection();
			}
		}

		private void OnRecvControlData()
		{
			StaticClient.send($"[Details-RPC]", "ChaMessage");
			while (!m_isNormalExit)
			{
				int num;
				byte[] array;
				try
				{
					num = Reader.ReadUInt16();
					array = Reader.ReadBytes(num);
				}
				catch (Exception ex)
				{
					PrintInfo(ex.Message);
					if (!m_isNormalExit)
					{
						CloseGameCleaning();
					}
					return;
				}
				PrintInfo($"[control] From MC：{Others.ByteToString(array, 0, num)}");
				HandleMcControlMessage(array);
			}
		}

		private void HandleMcControlMessage(byte[] message)
		{
			if (NeedChaCha)
			{
				rec_ccx.Process(message);
			}
			ushort num = BitConverter.ToUInt16(message, 0);
			byte[] paramlist = message.Skip(2).Take(message.Length - 2).ToArray();
			PrintInfo($"cmd命令：{num:x4}");
			if (!m_isLaunchIdxReady && num == 261)
			{
				m_launchIdx = BitConverter.ToInt16(message, 2);
				o();
			}
			try
			{
				SocketCallbackFuc.CallBack(num, paramlist);
			}
			catch (Exception ex)
			{
				StaticClient.send($"[MainError] 游戏Launcher报文通信出错", ex.ToString());
			}
		}

		private void o()
		{
			foreach (KeyValuePair<string, Action> item in q)
			{
				item.Value();
			}
		}

		public void AuthServer(byte[] re)
		{
			SendControlData(SimplePack.Pack((ushort)1031, serverIP, serverPort, RoleName, false));
		}

		public void SendControlData(byte[] message)
		{
			if (NeedChaCha)
			{
				send_ccx.Process(message);
			}
			byte[] array = BitConverter.GetBytes((ushort)message.Length).Concat(message).ToArray();
			StaticClient.send($"[Send] {Others.ByteToString(array, 0, Math.Min(64, array.Length))}", "");
			try
			{
				Writer.Write(array);
				Writer.Flush();
			}
			catch (Exception ex)
			{
				StaticClient.send($"[MainError] ", ex.Message);
				StaticClient.send($"[MainError]SendAuthenticationData Error!", "");
			}
		}

		public void CloseControlConnection()
		{
			try
			{
				m_mcControlListener?.Stop();
				m_mcControlListener = null;
			}
			catch (Exception ex)
			{
				StaticClient.send($"[MainWarning] ", ex.Message);
			}
		}

		public void RegisterReadyAction(string name, Action action)
		{
			ReadyActions.Add(name, action);
			PrintInfo($"就绪处理[{name}]注册成功:{action.Method}");
		}

		public void RegisterCloseAction(string name, Action action)
		{
			CloseActions.Add(name, action);
			PrintInfo($"关闭处理[{name}]注册成功:{action.Method}");
		}

		private void ClearActions()
		{
			ReadyActions.Clear();
			CloseActions.Clear();
		}

		public void CloseGameCleaning()
		{
			StaticClient.send($"[Warning] ", "RPC CloseGameCleaning");

			try
			{
				ExecuteCloseActions();
				ClearActions();
			}
			catch (Exception ex)
			{
				PrintInfo(ex.Message);
			}
			m_isLaunchIdxReady = false;
			m_launchIdx = -1;
		}

		private void ExecuteCloseActions()
		{
			foreach (KeyValuePair<string, Action> closeAction in CloseActions)
			{
				PrintInfo($"执行关闭处理函数: {closeAction.Key}");
				closeAction.Value();
			}
		}

		private void ExecuteReadyActions()
		{
			foreach (KeyValuePair<string, Action> readyAction in ReadyActions)
			{
				PrintInfo($"执行就绪处理函数: {readyAction.Key}");
				readyAction.Value();
			}
		}

		private void PrintInfo(string v)
		{
			Call.Log = Call.Log + "[RPC]" + v + "\r\n";
			Console.WriteLine(v);
		}

		private void Close()
		{
			Reader?.Close();
			Writer?.Close();
			Client?.Close();
			CloseControlConnection();
		}

		public void NormalExit()
		{
			m_isNormalExit = true;
			Close();
		}

		public string RpcToString()
		{
			string text = "=============================\n";
			string text2 = text;
			text = text2 + Client;
			string text3 = text;
			text = text3 + Reader;
			string text4 = text;
			text = text4 + Writer;
			text += j;
			return text + "=============================\n";
		}

		private void HandShake(byte[] iyl)
		{
			byte[] message = Tool.a((ushort)512, "i'am wpflauncher");
			SendControlData(message);
			StaticClient.send($"[Send] ", "i'am wpflauncher");
		}
	}
}
