#define DEBUG
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static NeteaseLogin.NeteaseLogin.Memory.WriteMethod;

namespace NeteaseLogin
{
    public class Call
    {
        private static int m_hDLL = 0;

        private const string DLLNAEM = "api-ms-win-crt-utility-l1-1-1.dll";

        private static string HKEY_BASE = "SOFTWARE\\Netease\\MCLauncher";

        public static string Log = "";

        public static int FindBeginAddr = 0;

        public static int Check = 0;
        public static int LoginFinished = 1;

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_51258412ae7f26a1cbfcfc4c52b215cb")]
        public unsafe static extern IntPtr NewChaCha(uint dlz, byte* dma);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_b71fa6b924744e4fdf5091006d3ac0c8")]
        public static extern void DeleteChaCha(IntPtr dmd);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_b79c5024733866f2a0d68ae29f94b595")]
        public unsafe static extern void ChaChaProcess(IntPtr dmf, byte* dmg, uint dmh);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ff487957b05f3b54712db300a8687189")]
        public unsafe static extern uint ParseUUID_(bool dml, byte* dmm, uint dmn);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public unsafe static extern int HttpEncrypt(byte* dmv, byte* dmw, out IntPtr dmx, out IntPtr dmy);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public unsafe static extern int ComputeSilence(byte* dmz, byte* dna, byte* dnb, out IntPtr dnc, int dnd);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int ParseLoginResponse(IntPtr dne, int dnf, out IntPtr dng, out IntPtr dnh);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int ComputeDynamicToken(IntPtr dni, int dnj, IntPtr dnk, int dnl, out IntPtr dnm);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public unsafe static extern int GetToken(byte* dnn);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int GetH5Token(out IntPtr dno, out IntPtr dnp);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int HttpDecrypt(IntPtr dnq, int dnr, out IntPtr dns, out IntPtr dnt);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public unsafe static extern void AuthLogin(uint dnu, byte* dnv, int dnw);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void AuthLogout();

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void AuthMakeAuthentication(ulong dnx, int dny, uint dnz, int doa);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void AuthCloseAuthentication();

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public unsafe static extern void AuthSend(int dob, byte* dog);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_1b559cbb2d1a2c82336de16a49adc867")]
        private unsafe static extern bool Parse2_(byte* dod, uint doe, byte* dof, uint dog);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl)]
        private unsafe static extern void _aee7739eac9adbadc28f90f6d9ad984c(byte* dol, uint dom);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern void FreeMemory(IntPtr dop);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ConnectToAuthSrv(string doq, int dor);

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsAuthSrvConnected();

        [DllImport("api-ms-win-crt-utility-l1-1-1.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "AuthSend1")]
        public unsafe static extern void AuthSend(byte* dos);

        public static int Init()
        {
            FreeMemory((IntPtr)0);
            _Crack();
            return 1;
        }

        public static int GetModuleHandle(string Modulename)
        {
            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                if (module.ModuleName == Modulename)
                {
                    return (int)module.BaseAddress;
                }
            }
            return 0;
        }


        public static int Crack_Pcyc(int processId)
        {
            ProcessModule64[] wOW64Modules = ProcessModules.getWOW64Modules(processId);
            foreach (ProcessModule64 processModule in wOW64Modules)
            {
                if (Path.GetFileName(processModule.FileName) == "api-ms-win-crt-utility-l1-1-1.dll")
                {
                    int num = x64_写内存字节集(processId, (ulong)(processModule.BaseAddress + 453568), new byte[1] { 63 }, 10uL);
                    num += x64_写内存字节集(processId, (ulong)(processModule.BaseAddress + 415912), new byte[1] { 63 }, 10uL);
                    num += x64_写内存字节集(processId, (ulong)(processModule.BaseAddress + 445092), new byte[1] { 63 }, 10uL);
                    num += x64_写内存字节集(processId, (ulong)(processModule.BaseAddress + 416288), new byte[1] { 63 }, 17uL);
                    num += x64_写内存字节集(processId, (ulong)(processModule.BaseAddress + 418000), new byte[1] { 63 }, 19uL);
                    num += x64_写内存字节集(processId, (ulong)(processModule.BaseAddress + 419560), new byte[1] { 63 }, 52uL);
                    num += x64_写内存字节集(processId, (ulong)(processModule.BaseAddress + 420064), new byte[1] { 63 }, 43uL);
                    return num + x64_写内存字节集(processId, (ulong)(processModule.BaseAddress + 266053), new byte[5] { 176, 1, 195, 36, 16 }, 5uL);
                }
            }
            return -1;
        }


        public static void _Crack()
        {
            Api.WriteProcessMemory(Api.GetCurrentProcess(), (IntPtr)GetModuleHandle("api-ms-win-crt-utility-l1-1-1.dll") + 235232, new uint[]
{
                Convert.ToUInt32("B001C3", 16)
}, 3U, (IntPtr)0);
        }

        public static byte[] DesDecrypt(byte[] input, string key, CipherMode mode = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7)
        {
            if (key.Length > 8)
            {
                key = key.Substring(0, 8);
            }
            if (key.Length != 8)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < 8 - key.Length; i++)
                {
                    stringBuilder.Append("0");
                }
                key += stringBuilder.ToString();
            }
            try
            {
                using DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
                dESCryptoServiceProvider.Mode = mode;
                dESCryptoServiceProvider.Padding = padding;
                dESCryptoServiceProvider.Key = Encoding.UTF8.GetBytes(key);
                dESCryptoServiceProvider.IV = Encoding.UTF8.GetBytes(key);
                using MemoryStream memoryStream = new MemoryStream();
                using CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
                cryptoStream.Write(input, 0, input.Length);
                cryptoStream.FlushFinalBlock();
                memoryStream.ToArray();
                return memoryStream.ToArray();
            }
            catch
            {
                return new byte[0];
            }
        }

        public static byte[] DesEncrypt(string input, string key, CipherMode mode = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7)
        {
            if (key.Length > 8)
            {
                key = key.Substring(0, 8);
            }
            if (key.Length != 8)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < 8 - key.Length; i++)
                {
                    stringBuilder.Append("0");
                }
                key += stringBuilder.ToString();
            }
            try
            {
                using DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                dESCryptoServiceProvider.Mode = mode;
                dESCryptoServiceProvider.Padding = padding;
                dESCryptoServiceProvider.Key = Encoding.UTF8.GetBytes(key);
                dESCryptoServiceProvider.IV = Encoding.UTF8.GetBytes(key);
                using MemoryStream memoryStream = new MemoryStream();
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                }
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new byte[0];
            }
        }

        public static void CanLaunch_Authentication()
        {
            Check = 1;
        }

        public static void killMe(int pid = -1)
        {
            if (pid <= 0)
            {
                pid = Process.GetCurrentProcess().Id;
            }
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string text = "taskkill /PID " + pid;
            process.StandardInput.WriteLine(text + "&exit");
            process.StandardInput.AutoFlush = true;
            process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            Process.GetCurrentProcess().Kill();
        }

        public static string randomStr(int len, string[] arr = null)
        {
            if (arr == null || arr.Length <= 1)
            {
                arr = new string[16]
                {
                    "a", "b", "c", "d", "e", "f", "0", "1", "2", "3",
                    "4", "5", "6", "7", "8", "9"
                };
            }
            string text = "";
            for (int i = 0; i < len; i++)
            {
                text += arr[new Random(new Random(Guid.NewGuid().GetHashCode()).Next(0, 100)).Next(arr.Length - 1)];
            }
            return text;
        }

        public static string getGamePath()
        {
            try
            {
                return (string)Registry.CurrentUser.OpenSubKey(HKEY_BASE).GetValue("DownloadPath");
            }
            catch (Exception)
            {
                return "C:\\MCLDownload";
            }
        }

        public static string GetToken(string dpi, string dpj)
        {
            byte[] array = null;
            byte[] array2 = null;
            int num = 0;
            int num2 = 0;
            IntPtr intPtr = IntPtr.Zero;
            IntPtr intPtr2 = IntPtr.Zero;
            if (!string.IsNullOrEmpty(dpi))
            {
                if (!dpi.StartsWith("/"))
                {
                    dpi = "/" + dpi;
                }
                array = Encoding.UTF8.GetBytes(dpi);
                num2 = Marshal.SizeOf((object)array[0]) * array.Length;
                intPtr2 = Marshal.AllocHGlobal(num2);
            }
            if (!string.IsNullOrEmpty(dpj))
            {
                array2 = Encoding.UTF8.GetBytes(dpj);
                num = Marshal.SizeOf((object)array2[0]) * array2.Length;
                intPtr = Marshal.AllocHGlobal(num);
            }
            string result = string.Empty;
            try
            {
                if (array2 != null)
                {
                    Marshal.Copy(array2, 0, intPtr, array2.Length);
                }
                if (array != null)
                {
                    Marshal.Copy(array, 0, intPtr2, array.Length);
                }
                IntPtr dnm = IntPtr.Zero;
                int num3 = ComputeDynamicToken(intPtr2, num2, intPtr, num, out dnm);
                if (num3 != 0 && dnm != IntPtr.Zero)
                {
                    byte[] array3 = new byte[num3];
                    Marshal.Copy(dnm, array3, 0, num3);
                    FreeMemory(dnm);
                    result = Encoding.UTF8.GetString(array3);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (intPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(intPtr);
                }
                if (intPtr2 != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(intPtr2);
                }
            }
            return result;
        }

        public unsafe static byte[] _HttpEncrypt_(string dpb, string dpc, out string dpd)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(dpc);
            byte[] result;
            if (string.IsNullOrEmpty(dpc))
            {
                dpd = string.Empty;
                result = bytes;
            }
            else
            {
                byte[] bytes2 = Encoding.UTF8.GetBytes(dpb);
                fixed (byte* ptr = &bytes2[0])
                {
                    byte* dmv = ptr;
                    fixed (byte* ptr2 = &bytes[0])
                    {
                        byte* dmw = ptr2;
                        IntPtr dmx = IntPtr.Zero;
                        IntPtr dmy = IntPtr.Zero;
                        int num = HttpEncrypt(dmv, dmw, out dmx, out dmy);
                        byte[] array = new byte[num];
                        byte[] array2 = new byte[16];
                        if (num != 0 || dmx != IntPtr.Zero)
                        {
                            Marshal.Copy(dmx, array, 0, num);
                            FreeMemory(dmx);
                        }
                        if (dmy != IntPtr.Zero)
                        {
                            Marshal.Copy(dmy, array2, 0, array2.Length);
                            FreeMemory(dmy);
                        }
                        dpd = Encoding.UTF8.GetString(array2);
                        result = array;
                    }
                }
            }
            return result;
        }

        public static string _ParseLoginResponse_(byte[] dpg, out string dph)
        {
            if (dpg == null || dpg.Length == 0)
            {
                dph = string.Empty;
                return string.Empty;
            }
            int cb = Marshal.SizeOf((object)dpg[0]) * dpg.Length;
            IntPtr intPtr = Marshal.AllocHGlobal(cb);
            string result = string.Empty;
            dph = string.Empty;
            int dnf = dpg.Length;
            try
            {
                Marshal.Copy(dpg, 0, intPtr, dpg.Length);
                IntPtr dng = IntPtr.Zero;
                IntPtr dnh = IntPtr.Zero;
                int num = ParseLoginResponse(intPtr, dnf, out dng, out dnh);
                if (num != 0 && dng != IntPtr.Zero)
                {
                    byte[] array = new byte[num];
                    Marshal.Copy(dng, array, 0, num);
                    FreeMemory(dng);
                    result = Encoding.UTF8.GetString(array);
                }
                if (dnh != IntPtr.Zero)
                {
                    byte[] array2 = new byte[16];
                    Marshal.Copy(dnh, array2, 0, array2.Length);
                    FreeMemory(dnh);
                    dph = Encoding.UTF8.GetString(array2);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(intPtr);
            }
            return result;
        }

        public unsafe static uint ParseUUID(bool cng, byte* cnh, uint cni)
        {
            return ParseUUID_(cng, cnh, cni);
        }

        public unsafe static bool Parse2(byte* dpa, byte* dpb)
        {
            return Parse2_(dpa, 32u, dpb, 16u);
        }

        public static string _HttpDecrypt_(byte[] dpe, out string dpf)
        {
            if (dpe == null || dpe.Length == 0)
            {
                dpf = string.Empty;
                return string.Empty;
            }
            int cb = Marshal.SizeOf((object)dpe[0]) * dpe.Length;
            IntPtr intPtr = Marshal.AllocHGlobal(cb);
            string result = string.Empty;
            dpf = string.Empty;
            int dnr = dpe.Length;
            try
            {
                Marshal.Copy(dpe, 0, intPtr, dpe.Length);
                IntPtr dns = IntPtr.Zero;
                IntPtr dnt = IntPtr.Zero;
                int num = HttpDecrypt(intPtr, dnr, out dns, out dnt);
                if (num != 0 && dns != IntPtr.Zero)
                {
                    byte[] array = new byte[num];
                    Marshal.Copy(dns, array, 0, num);
                    FreeMemory(dns);
                    result = Encoding.UTF8.GetString(array);
                }
                if (dnt != IntPtr.Zero)
                {
                    byte[] array2 = new byte[16];
                    Marshal.Copy(dnt, array2, 0, array2.Length);
                    FreeMemory(dnt);
                    dpf = Encoding.UTF8.GetString(array2);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(intPtr);
            }
            return result;
        }
    }
}
