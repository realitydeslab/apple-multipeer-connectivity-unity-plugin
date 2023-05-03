using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace HoloInteractive.iOS.MultipeerConnectivity
{
    public struct PendingInvitation
    {
        public MCPeerID PeerID;

        public NSData Context;

        public InvitationHandler InvitationHandler;
    }

    public class MCNearbyServiceAdvertiser : IDisposable
    {
        IntPtr m_Ptr;

        List<PendingInvitation> m_PendingInvitations;

        static Dictionary<IntPtr, MCNearbyServiceAdvertiser> s_AdvertiserInstances = new();

        public bool Created => m_Ptr != IntPtr.Zero;

        public List<PendingInvitation> PendingInvitations => m_PendingInvitations;

        public MCNearbyServiceAdvertiser(MCPeerID peerID, Dictionary<string, string> discoveryInfo, string serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (discoveryInfo == null)
            {
                using (NSString serviceType_Native = new(serviceType))
                {
                    m_Ptr = InitWithPeer(peerID, IntPtr.Zero, serviceType_Native);
                    s_AdvertiserInstances[m_Ptr] = this;
                    RegisterCallbacks(m_Ptr, OnDidReceiveInvitationFromPeerCallback);
                }
            }
            else
            {
                using (NSDictionary_NSString discoveryInfo_Native = new(discoveryInfo))
                using (NSString serviceType_Native = new(serviceType))
                {
                    m_Ptr = InitWithPeer(peerID, discoveryInfo_Native.NativePtr, serviceType_Native);
                    s_AdvertiserInstances[m_Ptr] = this;
                    RegisterCallbacks(m_Ptr, OnDidReceiveInvitationFromPeerCallback);
                }
            }
        }

        public void StartAdvertisingPeer()
        {
            m_PendingInvitations = new();
            StartAdvertisingPeer(m_Ptr);
        }

        public void StopAdvertisingPeer()
        {
            StopAdvertisingPeer(m_Ptr);
        }

        public void HandleInvitation(InvitationHandler invitationHandler, bool accept, MCSession session)
        {
            HandleInvitation_Native(invitationHandler, accept, session.NativePtr);
            ReleasePendingInvitation(invitationHandler);
        }

        public void Dispose()
        {
            if (m_Ptr != IntPtr.Zero)
            {
                s_AdvertiserInstances.Remove(m_Ptr);
                ReleasePendingInvitations();
                NativeApi.CFRelease(ref m_Ptr);
                m_Ptr = IntPtr.Zero;
            }
        }

        private void ReleasePendingInvitation(InvitationHandler invitationHandler)
        {
            foreach (var pendingInvitation in m_PendingInvitations)
            {
                if (pendingInvitation.InvitationHandler.Equals(invitationHandler))
                {
                    pendingInvitation.PeerID.Dispose();
                    pendingInvitation.Context.Dispose();
                    pendingInvitation.InvitationHandler.Dispose();
                    m_PendingInvitations.Remove(pendingInvitation);
                    return;
                }
            }
        }

        private void ReleasePendingInvitations()
        {
            foreach (var pendingInvitation in m_PendingInvitations)
            {
                pendingInvitation.PeerID.Dispose();
                pendingInvitation.Context.Dispose();
                pendingInvitation.InvitationHandler.Dispose();
            }
            m_PendingInvitations.Clear();
        }

        public event Action<MCPeerID, NSData, InvitationHandler> OnDidReceiveInvitationFromPeer;

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerNearbyServiceAdvertiser_initWithPeerAndDiscoveryInfo")]
        static extern IntPtr InitWithPeer(MCPeerID peerID, IntPtr discoveryInfo, NSString serviceType);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerNearbyServiceAdvertiser_startAdvertisingPeer")]
        static extern void StartAdvertisingPeer(IntPtr self);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerNearbyServiceAdvertiser_stopAdvertisingPeer")]
        static extern void StopAdvertisingPeer(IntPtr self);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerNearbyServiceAdvertiser_registerCallbacks")]
        static extern void RegisterCallbacks(IntPtr self, Action<IntPtr, IntPtr, IntPtr, IntPtr> onDidReceiveInvitationFromPeer);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_MultipeerNearbyServiceAdvertiser_handleInvitation")]
        static extern void HandleInvitation_Native(InvitationHandler invitationHandler, bool accept, IntPtr session);

        [AOT.MonoPInvokeCallback(typeof(Action<IntPtr, IntPtr, IntPtr, IntPtr>))]
        static void OnDidReceiveInvitationFromPeerCallback(IntPtr advertiserPtr, IntPtr peerIDPtr, IntPtr contextPtr, IntPtr invitationHandlerPtr)
        {
            if (s_AdvertiserInstances.TryGetValue(advertiserPtr, out MCNearbyServiceAdvertiser advertiser))
            {
                PendingInvitation pendingInvitation = new PendingInvitation
                {
                    PeerID = new MCPeerID(peerIDPtr),
                    Context = new NSData(contextPtr),
                    InvitationHandler = new InvitationHandler(invitationHandlerPtr)
                };
                advertiser.m_PendingInvitations.Add(pendingInvitation);
                advertiser.OnDidReceiveInvitationFromPeer.Invoke(pendingInvitation.PeerID, pendingInvitation.Context, pendingInvitation.InvitationHandler);
            }
        }
    }
}
