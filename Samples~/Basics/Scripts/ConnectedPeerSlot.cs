// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace HoloInteractive.iOS.MultipeerConnectivity.Samples.Basics
{
    public class ConnectedPeerSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_DisplayName;

        [SerializeField] private Button m_SendMessageBtn;

        public void Init(string displayName, Action onSend)
        {
            m_DisplayName.text = displayName;
            m_SendMessageBtn.onClick.AddListener(() =>
            {
                onSend();
            });
        }
    }
}
