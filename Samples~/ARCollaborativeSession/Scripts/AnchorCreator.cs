// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

namespace HoloInteractive.iOS.MultipeerConnectivity.Samples.ARCollaborativeSession
{
    public class AnchorCreator : MonoBehaviour
    {
        ARRaycastManager m_RaycastManager;

        ARAnchorManager m_AnchorManager;

        float m_LastAnchorTime;

        const float COOLDOWN = 1f;

        private void Start()
        {
            m_RaycastManager = FindObjectOfType<ARRaycastManager>();
            m_AnchorManager = FindObjectOfType<ARAnchorManager>();
        }

        private void Update()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && Time.time - m_LastAnchorTime > COOLDOWN)
            {
                const TrackableType trackableTypes =
                    TrackableType.FeaturePoint |
                    TrackableType.PlaneWithinPolygon;

                List<ARRaycastHit> hits = new();
                if (m_RaycastManager.Raycast(Input.GetTouch(0).position, hits, trackableTypes))
                {
                    var hit = hits[0];
                    _ = m_AnchorManager.AddAnchor(hit.pose);
                    m_LastAnchorTime = Time.time;
                }
            }
        }
    }
}
