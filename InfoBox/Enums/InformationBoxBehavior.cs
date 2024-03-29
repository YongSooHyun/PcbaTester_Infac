// <copyright file="InformationBoxBehavior.cs" company="Johann Blais">
// Copyright (c) 2008 All Right Reserved
// </copyright>
// <author>Johann Blais</author>
// <summary>Specifies constants defining how is displayed the InformationBox</summary>

namespace InfoBox
{
    /// <summary>
    /// Specifies constants defining how is displayed the <see cref="InformationBox"/>.
    /// </summary>
    public enum InformationBoxBehavior
    {
        /// <summary>
        /// The InformationBox is displayed as a modal (blocking) window (default).
        /// </summary>
        Modal,

        /// <summary>
        /// The InformationBox is displayed as a modeless (non-blocking) window.
        /// </summary>
        Modeless,
    }
}
