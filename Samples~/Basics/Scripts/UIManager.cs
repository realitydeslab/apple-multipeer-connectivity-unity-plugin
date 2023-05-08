// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace HoloInteractive.iOS.MultipeerConnectivity.Samples.Basics
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private MCManager m_MCManager;

        [SerializeField] private GameObject m_StartPage;

        [SerializeField] private GameObject m_MainPage;

        [SerializeField] private GameObject m_AdvertiserWindow;

        [SerializeField] private GameObject m_BrowserWindow;

        [SerializeField] private GameObject m_SessionWindow;

        [SerializeField] private GameObject m_StartAdvertisingBtn;

        [SerializeField] private GameObject m_StopAdvertisingBtn;

        [SerializeField] private GameObject m_StartBrowsingBtn;

        [SerializeField] private GameObject m_StopBrowsingBtn;

        [SerializeField] private RectTransform m_InvitationList;

        [SerializeField] private RectTransform m_FoundPeerList;

        [SerializeField] private RectTransform m_ConnectedPeerList;

        [SerializeField] private InvitationSlot m_InvitationSlotPrefab;

        [SerializeField] private FoundPeerSlot m_FoundPeerSlotPrefab;

        [SerializeField] private ConnectedPeerSlot m_ConnectedPeerSlotPrefab;

        [SerializeField] private GameObject m_MessagePrompt;

        private float m_MessagePromptLastShowTime;

        private float MESSAGE_PROMPT_DURATION = 3;

        private void Start()
        {
            m_MCManager.OnInitialized += OnInitialized;
            m_MCManager.OnShutdown += OnShutdown;

            m_StartPage.SetActive(true);
            m_MainPage.SetActive(false);
        }

        private void OnInitialized()
        {
            m_StartPage.SetActive(false);
            m_MainPage.SetActive(true);

            m_MCManager.Advertiser.OnDidReceiveInvitationFromPeer += OnDidReceiveInvitationFromPeer;
            m_MCManager.Browser.OnFoundPeer += OnFoundPeer;
            m_MCManager.Browser.OnLostPeer += OnLostPeer;
            m_MCManager.Session.OnPeerDidChangeState += OnPeerDidChangeState;
            m_MCManager.Session.OnDidReceiveData += OnDidReceiveData;

            m_AdvertiserWindow.SetActive(!m_MCManager.AutoConnectMode);
            m_BrowserWindow.SetActive(!m_MCManager.AutoConnectMode);
        }

        private void OnShutdown()
        {
            m_StartPage.SetActive(true);
            m_MainPage.SetActive(false);

            for (int i = 0; i < m_InvitationList.childCount; i++)
            {
                Destroy(m_InvitationList.GetChild(i).gameObject);
            }
            for (int i = 0; i < m_FoundPeerList.childCount; i++)
            {
                Destroy(m_FoundPeerList.GetChild(i).gameObject);
            }
            for (int i = 0; i < m_ConnectedPeerList.childCount; i++)
            {
                Destroy(m_ConnectedPeerList.GetChild(i).gameObject);
            }
            m_StartAdvertisingBtn.SetActive(true);
            m_StopAdvertisingBtn.SetActive(false);
            m_StartBrowsingBtn.SetActive(true);
            m_StopBrowsingBtn.SetActive(false);
        }

        private void OnDidReceiveInvitationFromPeer(MCPeerID peerID, NSData context, InvitationHandler invitationHandler)
        {
            UpdateInvitationList();
        }

        private void OnFoundPeer(MCPeerID peerID, Dictionary<string, string> discoveryInfo)
        {
            UpdateFoundPeerList();
        }

        private void OnLostPeer(MCPeerID peerID)
        {
            UpdateFoundPeerList();
        }

        private void OnPeerDidChangeState(MCPeerID peerID, MCSessionState state)
        {
            UpdateConnectedPeerList();
        }

        private void OnDidReceiveData(NSData data, MCPeerID peerID)
        {
            using (NSString str = new(data))
            {
                m_MessagePrompt.GetComponentInChildren<TMP_Text>().text = $"Did receive message from peer {peerID.DisplayName}";
                m_MessagePrompt.SetActive(true);
                m_MessagePromptLastShowTime = Time.time;
            }
        }

        private void Update()
        {
            if (Time.time - m_MessagePromptLastShowTime > MESSAGE_PROMPT_DURATION)
            {
                m_MessagePrompt.SetActive(false);
            }            
        }

        public void StartAdvertisingPeer()
        {
            m_MCManager.Advertiser.StartAdvertisingPeer();
            m_StartAdvertisingBtn.SetActive(false);
            m_StopAdvertisingBtn.SetActive(true);
        }

        public void StopAdvertisingPeer()
        {
            m_MCManager.Advertiser.StopAdvertisingPeer();
            m_StartAdvertisingBtn.SetActive(true);
            m_StopAdvertisingBtn.SetActive(false);
        }

        public void StartBrowsingForPeers()
        {
            m_MCManager.Browser.StartBrowsingForPeers();
            m_StartBrowsingBtn.SetActive(false);
            m_StopBrowsingBtn.SetActive(true);
        }

        public void StopBrowsingForPeers()
        {
            m_MCManager.Browser.StopBrowsingForPeers();
            m_StartBrowsingBtn.SetActive(true);
            m_StopBrowsingBtn.SetActive(false);
        }

        public void Disconnect()
        {
            m_MCManager.Session.Disconnect();
        }

        private void UpdateInvitationList()
        {
            for (int i = 0; i < m_InvitationList.childCount; i++)
            {
                Destroy(m_InvitationList.GetChild(i).gameObject);
            }

            foreach (var invitation in m_MCManager.Advertiser.PendingInvitations)
            {
                var invitationSlot = Instantiate(m_InvitationSlotPrefab, m_InvitationList);
                invitationSlot.GetComponent<RectTransform>().localScale = Vector3.one;
                invitationSlot.Init(invitation.PeerID.DisplayName,
                    () =>
                    {
                        m_MCManager.Advertiser.HandleInvitation(invitation.InvitationHandler, true, m_MCManager.Session);
                        UpdateInvitationList();
                    },
                    () =>
                    {
                        m_MCManager.Advertiser.HandleInvitation(invitation.InvitationHandler, false, m_MCManager.Session);
                        UpdateInvitationList();
                    });
            }
        }

        private void UpdateFoundPeerList()
        {
            for (int i = 0; i < m_FoundPeerList.childCount; i++)
            {
                Destroy(m_FoundPeerList.GetChild(i).gameObject);
            }

            foreach (var foundPeer in m_MCManager.Browser.FoundPeers)
            {
                var foundPeerSlot = Instantiate(m_FoundPeerSlotPrefab, m_FoundPeerList);
                foundPeerSlot.GetComponent<RectTransform>().localScale = Vector3.one;
                foundPeerSlot.Init(foundPeer.PeerID.DisplayName,
                    () =>
                    {
                        m_MCManager.Browser.InvitePeer(foundPeer.PeerID, m_MCManager.Session);
                        UpdateFoundPeerList();
                    });
            }
        }

        private void UpdateConnectedPeerList()
        {
            for (int i = 0; i < m_ConnectedPeerList.childCount; i++)
            {
                Destroy(m_ConnectedPeerList.GetChild(i).gameObject);
            }

            foreach (var connectedPeer in m_MCManager.Session.ConnectedPeers)
            {
                var connectedPeerSlot = Instantiate(m_ConnectedPeerSlotPrefab, m_ConnectedPeerList);
                connectedPeerSlot.GetComponent<RectTransform>().localScale = Vector3.one;
                connectedPeerSlot.Init(connectedPeer.DisplayName,
                    () =>
                    {
                        using (NSString str = new("Nihao"))
                        using (NSData data = str.Serialize())
                        {
                            m_MCManager.Session.SendData(data, connectedPeer, MCSessionSendDataMode.Reliable);
                        }
                    });
            }
        }
    }
}
