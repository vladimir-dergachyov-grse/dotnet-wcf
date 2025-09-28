// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class ReplaceFieldsByPropsInMessageContracts : MessageContractVisitor
    {
        public ReplaceFieldsByPropsInMessageContracts()
        {
        }
        protected override void VisitAttributedType(CodeTypeDeclaration type)
        {
            base.VisitAttributedType(type);

            var fields = new List<CodeMemberField>();

            CollectionHelpers.MapList<CodeMemberField>(type.Members, delegate (CodeMemberField field) { fields.Add(field); return false; }, null);

            foreach (var field in fields)
            {
                var prop = new CodeMemberProperty()
                {
                    Type = field.Type,
                    Name = field.Name,
                    CustomAttributes = field.CustomAttributes,
                    LinePragma = field.LinePragma,
                    HasGet = true,
                    HasSet = true,
                    InitExpression = field.InitExpression,
                };

                prop.Attributes = (prop.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Public;

                prop.Comments.AddRange(field.Comments);
                prop.StartDirectives.AddRange(field.StartDirectives);
                prop.EndDirectives.AddRange(field.EndDirectives);

                type.Members.Add(prop);
            }
        }
    }
}
