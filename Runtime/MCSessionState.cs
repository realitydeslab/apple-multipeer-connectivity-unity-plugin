namespace HoloInteractive.iOS.MultipeerConnectivity
{
    /// <summary>
    /// Indicates the current state of a given peer within a session.
    /// https://developer.apple.com/documentation/multipeerconnectivity/mcsessionstate?language=objc
    /// </summary>
    public enum MCSessionState
    {
        /// <summary>
        /// The peer is not (or is no longer) in this session.
        /// </summary>
        NotConnected = 0,

        /// <summary>
        /// A connection to the peer is currently being established.
        /// </summary>
        Connecting = 1,

        /// <summary>
        /// The peer is connected to this session.
        /// </summary>
        Connected = 2
    }
}
