// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class AddInitExpressionToProps : CodeDomVisitor
    {
        public AddInitExpressionToProps()
        {
        }
        protected override void Visit(CodeTypeDeclaration type)
        {
            base.Visit(type);

            CollectionHelpers.MapList<CodeMemberProperty>(type.Members, TryAddDefaultValueInit, null);
        }

        private bool TryAddDefaultValueInit(CodeMemberProperty prop)
        {
            foreach (CodeAttributeDeclaration declaration in prop.CustomAttributes)
            {
                if (declaration.Name == "Onvif.Property.InitExpression" && declaration.Arguments.Count > 0)
                {
                    prop.InitExpression = declaration.Arguments[0].Value;
                    return true;
                }
            }

            return true;
        }
    }
}
