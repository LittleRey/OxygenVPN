﻿using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace OxygenVPN.Controllers {
    public class DNSController : Controller {
        public DNSController() {
            Name = "DNS Service";
            RedirectStd = false;
        }

        /// <summary>
        ///     启动DNS服务
        /// </summary>
        /// <returns></returns>
        public bool Start() {
            if (!aiodns_dial(Encoding.UTF8.GetBytes(Path.GetFullPath("bin\\china_site_list")),
                Encoding.UTF8.GetBytes("223.5.5.5:53"),
                Encoding.UTF8.GetBytes("1.1.1.1:53"))
            )
                return false;
            return
                aiodns_init();
        }

        public override void Stop() {
            aiodns_free();
        }

        #region NativeMethods

        [DllImport("aiodns.bin", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool aiodns_dial(byte[] chinacon, byte[] chinadns, byte[] otherdns);

        [DllImport("aiodns.bin", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool aiodns_init();

        [DllImport("aiodns.bin", CallingConvention = CallingConvention.Cdecl)]
        public static extern void aiodns_free();

        #endregion
    }
}