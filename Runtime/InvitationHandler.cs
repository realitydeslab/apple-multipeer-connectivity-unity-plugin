using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace HoloInteractive.iOS.MultipeerConnectivity
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InvitationHandler : IDisposable, IEquatable<InvitationHandler>
    {
        IntPtr m_Ptr;

        public InvitationHandler(IntPtr existing) => m_Ptr = existing;

        public bool Created => m_Ptr != IntPtr.Zero;

        public void Dispose()
        {
            if (m_Ptr != IntPtr.Zero)
            {
                NativeApi.CFRelease(m_Ptr);
                m_Ptr = IntPtr.Zero;
            }
        }

        public bool Equals(InvitationHandler other) => m_Ptr == other.m_Ptr;
    }
}
