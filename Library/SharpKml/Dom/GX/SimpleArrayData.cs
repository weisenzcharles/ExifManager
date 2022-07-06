﻿// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.GX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using SharpKml.Base;

    /// <summary>
    /// Represents an array of values.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("SimpleArrayData", KmlNamespaces.GX22Namespace)]
    public class SimpleArrayData : Element
    {
        private static readonly XmlComponent ValueComponent = new XmlComponent(null, "value", KmlNamespaces.GX22Namespace);

        /// <summary>
        /// Gets or sets the name of the array.
        /// </summary>
        [KmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the collection of values stored by this instance.
        /// </summary>
        public IEnumerable<string> Values => this.ValueElements.Select(v => v.Value);

        [KmlElement(null, 1)]
        private List<ValueElement> ValueElements { get; } = new List<ValueElement>();

        /// <summary>
        /// Adds the specified value to <see cref="Values"/>.</summary>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        public void AddValue(string value)
        {
            Check.IsNotNull(value, nameof(value));

            this.AddAsChild(this.ValueElements, new ValueElement(value));
        }

        /// <summary>
        /// Adds the gx:value to <see cref="Values"/>.
        /// </summary>
        /// <param name="orphan">The <see cref="Element"/> to add.</param>
        protected internal override void AddOrphan(Element orphan)
        {
            if (orphan is UnknownElement unknown)
            {
                if (ValueComponent.Equals(unknown.UnknownData))
                {
                    this.AddValue(unknown.InnerText);
                    return;
                }
            }

            base.AddOrphan(orphan);
        }

        // Required for the parsing to work
#pragma warning disable IDE0051 // Remove unused private members
        private void AddValueElement(ValueElement value)
        {
            this.ValueElements.Add(value);
        }
#pragma warning restore IDE0051 // Remove unused private members

        /// <summary>
        /// Used to correctly serialize the strings in Values.
        /// </summary>
        internal class ValueElement : Element, ICustomElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValueElement"/> class.
            /// </summary>
            /// <param name="value">The value of the element.</param>
            public ValueElement(string value)
            {
                this.Value = value;
            }

            /// <summary>
            /// Gets a value indicating whether to process the children of the Element.
            /// </summary>
            public bool ProcessChildren => false;

            /// <summary>
            /// Gets the value of the node.
            /// </summary>
            public string Value { get; }

            /// <summary>
            /// Writes the start of an XML element.
            /// </summary>
            /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
            public void CreateStartElement(XmlWriter writer)
            {
                writer.WriteElementString(
                    KmlNamespaces.GX22Prefix,
                    "value",
                    KmlNamespaces.GX22Namespace,
                    this.Value);
            }
        }
    }
}
