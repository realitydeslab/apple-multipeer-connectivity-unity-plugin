// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;

namespace HoloInteractive.iOS.MultipeerConnectivity
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MCPeerID : IDisposable, IEquatable<MCPeerID>
    {
        IntPtr m_Ptr;

        internal MCPeerID(IntPtr existing) => m_Ptr = existing;

        public MCPeerID(string displayName)
        {
            NSString displayName_Native = new NSString(displayName);
            m_Ptr = InitWithDisplayName(displayName_Native);
            displayName_Native.Dispose();
        }

        public bool Created => m_Ptr != IntPtr.Zero;

        public string DisplayName
        {
            get
            {
                NSString displayName_Native = new(GetDisplayName(this));
                string displayName = displayName_Native.ToString();
                displayName_Native.Dispose();
                return displayName;
            }
        }

        public IntPtr NativePtr => m_Ptr;

        public void Dispose()
        {
            if (m_Ptr != IntPtr.Zero)
            {
                NativeApi.CFRelease(ref m_Ptr);
                m_Ptr = IntPtr.Zero;
            }
        }

        public bool Equals(MCPeerID other) => m_Ptr == other.m_Ptr;

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MCPeerID_initWithDisplayName")]
        static extern IntPtr InitWithDisplayName(NSString displayName);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MCPeerID_getDisplayName")]
        static extern IntPtr GetDisplayName(MCPeerID self);
    }
}
