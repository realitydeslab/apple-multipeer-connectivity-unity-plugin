// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace HoloInteractive.iOS.MultipeerConnectivity
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NSDictionary_NSString : IDisposable
    {
        IntPtr m_Ptr;

        internal NSDictionary_NSString(IntPtr existing) => m_Ptr = existing;

        public NSDictionary_NSString(Dictionary<string, string> dict)
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));

            List<NSString> nsStringList = new();
            foreach (string key in dict.Keys)
            {
                nsStringList.Add(new NSString(key));
                nsStringList.Add(new NSString(dict[key]));
            }
            var nsStringArr = nsStringList.ToArray();
            GCHandle arrayHandle = GCHandle.Alloc(nsStringArr, GCHandleType.Pinned);
            IntPtr arrPtr = arrayHandle.AddrOfPinnedObject();
            m_Ptr = CreateWithArray(arrPtr, dict.Count);

            foreach (var nsString in nsStringArr)
            {
                nsString.Dispose();
            }
        }

        public IntPtr NativePtr => m_Ptr;

        public Dictionary<string, string> ToDictionary()
        {
            int count = GetDictCount(this);
            NSString[] keys = new NSString[count];
            NSString[] values = new NSString[count];

            GetDictKeys(this, keys);
            GetDictValues(this, values);

            Dictionary<string, string> resDict = new();
            for(int i = 0; i < count; i++)
            {
                string key = keys[i].ToString();
                string value = values[i].ToString();
                resDict[key] = value;

                keys[i].Dispose();
                values[i].Dispose();
            }

            return resDict;
        }

        public void Dispose() => NativeApi.CFRelease(ref m_Ptr);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_NSDictionaryNSString_createWithArray")]
        static extern IntPtr CreateWithArray(IntPtr arrPtr, int length);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_NSDictionaryNSString_getDictCount")]
        static extern int GetDictCount(NSDictionary_NSString self);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_NSDictionaryNSString_getDictKeys")]
        static extern void GetDictKeys(NSDictionary_NSString self, NSString[] keys);

        [DllImport("__Internal", EntryPoint = "HoloInteractiveMC_NSDictionaryNSString_getDictValues")]
        static extern void GetDictValues(NSDictionary_NSString self, NSString[] values);
    }
}
