// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
//
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
//
// SPDX-License-Identifier: MIT

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace HoloInteractive.iOS.MultipeerConnectivity.Editor
{
    public static class MultipeerConnectivityBuildProcessor
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                // For info.plist
                string plistPath = buildPath + "/Info.plist";
                PlistDocument plist = new();
                plist.ReadFromFile(plistPath);
                PlistElementDict rootDict = plist.root;

                rootDict.SetString("NSLocalNetworkUsageDescription", "For connecting to nearby iOS devices.");
                PlistElementArray array = rootDict.CreateArray("NSBonjourServices");
                array.AddString("_unity-mc._tcp");
                array.AddString("_unity-mc._udp");

                File.WriteAllText(plistPath, plist.WriteToString());

                // For build settings
                string projectPath = PBXProject.GetPBXProjectPath(buildPath);
                PBXProject project = new();
                project.ReadFromString(File.ReadAllText(projectPath));

                string mainTargetGuid = project.GetUnityMainTargetGuid();
                string unityFrameworkTargetGuid = project.GetUnityFrameworkTargetGuid();

                project.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");
                project.SetBuildProperty(unityFrameworkTargetGuid, "ENABLE_BITCODE", "NO");

                project.WriteToFile(projectPath);
            }
        }
    }
}