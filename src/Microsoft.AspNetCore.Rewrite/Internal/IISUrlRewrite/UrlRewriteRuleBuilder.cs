﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Rewrite.Internal.UrlMatches;

namespace Microsoft.AspNetCore.Rewrite.Internal.IISUrlRewrite
{
    public class UrlRewriteRuleBuilder
    {
        private readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(1);

        public string Name { get; set; }
        public bool Enabled { get; set; }

        private UrlMatch _initialMatch;
        private IList<Condition> _conditions;
        private readonly IDictionary<string, ServerVariable> _serverVariables = new Dictionary<string, ServerVariable>();
        private UrlAction _action;
        private bool _matchAny;
        private bool _trackAllCaptures;

        public IISUrlRewriteRule Build(bool global)
        {
            if (_initialMatch == null || _action == null)
            {
                throw new InvalidOperationException("Cannot create UrlRewriteRule without action and match");
            }

            return new IISUrlRewriteRule(Name, _initialMatch, _conditions, _serverVariables.Values, _action, _trackAllCaptures, global);
        }

        public void AddUrlAction(UrlAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action), "Rules must contain an action");
            }
            _action = action;
        }

        public void AddUrlMatch(string input, bool ignoreCase = true, bool negate = false, PatternSyntax patternSyntax = PatternSyntax.ECMAScript)
        {
            switch (patternSyntax)
            {
                case PatternSyntax.ECMAScript:
                    {
                        if (ignoreCase)
                        {
                            var regex = new Regex(input, RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase, RegexTimeout);
                            _initialMatch = new RegexMatch(regex, negate);
                        }
                        else
                        {
                            var regex = new Regex(input, RegexOptions.CultureInvariant | RegexOptions.Compiled, RegexTimeout);
                            _initialMatch = new RegexMatch(regex, negate);
                        }
                        break;
                    }
                case PatternSyntax.Wildcard:
                    throw new NotSupportedException("Wildcard syntax is not supported");
                case PatternSyntax.ExactMatch:
                    _initialMatch = new ExactMatch(ignoreCase, input, negate);
                    break;
            }
        }

        public void AddUrlCondition(Pattern input, string pattern, PatternSyntax patternSyntax, MatchType matchType, bool ignoreCase, bool negate, bool trackAllCaptures)
        {
            // If there are no conditions specified
            if (_conditions == null)
            {
                AddUrlConditions(LogicalGrouping.MatchAll, trackAllCaptures);
            }

            switch (patternSyntax)
            {
                case PatternSyntax.ECMAScript:
                    {
                        switch (matchType)
                        {
                            case MatchType.Pattern:
                                {
                                    if (string.IsNullOrEmpty(pattern))
                                    {
                                        throw new FormatException("Match does not have an associated pattern attribute in condition");
                                    }

                                    var regex = new Regex(
                                        pattern,
                                        ignoreCase ? RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase :
                                            RegexOptions.CultureInvariant | RegexOptions.Compiled,
                                        RegexTimeout);

                                    _conditions.Add(new Condition { Input = input, Match = new RegexMatch(regex, negate), OrNext = _matchAny });
                                    break;
                                }
                            case MatchType.IsDirectory:
                                {
                                    _conditions.Add(new Condition { Input = input, Match = new IsDirectoryMatch(negate), OrNext = _matchAny });
                                    break;
                                }
                            case MatchType.IsFile:
                                {
                                    _conditions.Add(new Condition { Input = input, Match = new IsFileMatch(negate), OrNext = _matchAny });
                                    break;
                                }
                            default:
                                throw new FormatException("Unrecognized matchType");
                        }
                        break;
                    }
                case PatternSyntax.Wildcard:
                    throw new NotSupportedException("Wildcard syntax is not supported");
                case PatternSyntax.ExactMatch:
                    if (pattern == null)
                    {
                        throw new FormatException("Match does not have an associated pattern attribute in condition");
                    }
                    _conditions.Add(new Condition { Input = input, Match = new ExactMatch(ignoreCase, pattern, negate), OrNext = _matchAny });
                    break;
                default:
                    throw new FormatException("Unrecognized pattern syntax");
            }
        }

        public void AddUrlConditions(LogicalGrouping logicalGrouping, bool trackAllCaptures)
        {
            _conditions = new List<Condition>();
            _matchAny = logicalGrouping == LogicalGrouping.MatchAny;
            _trackAllCaptures = trackAllCaptures;
        }

        public void SetServerVariable(ServerVariable serverVariable)
        {
            _serverVariables[serverVariable.Name] = serverVariable;
        }
    }
}
