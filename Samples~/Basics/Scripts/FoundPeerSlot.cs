// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

namespace HoloInteractive.iOS.MultipeerConnectivity.Samples.Basics
{
    public class FoundPeerSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_DisplayName;

        [SerializeField] private Button m_InviteBtn;

        public void Init(string displayName, Action onInvite)
        {
            m_DisplayName.text = displayName;
            m_InviteBtn.onClick.AddListener(() =>
            {
                onInvite();
            });
        }
    }
}
