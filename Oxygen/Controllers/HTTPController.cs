﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using OxygenVPN.Models;
using OxygenVPN.ServerEx.Socks5;
using OxygenVPN.Utils;

namespace OxygenVPN.Controllers {
    public class HTTPController : ModeController {
        public override bool TestNatRequired { get; } = false;

        public const string IEProxyExceptions = "localhost;127.*;10.*;172.16.*;172.17.*;172.18.*;172.19.*;172.20.*;172.21.*;172.22.*;172.23.*;172.24.*;172.25.*;172.26.*;172.27.*;172.28.*;172.29.*;172.30.*;172.31.*;192.168.*";

        /// <summary>
        ///     实例
        /// </summary>
        public PrivoxyController pPrivoxyController = new PrivoxyController();

        private string prevBypass, prevHTTP, prevPAC;
        private bool prevEnabled;

        public HTTPController() {
            Name = "HTTP";
        }

        /// <summary>
        ///     启动
        /// </summary>
        /// <param name="s">服务器</param>
        /// <param name="mode">模式</param>
        /// <returns>是否启动成功</returns>
        public override bool Start(Server s, Mode mode) {
            RecordPrevious();

            try {
                if (s.IsSocks5()) {
                    var server = (Socks5)s;
                    if (!string.IsNullOrWhiteSpace(server.Username) && !string.IsNullOrWhiteSpace(server.Password)) return false;

                    pPrivoxyController.Start(s, mode);
                } else {
                    pPrivoxyController.Start(s, mode);
                }

                if (mode.Type == 3) NativeMethods.SetGlobal($"127.0.0.1:{Global.Settings.HTTPLocalPort}", IEProxyExceptions);
            } catch (Exception e) {
                if (MessageBoxX.Show(i18N.Translate("Failed to set the system proxy, it may be caused by the lack of dependent programs. Do you want to jump to Oxygen VPN's official website to download dependent programs?"), confirm: true) == DialogResult.OK) Process.Start("https://github.com/kanskinson/OxygenVPN");

                Logging.Error("Failed to set system proxy" + e);
                return false;
            }

            return true;
        }

        private void RecordPrevious() {
            try {
                var registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings");
                if (registry == null)
                    throw new Exception();

                prevPAC = registry.GetValue("AutoConfigURL")?.ToString() ?? "";
                prevHTTP = registry.GetValue("ProxyServer")?.ToString() ?? "";
                prevBypass = registry.GetValue("ProxyOverride")?.ToString() ?? "";
                prevEnabled = registry.GetValue("ProxyEnable")?.Equals(1) ?? false; // HTTP Proxy Enabled

                if (prevHTTP == $"127.0.0.1:{Global.Settings.HTTPLocalPort}") {
                    prevEnabled = false;
                    prevHTTP = "";
                }

                if (prevPAC != "")
                    prevEnabled = true;
            } catch {
                prevEnabled = false;
                prevPAC = prevHTTP = prevBypass = "";
            }
        }

        /// <summary>
        ///     停止
        /// </summary>
        public override void Stop() {
            var tasks = new[]
            {
                Task.Factory.StartNew(pPrivoxyController.Stop),
                Task.Factory.StartNew(() =>
                {
                    if (prevEnabled)
                    {
                        if (prevHTTP != "")
                            NativeMethods.SetGlobal(prevHTTP, prevBypass);
                        if (prevPAC != "")
                            NativeMethods.SetURL(prevPAC);
                    }
                    else
                        NativeMethods.SetDIRECT();
                })
            };
            Task.WaitAll(tasks);
        }
    }
}