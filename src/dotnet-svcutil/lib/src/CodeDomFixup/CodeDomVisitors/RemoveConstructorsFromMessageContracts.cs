// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class RemoveConstructorsFromMessageContracts : MessageContractVisitor
    {
        public RemoveConstructorsFromMessageContracts()
        {
        }
        protected override void VisitAttributedType(CodeTypeDeclaration type)
        {
            base.VisitAttributedType(type);

            CollectionHelpers.MapList<CodeConstructor>(type.Members, delegate (CodeConstructor ctor) { return false; }, null);
        }
    }
}
