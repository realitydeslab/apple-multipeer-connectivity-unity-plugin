// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;

namespace HoloInteractive.iOS.MultipeerConnectivity.Samples.ARCollaborativeSession
{
    public class CollaborativeSession : MonoBehaviour
    {
        MCPeerID m_PeerID;

        MCSession m_MCSession;

        MCNearbyServiceAdvertiser m_Advertiser;

        MCNearbyServiceBrowser m_Browser;

        public MCSession MCSession => m_MCSession;

        public MCNearbyServiceAdvertiser Advertiser => m_Advertiser;

        public MCNearbyServiceBrowser Browser => m_Browser;

        ARSession m_ARSession;

        ARKitSessionSubsystem m_Subsystem;

        const string SERVICE_TYPE = "unity-mc";

        public event Action OnInitialized;

        private void Start()
        {
            m_ARSession = FindObjectOfType<ARSession>();
            m_Subsystem = m_ARSession.subsystem as ARKitSessionSubsystem;
            if (!ARKitSessionSubsystem.supportsCollaboration || m_Subsystem == null)
            {
                Debug.LogError("Collaborative sessions require iOS 13.");
                return;
            }
            m_Subsystem.collaborationRequested = true;

            m_PeerID = new($"yifei{UnityEngine.Random.Range(1, 100)}");
            m_MCSession = new(m_PeerID);
            m_Advertiser = new(m_PeerID, null, SERVICE_TYPE);
            m_Browser = new(m_PeerID, SERVICE_TYPE);

            m_Advertiser.OnDidReceiveInvitationFromPeer += OnDidReceiveInvitationFromPeer;
            m_Browser.OnFoundPeer += OnFoundPeer;
            m_MCSession.OnDidReceiveData += OnDidReceiveData;

            OnInitialized?.Invoke();

            m_Advertiser.StartAdvertisingPeer();
            m_Browser.StartBrowsingForPeers();
        }

        private void Update()
        {
            while (m_Subsystem.collaborationDataCount > 0)
            {
                using (var collaborationData = m_Subsystem.DequeueCollaborationData())
                {
                    if (m_MCSession.ConnectedPeers.Count == 0)
                    {
                        continue;
                    }

                    using (var serializedData = collaborationData.ToSerialized())
                    using (var data = NSData.CreateWithBytesNoCopy(serializedData.bytes))
                    {
                        _ = m_MCSession.SendDataToAllPeers(data, collaborationData.priority == ARCollaborationDataPriority.Critical ?
                            MCSessionSendDataMode.Reliable : MCSessionSendDataMode.Unreliable);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            m_PeerID.Dispose();
            m_MCSession.Dispose();
            m_Advertiser.Dispose();
            m_Browser.Dispose();
        }

        private void OnDidReceiveInvitationFromPeer(MCPeerID peerID, NSData context, InvitationHandler invitationHandler)
        {
            m_Advertiser.HandleInvitation(invitationHandler, true, m_MCSession);
        }

        private void OnFoundPeer(MCPeerID peerID, Dictionary<string ,string> discoveryInfo)
        {
            m_Browser.InvitePeer(peerID, m_MCSession);
        }

        private void OnDidReceiveData(NSData data, MCPeerID peerID)
        {
            using (var collaborationData = new ARCollaborationData(data.Bytes))
            {
                if (collaborationData.valid)
                {
                    m_Subsystem.UpdateWithCollaborationData(collaborationData);
                }
            }
        }
    }
}
