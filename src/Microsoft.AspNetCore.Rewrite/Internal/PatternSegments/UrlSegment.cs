﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http.Extensions;

namespace Microsoft.AspNetCore.Rewrite.Internal.PatternSegments
{
    public class UrlSegment : PatternSegment
    {
        public override string Evaluate(RewriteContext context, BackReferenceCollection ruleBackReferences, BackReferenceCollection conditionBackReferences)
        {
            return context.GlobalRule ? context.HttpContext.Request.GetEncodedUrl() : (string)context.HttpContext.Request.Path;
        }
    }
}
