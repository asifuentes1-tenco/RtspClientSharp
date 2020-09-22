using System;

namespace RtspClientSharp.RawFrames.Metadata
{
    /// <summary>
    /// Represents a frame contaning an xml ONVIF METADATA response
    /// </summary>
    class MetadataFrame : RawFrame
    {
        /// <summary>
        /// Value saving the type of this metadata frame
        /// </summary>
        public override FrameType Type => FrameType.Metadata;
        /// <summary>
        /// Represents a frame contaning an xml ONVIF METADATA response
        /// </summary>
        /// <param name="timestamp">Datetime when this frame was created</param>
        /// <param name="frameSegment">Frame segment contaning the byte array</param>
        public MetadataFrame(DateTime timestamp, ArraySegment<byte> frameSegment)
            : base(timestamp, frameSegment)
        {
        }
    }
}
