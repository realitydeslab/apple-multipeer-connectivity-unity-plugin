// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace HoloInteractive.iOS.MultipeerConnectivity
{
    public struct FoundPeer
    {
        public MCPeerID PeerID;
        public Dictionary<string, string> DiscoveryInfo;
    }

    public class MCNearbyServiceBrowser : IDisposable
    {
        IntPtr m_Ptr;

        List<FoundPeer> m_FoundPeers = new();

        // A static Dictionary to store the mapping of native pointers to C# objects
        static Dictionary<IntPtr, MCNearbyServiceBrowser> s_BrowserInstances = new();

        public bool Created => m_Ptr != IntPtr.Zero;

        public bool IsBrowsingForPeers = false;

        public List<FoundPeer> FoundPeers => m_FoundPeers;

        public MCNearbyServiceBrowser(MCPeerID peerID, string serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            using (NSString serviceType_Native = new(serviceType))
            {
                m_Ptr = InitWithPeer(peerID, serviceType_Native);
                RegisterCallbacks(m_Ptr, OnFoundPeerCallback, OnLostPeerCallback, OnDidNotStartBrowsingForPeersCallback);

                s_BrowserInstances[m_Ptr] = this;
            }
        }

        public void StartBrowsingForPeers()
        {
            StartBrowsingForPeers(m_Ptr);
            IsBrowsingForPeers = true;
        }

        public void StopBrowsingForPeers()
        {
            StopBrowsingForPeers(m_Ptr);
            IsBrowsingForPeers = false;
            ReleaseFoundPeers();
        }

        public void InvitePeer(MCPeerID peerID, MCSession session, NSData context = new NSData(), double timeout = 30)
        {
            InvitePeer(m_Ptr, peerID, session.NativePtr, context, timeout);
            ReleaseFoundPeer(peerID);
        }

        private void ReleaseFoundPeer(MCPeerID peerID)
        {
            foreach (var foundPeer in m_FoundPeers)
            {
                if (foundPeer.PeerID.Equals(peerID))
                {
                    foundPeer.PeerID.Dispose();
                    m_FoundPeers.Remove(foundPeer);
                    return;
                }
            }
        }

        private void ReleaseFoundPeers()
        {
            foreach (var foundPeer in m_FoundPeers)
            {
                foundPeer.PeerID.Dispose();
            }
            m_FoundPeers.Clear();
        }

        public void Dispose()
        {
            if (m_Ptr != IntPtr.Zero)
            {
                StopBrowsingForPeers();
                // Remove the mapping from the dictionary
                s_BrowserInstances.Remove(m_Ptr);
                NativeApi.CFRelease(ref m_Ptr);
                m_Ptr = IntPtr.Zero;
            }
        }

        public event Action<MCPeerID, Dictionary<string, string>> OnFoundPeer;
        public event Action<MCPeerID> OnLostPeer;
        public event Action OnDidNotStartBrowsingForPeers;

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerNearbyServiceBrowser_initWithPeer")]
        static extern IntPtr InitWithPeer(MCPeerID peerID, NSString serviceType);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerNearbyServiceBrowser_startBrowsingForPeers")]
        static extern void StartBrowsingForPeers(IntPtr self);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerNearbyServiceBrowser_stopBrowsingForPeers")]
        static extern void StopBrowsingForPeers(IntPtr self);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerNearbyServiceBrowser_registerCallbacks")]
        static extern void RegisterCallbacks(IntPtr self,
                                             Action<IntPtr, IntPtr, IntPtr> onFoundPeerCallback,
                                             Action<IntPtr, IntPtr> onLostPeerCallback,
                                             Action<IntPtr, NSError> onDidNotStartBrowsingForPeersCallback);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerNearbyServiceBrowser_invitePeer")]
        static extern void InvitePeer(IntPtr self, MCPeerID peerID, IntPtr session, NSData context, double timeout);

        [AOT.MonoPInvokeCallback(typeof(Action<IntPtr, IntPtr, IntPtr>))]
        static void OnFoundPeerCallback(IntPtr browserPtr, IntPtr peerIDPtr, IntPtr discoveryInfoPtr)
        {
            if (s_BrowserInstances.TryGetValue(browserPtr, out MCNearbyServiceBrowser browser))
            {
                FoundPeer foundPeer = new();
                foundPeer.PeerID = new(peerIDPtr);
                if (discoveryInfoPtr != IntPtr.Zero)
                {
                    using (NSDictionary_NSString discoveryInfo_Native = new(discoveryInfoPtr))
                    {
                        Dictionary<string, string> discoveryInfo = discoveryInfo_Native.ToDictionary();
                        foundPeer.DiscoveryInfo = discoveryInfo;
                        browser.m_FoundPeers.Add(foundPeer);
                        browser.OnFoundPeer?.Invoke(foundPeer.PeerID, discoveryInfo);
                    }
                }
                else
                {
                    browser.m_FoundPeers.Add(foundPeer);
                    browser.OnFoundPeer?.Invoke(foundPeer.PeerID, null);
                }
            }
        }

        [AOT.MonoPInvokeCallback(typeof(Action<IntPtr, IntPtr>))]
        static void OnLostPeerCallback(IntPtr browserPtr, IntPtr peerIDPtr)
        {
            if (s_BrowserInstances.TryGetValue(browserPtr, out MCNearbyServiceBrowser browser))
            {
                foreach (var foundPeer in browser.m_FoundPeers)
                {
                    if (foundPeer.PeerID.NativePtr == peerIDPtr)
                    {
                        browser.OnLostPeer?.Invoke(foundPeer.PeerID);
                        browser.m_FoundPeers.Remove(foundPeer);
                        foundPeer.PeerID.Dispose();

                        return;
                    }
                }
            }
        }

        [AOT.MonoPInvokeCallback(typeof(Action<IntPtr, NSError>))]
        static void OnDidNotStartBrowsingForPeersCallback(IntPtr browserPtr, NSError error)
        {
            if (s_BrowserInstances.TryGetValue(browserPtr, out MCNearbyServiceBrowser browser))
            {
                browser.OnDidNotStartBrowsingForPeers?.Invoke();
                error.Dispose();
            }
        }
    }
}
