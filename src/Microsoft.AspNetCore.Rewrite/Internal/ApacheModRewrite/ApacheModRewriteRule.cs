﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Rewrite.Logging;

namespace Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite
{
    public class ApacheModRewriteRule : IRule
    {
        public UrlMatch InitialMatch { get; }
        public IList<Condition> Conditions { get; }
        public IList<UrlAction> Actions { get; }

        public ApacheModRewriteRule(UrlMatch initialMatch, IList<Condition> conditions, IList<UrlAction> urlActions)
        {
            Conditions = conditions;
            InitialMatch = initialMatch;
            Actions = urlActions;
        }

        public virtual async Task ApplyRuleAsync(RewriteContext context)
        {
            // 1. Figure out which section of the string to match for the initial rule.
            var initMatchRes = InitialMatch.Evaluate(context.HttpContext.Request.Path, context);

            if (!initMatchRes.Success)
            {
                context.Logger?.ModRewriteDidNotMatchRule();
                return;
            }

            BackReferenceCollection condBackReferences = null;
            if (Conditions != null)
            {
                var condResult = ConditionHelper.Evaluate(Conditions, context, initMatchRes.BackReferences);
                if (!condResult.Success)
                {
                    context.Logger?.ModRewriteDidNotMatchRule();
                    return;
                }
            }

            // At this point, we know our rule passed, first apply pre conditions,
            // which can modify things like the cookie or env, and then apply the action
            context.Logger?.ModRewriteMatchedRule();

            foreach (var action in Actions)
            {
                await action.ApplyActionAsync(context, initMatchRes?.BackReferences, condBackReferences);
            }
        }
    }
}
