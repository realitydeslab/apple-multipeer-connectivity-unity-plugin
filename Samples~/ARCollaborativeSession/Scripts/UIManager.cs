// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;

namespace HoloInteractive.iOS.MultipeerConnectivity.Samples.ARCollaborativeSession
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject m_MessagePrompt;

        CollaborativeSession m_Session;

        private float m_MessagePromptLastShowTime;

        private const float MESSAGE_PROMPT_DURATION = 3;

        private void Awake()
        {
            m_Session = FindObjectOfType<CollaborativeSession>();
            m_Session.OnInitialized += OnInitialized;

            m_MessagePrompt.SetActive(false);
        }

        private void OnInitialized()
        {
            m_Session.MCSession.OnPeerDidChangeState += OnPeerDidChangeState;

            var participantManager = FindObjectOfType<ARParticipantManager>();
            participantManager.participantsChanged += OnParticipantsChanged;
        }

        private void OnPeerDidChangeState(MCPeerID peerID, MCSessionState state)
        {
            m_MessagePrompt.GetComponentInChildren<TMP_Text>().text = $"Peer {peerID.DisplayName} did change state to {state}";
            m_MessagePromptLastShowTime = Time.time;
            m_MessagePrompt.SetActive(true);
        }

        private void Update()
        {
            if (m_MessagePrompt.activeSelf && Time.time - m_MessagePromptLastShowTime > MESSAGE_PROMPT_DURATION)
            {
                m_MessagePrompt.SetActive(false);
            }
        }

        private void OnParticipantsChanged(ARParticipantsChangedEventArgs args)
        {
            if (args.added.Count > 0)
            {
                m_MessagePrompt.GetComponentInChildren<TMP_Text>().text = $"Added ARParticipantAnchor";
                m_MessagePromptLastShowTime = Time.time;
                m_MessagePrompt.SetActive(true);
            }
        }
    }
}
