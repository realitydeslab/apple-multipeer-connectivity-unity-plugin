using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace HoloInteractive.iOS.MultipeerConnectivity
{
    public class MCSession : IDisposable, IEquatable<MCSession>
    {
        IntPtr m_Ptr;

        List<MCPeerID> m_ConnectingPeers = new();

        List<MCPeerID> m_ConnectedPeers = new();

        static Dictionary<IntPtr, MCSession> s_SessionInstances = new();

        public bool Created => m_Ptr != IntPtr.Zero;

        public IntPtr NativePtr => m_Ptr;

        public List<MCPeerID> ConnectedPeers => m_ConnectedPeers;

        public MCSession(MCPeerID peerID)
        {
            m_Ptr = InitWithPeer(peerID);
            RegisterCallbacks(m_Ptr, OnPeerDidChangeStateCallback, OnDidReceiveDataCallback);

            s_SessionInstances[m_Ptr] = this;
        }

        public bool SendData(NSData data, MCPeerID peerID, MCSessionSendDataMode mode)
        {
            return SendData(m_Ptr, data, peerID, (long)mode);
        }

        public bool SendDataToAllPeers(NSData data, MCSessionSendDataMode mode)
        {
            return SendDataToAllPeers(m_Ptr, data, (long)mode);
        }

        public void Disconnect()
        {
            Disconnect(m_Ptr);
        }

        public void Dispose()
        {
            if (m_Ptr != IntPtr.Zero)
            {
                s_SessionInstances.Remove(m_Ptr);
                ReleaseConnectingPeers();
                ReleaseConnectedPeers();
                NativeApi.CFRelease(ref m_Ptr);
                m_Ptr = IntPtr.Zero;
            }
        }

        private void ReleaseConnectingPeers()
        {
            foreach (var connectingPeer in m_ConnectingPeers)
            {
                connectingPeer.Dispose();
            }
        }

        private void ReleaseConnectedPeers()
        {
            foreach (var connectedPeer in m_ConnectedPeers)
            {
                connectedPeer.Dispose();
            }
        }

        public bool Equals(MCSession other) => m_Ptr == other.m_Ptr;

        public event Action<MCPeerID, MCSessionState> OnPeerDidChangeState;
        public event Action<NSData, MCPeerID> OnDidReceiveData;

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerSession_initWithPeer")]
        static extern IntPtr InitWithPeer(MCPeerID peerID);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerSession_registerCallbacks")]
        static extern void RegisterCallbacks(IntPtr self,
                                             Action<IntPtr, IntPtr, long> onPeerDidChangeStateCallback,
                                             Action<IntPtr, IntPtr, IntPtr> onDidReceiveDataCallback);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerSession_sendData")]
        static extern bool SendData(IntPtr self, NSData data, MCPeerID peerID, long mode);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerSession_sendDataToAllPeers")]
        static extern bool SendDataToAllPeers(IntPtr self, NSData data, long mode);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerSession_disconnect")]
        static extern void Disconnect(IntPtr self);

        [AOT.MonoPInvokeCallback(typeof(Action<IntPtr, IntPtr, long>))]
        static void OnPeerDidChangeStateCallback(IntPtr sessionPtr, IntPtr peerIDPtr, long stateIndex)
        {
            if (s_SessionInstances.TryGetValue(sessionPtr, out MCSession session))
            {
                MCPeerID peerID = new(peerIDPtr);
                MCSessionState state = (MCSessionState)stateIndex;
                switch (state)
                {
                    case MCSessionState.NotConnected:
                        session.OnPeerDidChangeState.Invoke(peerID, state);
                        if (session.m_ConnectingPeers.Contains(peerID))
                        {
                            session.m_ConnectingPeers.Remove(peerID);
                            peerID.Dispose();
                        }
                        else if (session.m_ConnectedPeers.Contains(peerID))
                        {
                            session.m_ConnectedPeers.Remove(peerID);
                            peerID.Dispose();
                        }
                        break;
                    case MCSessionState.Connecting:
                        session.m_ConnectingPeers.Add(peerID);
                        session.OnPeerDidChangeState.Invoke(peerID, state);
                        break;
                    case MCSessionState.Connected:
                        session.m_ConnectingPeers.Remove(peerID);
                        session.m_ConnectedPeers.Add(peerID);
                        session.OnPeerDidChangeState.Invoke(peerID, state);
                        break;
                }
            }
        }

        [AOT.MonoPInvokeCallback(typeof(Action<IntPtr, IntPtr, IntPtr>))]
        static void OnDidReceiveDataCallback(IntPtr sessionPtr, IntPtr dataPtr, IntPtr peerIDPtr)
        {
            using (NSData data = new(dataPtr))
            {
                if (s_SessionInstances.TryGetValue(sessionPtr, out MCSession session))
                {
                    MCPeerID peerID = new(peerIDPtr);
                    session.OnDidReceiveData.Invoke(data, peerID);
                }
            } 
        }
    }
}
