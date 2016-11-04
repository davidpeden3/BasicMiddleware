// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.AspNetCore.Rewrite.Internal
{
    public class BackReferenceCollection
    {
        public List<GroupCollection> backReferenceCollection;

        public BackReferenceCollection()
        {
            backReferenceCollection = new List<GroupCollection>();
        }



        public void AddBackReferences(GroupCollection backReferences)
        {
            backReferenceCollection.Add(backReferences);
        }

        public string getBackReferenceAtIndex(int index)
        {
            var currentNumBackRefs = 0;
            for(int i = 0; i < backReferenceCollection.Count -1; i++ )
            {
                var currentGroupCollection = backReferenceCollection[i];
                currentNumBackRefs += currentGroupCollection.Count;
                if (currentNumBackRefs >= index)
                {
                    return currentGroupCollection[currentNumBackRefs - index].Value;
                }
            }
            throw new IndexOutOfRangeException($"There index {index} was outside the range of the captured back references");
        }
    }
}
