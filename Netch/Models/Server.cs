﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Netch.Models
{
    public class Server
    {
        /// <summary>
        ///     备注
        /// </summary>
        public string Remark;

        /// <summary>
        ///     组
        /// </summary>
        public string Group = "None";

        /// <summary>
        ///     代理类型（HTTP、HTTPS、Socks5、SS、SSR、VMess）
        /// </summary>
        public string Type;

        /// <summary>
        ///     倍率
        /// </summary>
        public double Rate = 1.0;

        /// <summary>
        ///     地址
        /// </summary>
        public string Hostname;

        /// <summary>
        ///     端口
        /// </summary>
        public int Port;

        /// <summary>
        ///     账号（HTTP、HTTPS、Socks5）
        /// </summary>
        public string Username;

        /// <summary>
        ///     密码（HTTP、HTTPS、Socks5、SS、SSR）
        /// </summary>
        public string Password;

        /// <summary>
        ///		用户 ID（VMess）
        /// </summary>
        public string UserID = String.Empty;

        /// <summary>
        ///		额外 ID（VMess）
        /// </summary>
        public int AlterID = 0;

        /// <summary>
        ///     加密方式（SS、SSR、VMess）
        /// </summary>
        public string EncryptMethod;

        /// <summary>
        ///     插件（SS）
        /// </summary>
        public string Plugin;

        /// <summary>
        ///     插件参数（SS）
        /// </summary>
        public string PluginOption;

        /// <summary>
        ///     协议（SSR）
        /// </summary>
        public string Protocol;

        /// <summary>
        ///     协议参数（SSR）
        /// </summary>
        public string ProtocolParam;

        /// <summary>
        ///     混淆（SSR）
        /// </summary>
        public string OBFS;

        /// <summary>
        ///     混淆参数（SSR）
        /// </summary>
        public string OBFSParam;

        /// <summary>
        ///		传输协议（VMess）
        /// </summary>
        public string TransferProtocol = "tcp";

        /// <summary>
        ///		伪装类型（VMess）
        /// </summary>
        public string FakeType = String.Empty;

        /// <summary>
        ///		伪装域名（VMess：HTTP、WebSocket、HTTP/2）
        /// </summary>
        public string Host = String.Empty;

        /// <summary>
        ///		传输路径（VMess：WebSocket、HTTP/2）
        /// </summary>
        public string Path = String.Empty;

        /// <summary>
        ///		QUIC 加密方式（VMess）
        /// </summary>
        public string QUICSecure = "none";

        /// <summary>
        ///		QUIC 加密密钥（VMess）
        /// </summary>
        public string QUICSecret = String.Empty;

        /// <summary>
        ///		TLS 底层传输安全（VMess）
        /// </summary>
        public bool TLSSecure = false;

        /// <summary>
        ///     延迟
        /// </summary>
        public int Delay = -1;

        /// <summary>
		///		获取备注
		/// </summary>
		/// <returns>备注</returns>
		public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(Remark))
            {
                Remark = $"{Hostname}:{Port}";
            }

            switch (Type)
            {
                case "Socks5":
                    return $"[S5] {Remark}";
                case "SS":
                    return $"[SS] {Remark}";
                case "SSR":
                    return $"[SR] {Remark}";
                case "VMess":
                    return $"[V2] {Remark}";
                default:
                    return "WTF";
            }
        }

        /// <summary>
        ///		测试延迟
        /// </summary>
        /// <returns>延迟</returns>
        public int Test()
        {
            try
            {
                var destination = Utils.DNS.Lookup(Hostname);
                if (destination == null)
                {
                    return Delay = -2;
                }

                var list = new Task<int>[3];
                for (int i = 0; i < 3; i++)
                {
                    list[i] = Task.Run<int>(() =>
                    {
                        try
                        {
                            using (var client = new Socket(SocketType.Stream, ProtocolType.Tcp))
                            {
                                var watch = new Stopwatch();
                                watch.Start();

                                var task = client.BeginConnect(new IPEndPoint(destination, Port), (result) =>
                                {
                                    watch.Stop();
                                }, 0);

                                if (task.AsyncWaitHandle.WaitOne(1000))
                                {
                                    return (int)watch.ElapsedMilliseconds;
                                }

                                return 1000;
                            }
                        }
                        catch (Exception)
                        {
                            return -4;
                        }
                    });
                }

                Task.WaitAll(list);

                var min = Math.Min(list[0].Result, list[1].Result);
                min = Math.Min(min, list[2].Result);
                return Delay = min;
            }
            catch (Exception)
            {
                return Delay = -4;
            }
        }
    }
}
