// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

namespace HoloInteractive.iOS.MultipeerConnectivity
{
    /// <summary>
    /// Indicates whether delivery of data should be guaranteed.
    /// https://developer.apple.com/documentation/multipeerconnectivity/mcsessionsenddatamode?language=objc
    /// </summary>
    public enum MCSessionSendDataMode
    {
        /// <summary>
        /// The framework should guarantee delivery of each message, enqueueing and retransmitting data as needed,
        /// and ensuring in-order delivery.
        /// </summary>
        Reliable = 0,

        /// <summary>
        /// Messages to peers should be sent immediately without socket-level queueing.
        /// If a message cannot be sent immediately, it should be dropped. The order of messages is not guaranteed.
        /// </summary>
        Unreliable = 1
    }
}
