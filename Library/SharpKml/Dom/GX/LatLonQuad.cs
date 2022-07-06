﻿// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.GX
{
    using System.Diagnostics.CodeAnalysis;
    using SharpKml.Base;

    /// <summary>
    /// Allows non-rectangular quadrilateral ground overlays.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("LatLonQuad", KmlNamespaces.GX22Namespace)]
    public class LatLonQuad : KmlObject
    {
        private CoordinateCollection coords;

        /// <summary>
        /// Gets or sets the four corner points of a quadrilateral defining the
        /// overlay area.
        /// </summary>
        /// <remarks>
        /// <para>Exactly four coordinate tuples have to be provided, specified
        /// in counter-clockwise order with the first coordinate corresponding
        /// to the lower-left corner of the overlayed image. The shape described
        /// by these corners must be convex.</para>
        /// <para>All altitude values are ignored.</para>
        /// </remarks>
        [KmlElement(null)]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "This object is a DTO")]
        public CoordinateCollection Coordinates
        {
            get => this.coords;
            set => this.UpdatePropertyChild(value, ref this.coords);
        }
    }
}
