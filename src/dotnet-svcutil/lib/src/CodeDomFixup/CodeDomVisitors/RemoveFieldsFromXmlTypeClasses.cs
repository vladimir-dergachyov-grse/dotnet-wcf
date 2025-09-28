// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class RemoveFieldsFromXmlTypeClasses : XmlTypeVisitor
    {
        public RemoveFieldsFromXmlTypeClasses()
        {
        }

        protected override void VisitAttributedType(CodeTypeDeclaration type)
        {
            base.VisitAttributedType(type);

            if (!type.IsClass)
            {
                return;
            }

            CollectionHelpers.MapList<CodeMemberField>(type.Members, delegate (CodeMemberField field) { return false; }, null);

            foreach (var member in type.Members)
            {
                var prop = member as CodeMemberProperty;
                if (prop != null)
                {
                    prop.GetStatements.Clear();
                    prop.SetStatements.Clear();
                    prop.HasGet = true;
                    prop.HasSet = true;
                }
            }
        }
    }
}
