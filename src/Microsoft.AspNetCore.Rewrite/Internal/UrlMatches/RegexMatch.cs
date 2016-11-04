// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Microsoft.AspNetCore.Rewrite.Internal.UrlMatches
{
    public class RegexMatch : UrlMatch
    {
        private readonly Regex _match;

        public RegexMatch(Regex match, bool negate)
        {
            _match = match;
            Negate = negate;
        }

        public override MatchResults Evaluate(string pattern, RewriteContext context)
        {
            var res = _match.Match(pattern);
            context.BackReferences.AddBackReferences(res.Groups);
            return new MatchResults { Success = (res.Success != Negate) };
        }
    }
}
