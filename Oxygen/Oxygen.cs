﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxygenVPN.Controllers;
using OxygenVPN.Forms;
using OxygenVPN.Utils;

namespace OxygenVPN {
    public static class Oxygen {
        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args) {
            // 创建互斥体防止多次运行
            using (var mutex = new Mutex(false, "Global\\Oxygen")) {
                // 设置当前目录
                Directory.SetCurrentDirectory(Global.OxygenVPNDir);
                Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process) + ";" + Path.Combine(Global.OxygenVPNDir, "bin"), EnvironmentVariableTarget.Process);

                // 预创建目录
                var directories = new[] { "mode", "data", "i18n", "logging" };
                foreach (var item in directories) {
                    if (!Directory.Exists(item)) {
                        Directory.CreateDirectory(item);
                    }
                }

                // 加载配置
                Configuration.Load();

                // 加载语言
                i18N.Load(Global.Settings.Language);

                // 检查是否已经运行
                if (!mutex.WaitOne(0, false)) {
                    OnlyInstance.Send(OnlyInstance.Commands.Show);
                    Logging.Info("Wakeup Singletone");

                    // 退出进程
                    Environment.Exit(1);
                }

                // 清理上一次的日志文件，防止淤积占用磁盘空间
                if (Directory.Exists("logging")) {
                    var directory = new DirectoryInfo("logging");

                    foreach (var file in directory.GetFiles()) {
                        file.Delete();
                    }

                    foreach (var dir in directory.GetDirectories()) {
                        dir.Delete(true);
                    }
                }

                Logging.Info($"Version: {UpdateChecker.Owner}/{UpdateChecker.Repo}@{UpdateChecker.Version}");
                Task.Run(() => {
                    Logging.Info($"Exe SHA256: {Utils.Utils.SHA256CheckSum(Application.ExecutablePath)}");
                });
                Task.Run(() => {
                    Logging.Info("Singletone");
                    OnlyInstance.Server();
                });

                // 绑定错误捕获
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += Application_OnException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(Global.MainForm = new MainForm());
            }
        }

        public static void Application_OnException(object sender, ThreadExceptionEventArgs e) {
            Logging.Error(e.Exception.ToString());
            Logging.ShowLogForm();
        }
    }
}