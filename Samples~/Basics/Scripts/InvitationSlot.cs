// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

namespace HoloInteractive.iOS.MultipeerConnectivity.Samples.Basics
{
    public class InvitationSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_DisplayName;

        [SerializeField] private Button m_AcceptBtn;

        [SerializeField] private Button m_RejectBtn;

        public void Init(string displayName, Action onAccept, Action onReject)
        {
            m_DisplayName.text = displayName;
            m_AcceptBtn.onClick.AddListener(() => { onAccept(); });
            m_RejectBtn.onClick.AddListener(() => { onReject(); });
        }
    }
}
