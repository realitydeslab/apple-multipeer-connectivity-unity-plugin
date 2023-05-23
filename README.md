# Apple Multipeer Connectivity Unity Plugin

## Overview

This plugin is a C# wrapper of the [Apple's Multipeer Connectivity](https://developer.apple.com/documentation/multipeerconnectivity), which allows you to use the iOS native framework in Unity. Apple's Multipeer Connectivity framework supports peer-to-peer connectivity and the discovery of nearby devices, which allows nearby iOS devices to connect directly to each other and send messages. It is the technology behind AirDrop, which is useful in some scenarios such as multiuser Augmented Reality.

The main goal of this package is to mirror the native Objective-C API to C# in an authentic way. Therefore, in order to use this package, you need to deal with some unmanaged objects which need to be released manually using the `Dispose()` method in C#.

## How to Install

You can install this package from the following git URL in Package Manager.
```
https://github.com/holoi/apple-multipeer-connectivity-unity-plugin.git
```
<img width="286" alt="image" src="https://github.com/holoi/apple-multipeer-connectivity-unity-plugin/assets/44870300/212e2158-d97f-42b7-9c5c-0f09ed040151">
<img width="333" alt="image" src="https://github.com/holoi/apple-multipeer-connectivity-unity-plugin/assets/44870300/ecac6610-ce06-4b7c-b3cb-036e51fc54e5">

or by directly adding the following line to the `Packages/manifest.json` file.
```
"com.holoi.ios.multipeer-connectivity": "https://github.com/holoi/apple-multipeer-connectivity-unity-plugin.git"
```

## How to Use This Package

There are some sample projects in this package which you can import. We recommend you to start with the Basics sample, which showcases how to discover nearby devices and establish connection. Knowledge of the [Multipeer Connectivity framework](https://developer.apple.com/documentation/multipeerconnectivity) in Swift or Objective-C is definitely helpful, since this pakcage simply wraps the framework API from native iOS programming language to C#. However, if you are not familiar with the native framework, the following section will try to explain the basic mechanism of Multipeer Connectivity in a simplified way.

## Tutorial

In a Multipeer Connectivity session, there are discovery phase and connection phase. In discovery phase, a device discovers services provided by nearby devices and tries the establish the connection. In connection phase, nearby devices are connected in a peer-to-peer manner and able to send message to each other.

To establish a Multipeer Connectivity session, we first create a `MCPeerID` object and a `MCSession` object. 
```
MCPeerID m_PeerID;
MCSession m_MCSession;

public void InitMCSession() 
{
    m_PeerID = new MCPeerID("your-device-name");
    m_MCSession = new MCSession(m_PeerID);
}
```
A `MCPeerID` object represents your iOS device and its name is displayed in the network service. In order to provide and discover services, we need `MCNearbyServiceAdvertiser` object and `MCNearbyServiceBrowser` object. A `MCNearbyServiceAdvertiser` publishes an advertisement for a specific service that your app provides and a `MCNearbyServiceBrowser` searches for services offered by nearby devices. `SERVICE_TYPE` is a unique string that distinguish your service from others.
```
MCNearbyServiceAdvertiser m_Advertiser;
MCNearbyServiceBrowser m_Browser;

public void InitDiscoveryPhase()
{
    // Set the optional discoveryInfo to null
    m_Advertiser = new MCNearbyServiceAdvertiser(m_PeerID, null, SERVICE_TYPE);
    m_Browser = new(m_PeerID, SERVICE_TYPE);
}
```
When creating a new `MCNearbyServiceAdvertiser`, you can optionally offer a `discoveryInfo`, which is of type `Dictionary<string, string>`. When a `MCNearbyServiceBrowser` finds the service, it will also receive the `discoveryInfo` which provides further information of the service. We should register callback functions before starting the discovery phase.
```
public void StartDiscoveryPhase()
{
    // This callback function is invoked when the advertiser receives an invitation from a peer
    m_Advertiser.OnDidReceiveInvitationFromPeer += OnDidReceiveInvitationFromPeer;
    // This callback function is invoked when the browser finds a nearby service
    m_Browser.OnFoundPeer += OnFoundPeer;
    
    m_Advertiser.StartAdvertisingPeer();
    m_Browser.StartBrowsingForPeers();
}

private void OnDidReceiveInvitationFromPeer(MCPeerID peerID, NSData contect, InvitationHandler invitationHandler)
{
    // Accept the invitation to join the session
    m_Advertiser.HandleInvitation(invitationHandler, true, m_MCSession);
}

private void OnFoundPeer(MCPeerID peerID, Dictionary<string, string> discoveryInfo)
{
    // Invite the peer to join the session
    m_Browser.InvitePeer(peerID, m_MCSession);
}
```
As shown in the above code snippet, the callback function `MCNearbyServiceBrowser.OnFoundPeer` is invoked when the browser finds a nearby service. And the callback function `MCNearbyServiceAdvertiser.OnDidReceiveInvitationFromPeer` is invoked when the advertiser receives an invitation from a peer. When the advertiser accepts an invitation, the network connection between the devices should be established. The callback function `MCSession.OnPeerDidChangeState` is invoked when the state of a peer changes. Finally, when finishing using Multipeer Connectivity session, we need to manually release those unmanaged objects.
```
public void Shutdown()
{
    m_PeerID.Dispose();
    m_MCSession.Dispose();
    m_Advertiser.Dispose();
    m_Browser.Dispose();
}
```
The following is the minimal implementation of the Multipeer Connectivity framework.
```
using System.Collections.Generic;
using UnityEngine;
using System;

public class MCManager : MonoBehaviour
{
    MCPeerID m_PeerID;

    MCSession m_MCSession;

    MCNearbyServiceAdvertiser m_Advertiser;

    MCNearbyServiceBrowser m_Browser;

    const string SERVICE_TYPE = "unity-mc";

    public void Init()
    {
        // Initialize unmanaged objects
        m_PeerID = new("yuchen");
        m_MCSession = new(m_PeerID);
        m_Advertiser = new(m_PeerID, null, SERVICE_TYPE);
        m_Browser = new(m_PeerID, SERVICE_TYPE);

        // Register callback functions
        m_Advertiser.OnDidReceiveInvitationFromPeer += OnDidReceiveInvitationFromPeer;
        m_Browser.OnFoundPeer += OnFoundPeer;
        // This callback function is invoked when the state of a nearby peer changes
        m_MCSession.OnPeerDidChangeState += OnPeerDidChangeState;

        // Start discovery phase
        m_Advertiser.StartAdvertisingPeer();
        m_Browser.StartBrowsingForPeers();
    }

    public void Shutdown()
    {
        m_PeerID.Dispose();
        m_Session.Dispose();
        m_Advertiser.Dispose();
        m_Browser.Dispose();
    }

    private void OnDidReceiveInvitationFromPeer(MCPeerID peerID, NSData contect, InvitationHandler invitationHandler)
    {
        m_Advertiser.HandleInvitation(invitationHandler, true, m_MCSession);
    }

    private void OnFoundPeer(MCPeerID peerID, Dictionary<string, string> discoveryInfo)
    {
        m_Browser.InvitePeer(peerID, m_MCSession);
    }

    private void OnPeerDidChangeState(MCPeerID peerID, MCSessionState state)
    {
        Debug.Log($"OnPeerDidChangeState {peerID.DisplayName} {state}");
    }
}
```

### Want to change `SERVICE_TYPE`?

If you want to change `SERVICE_TYPE`, you also need to change the `NSBonjourServices` entry in Xcode's info.plist.
<img width="1053" alt="image" src="https://github.com/holoi/AppleMultipeerConnectivityUnityPlugin/assets/44870300/327f8354-d90d-4810-8735-8ae3294accbb">
For example, if your `SERVICE_TYPE` value is `my-service`, then the values in `NSBonjourServices` should be `_my-service._tcp` and `_my-service._tcp`. Please refer to [NSBonjourServices](https://developer.apple.com/documentation/bundleresources/information_property_list/nsbonjourservices?language=objc) for details.

## Need Help?

This is a new repo and we will keep refining it. If you have any question or problem, please post an issue. Thank you.
