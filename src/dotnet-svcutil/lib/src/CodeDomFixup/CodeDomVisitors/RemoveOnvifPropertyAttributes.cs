// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class RemoveOnvifPropertyAttributes : CodeDomVisitor
    {
        public RemoveOnvifPropertyAttributes()
        {
        }
        protected override void Visit(CodeTypeMember member)
        {
            base.Visit(member);

            CollectionHelpers.MapList<CodeAttributeDeclaration>(
                member.CustomAttributes, delegate(CodeAttributeDeclaration attr) { return !attr.Name.StartsWith("Onvif."); }, null);
            CollectionHelpers.MapList<CodeCommentStatement>(
                member.Comments, delegate (CodeCommentStatement comment) { return !comment.Comment.Text.Contains("Onvif."); }, null);
        }
    }
}
