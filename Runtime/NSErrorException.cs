// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System;

namespace HoloInteractive.iOS.MultipeerConnectivity
{
    public class NSErrorException : Exception
    {
        public NSErrorException(long code, string description)
        : base($"NSError {code}: {description}")
        {
            Code = code;
            Description = description;
        }

        public long Code { get; private set; }

        public string Description { get; private set; }
    }
}