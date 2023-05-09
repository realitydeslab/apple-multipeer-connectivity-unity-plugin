// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using System;

namespace HoloInteractive.iOS.MultipeerConnectivity.Samples.Basics
{
    public class MCManager : MonoBehaviour
    {
        MCPeerID m_PeerID;

        MCSession m_Session;

        MCNearbyServiceAdvertiser m_Advertiser;

        MCNearbyServiceBrowser m_Browser;

        public string DisplayName = "yuchen";

        public bool IsInitialized => m_Session != null;

        public bool AutoConnectMode = true;

        public MCSession Session => m_Session;

        public MCNearbyServiceAdvertiser Advertiser => m_Advertiser;

        public MCNearbyServiceBrowser Browser => m_Browser;

        const string SERVICE_TYPE = "unity-mc";

        public event Action OnInitialized;

        public event Action OnShutdown;

        public void InitMCSession()
        {
            m_PeerID = new("yuchen");
            m_Session = new(m_PeerID);
            m_Advertiser = new(m_PeerID, null, SERVICE_TYPE);
            m_Browser = new(m_PeerID, SERVICE_TYPE);

            OnInitialized?.Invoke();

            if (AutoConnectMode)
            {
                m_Advertiser.OnDidReceiveInvitationFromPeer += OnDidReceiveInvitationFromPeer;
                m_Browser.OnFoundPeer += OnFoundPeer;

                m_Advertiser.StartAdvertisingPeer();
                m_Browser.StartBrowsingForPeers();
            }
        }

        public void Shutdown()
        {
            m_PeerID.Dispose();
            m_Session.Dispose();
            m_Advertiser.Dispose();
            m_Browser.Dispose();

            m_Session = null;
            m_Advertiser = null;
            m_Browser = null;

            OnShutdown?.Invoke();
        }

        public void OnChangeDisplayName(string newName)
        {
            DisplayName = newName;
        }

        public void OnChangeConnectMode(bool mode)
        {
            AutoConnectMode = mode;
        }

        private void OnDidReceiveInvitationFromPeer(MCPeerID peerID, NSData contect, InvitationHandler invitationHandler)
        {
            m_Advertiser.HandleInvitation(invitationHandler, true, m_Session);
        }

        private void OnFoundPeer(MCPeerID peerID, Dictionary<string, string> discoveryInfo)
        {
            m_Browser.InvitePeer(peerID, m_Session);
        }
    }
}
