﻿using System;
using System.Runtime.InteropServices;

namespace OxygenVPN {
    public static class Win32Native {
        [DllImport("User32", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
    }
}