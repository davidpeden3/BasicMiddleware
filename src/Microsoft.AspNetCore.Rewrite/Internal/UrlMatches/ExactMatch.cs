// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Microsoft.AspNetCore.Rewrite.Internal.UrlMatches
{
    public class ExactMatch : UrlMatch
    {
        private readonly bool _ignoreCase;
        private readonly string _stringMatch;

        public ExactMatch(bool ignoreCase, string input, bool negate)
        {
            _ignoreCase = ignoreCase;
            _stringMatch = input;
            Negate = negate;
        }

        public override MatchResults Evaluate(string pattern, RewriteContext context)
        {
            var pathMatch = string.Compare(pattern, _stringMatch, _ignoreCase);
            Regex match;
            if(_ignoreCase)
            {
                match = new Regex(_stringMatch, RegexOptions.Compiled | RegexOptions.CultureInvariant);
            }
            else
            {
                match = new Regex(_stringMatch, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            }

            var groupCollection = match.Match(pattern).Groups;


            return new MatchResults { Success = ((pathMatch == 0) != Negate) };
        }
    }
}
