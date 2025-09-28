// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom
{
    using System.Diagnostics;
    using System;
    using Microsoft.Win32;
    using System.Collections;
    using System.Runtime.InteropServices;

    /// <devdoc>
    ///    <para>
    ///       Represents a class property.
    ///    </para>
    /// </devdoc>
    [
        //  ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
    // Serializable,
    ]
    public class CodeMemberProperty : CodeTypeMember
    {
        private CodeTypeReference _type;
        private CodeParameterDeclarationExpressionCollection _parameters = new CodeParameterDeclarationExpressionCollection();
        private bool _hasGet;
        private bool _hasSet;
        private CodeStatementCollection _getStatements = new CodeStatementCollection();
        private CodeStatementCollection _setStatements = new CodeStatementCollection();
        private CodeTypeReference _privateImplements = null;
        private CodeTypeReferenceCollection _implementationTypes = null;
        private CodeExpression _initExpression = null;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeTypeReference PrivateImplementationType
        {
            get
            {
                return _privateImplements;
            }
            set
            {
                _privateImplements = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeTypeReferenceCollection ImplementationTypes
        {
            get
            {
                if (_implementationTypes == null)
                {
                    _implementationTypes = new CodeTypeReferenceCollection();
                }
                return _implementationTypes;
            }
        }

        /// <devdoc>
        ///    <para>Gets or sets the data type of the property.</para>
        /// </devdoc>
        public CodeTypeReference Type
        {
            get
            {
                if (_type == null)
                {
                    _type = new CodeTypeReference("");
                }
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets a value
        ///       indicating whether the property has a get method accessor.
        ///    </para>
        /// </devdoc>
        public bool HasGet
        {
            get
            {
                return _hasGet || _getStatements.Count > 0;
            }
            set
            {
                _hasGet = value;
                if (!value)
                {
                    _getStatements.Clear();
                }
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets a value
        ///       indicating whether the property has a set method accessor.
        ///    </para>
        /// </devdoc>
        public bool HasSet
        {
            get
            {
                return _hasSet || _setStatements.Count > 0;
            }
            set
            {
                _hasSet = value;
                if (!value)
                {
                    _setStatements.Clear();
                }
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the collection of get statements for the
        ///       property.
        ///    </para>
        /// </devdoc>
        public CodeStatementCollection GetStatements
        {
            get
            {
                return _getStatements;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the collection of get statements for the property.
        ///    </para>
        /// </devdoc>
        public CodeStatementCollection SetStatements
        {
            get
            {
                return _setStatements;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the collection of declaration expressions
        ///       for
        ///       the property.
        ///    </para>
        /// </devdoc>
        public CodeParameterDeclarationExpressionCollection Parameters
        {
            get
            {
                return _parameters;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the initialization expression for the member field.
        ///    </para>
        /// </devdoc>
        public CodeExpression InitExpression
        {
            get
            {
                return _initExpression;
            }
            set
            {
                _initExpression = value;
            }
        }

        public string GetString { get; set; }
        public string SetString { get; set; }
        public bool Nullable { get; set; }
        public bool Required { get; set; }
    }
}
