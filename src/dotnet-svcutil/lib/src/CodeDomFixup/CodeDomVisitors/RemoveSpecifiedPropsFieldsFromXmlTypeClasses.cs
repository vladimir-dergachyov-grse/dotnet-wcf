// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class RemoveSpecifiedPropsFieldsFromXmlTypeClasses : XmlTypeVisitor
    {
        public RemoveSpecifiedPropsFieldsFromXmlTypeClasses()
        {
        }

        protected override void VisitAttributedType(CodeTypeDeclaration type)
        {
            base.VisitAttributedType(type);

            if (!type.IsClass)
            {
                return;
            }

            CollectionHelpers.MapList<CodeMemberProperty>(type.Members, IsNotSpecified, null);
            CollectionHelpers.MapList<CodeMemberField>(type.Members, IsNotSpecified, null);
        }

        private static bool IsNotSpecified(CodeTypeMember member)
        {
            return !member.Name.EndsWith("Specified");
        }
    }
}
