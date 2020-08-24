using RtspClientSharp.RawFrames.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RtspClientSharp.MediaParsers
{
    /// <summary>
    /// Allows to parse incomming metadata from an rtsp data
    /// </summary>
    class MetadataPayloadParser : MediaPayloadParser
    {
        /// <summary>
        /// Usual size of the header in the communication
        /// </summary>
        private const int _headerSize = 16;

        /// To find the starting point of an xml metadata file
        private readonly byte[] xmlHeader = System.Text.Encoding.UTF8.GetBytes("<?xml");
        /// To find the ending point of an xml metadata file
        private readonly byte[] xmlEndMetadata = System.Text.Encoding.UTF8.GetBytes("</tt:MetadataStream>");

        /// <summary>
        /// Tries to get bytes in a byte array that are part of one xml metadata response in a rtsp communication
        /// 
        /// TODO: PARSE XML FILES COMMING FROM TWO DIFFERENT BYTE SEGMENTS
        /// </summary>
        /// <param name="timeOffset">Offset between prev. byteSegment</param>
        /// <param name="byteSegment">array of bytes incomming from an rtsp communication</param>
        /// <param name="markerBit">true if the xml file is closed, false otherwise</param>
        public override void Parse(TimeSpan timeOffset, ArraySegment<byte> byteSegment, bool markerBit)
        {
            Debug.Assert(byteSegment.Array != null, "byteSegment.Array != null");

            if (byteSegment.Count < _headerSize)
                throw new MediaPayloadParserException("Input data size is smaller than header size");

            var xmlFiles = new List<byte[]>();
            // get all possible xml files in an rtsp transmission
            xmlFiles = GetXmlFiles(xmlFiles, byteSegment.Array, markerBit);
    
            foreach(var xmlFile in xmlFiles)
            {
                // generate an array segment for each xml file found
                var xmlFileSegment = new ArraySegment<byte>(xmlFile, 0, xmlFile.Length);
                var xmlFrame = new MetadataFrame(DateTime.UtcNow, xmlFileSegment);
                OnFrameGenerated(xmlFrame);
            }
        }
        /// <summary>
        /// Gets all possible metadata xml files in a given array, this is done recursively
        /// </summary>
        /// <param name="xmlFiles">xml files found in array</param>
        /// <param name="array">array of bytes contaning 0, 1 or more xml files</param>
        /// <param name="markerBit">true if the xml file is closed, false otherwise</param>
        /// <returns>a list of array of bytes, each one of these arrays represents an xml file</returns>
        private List<byte[]> GetXmlFiles(List<byte[]> xmlFiles, byte[] array, bool markerBit)
        {
            int xmlStartIndex = IndexOf(array, xmlHeader);

            if (xmlStartIndex < 0)
                return xmlFiles;

            int xmlEndMetadataIndex = IndexOf(array, xmlEndMetadata);

            if (xmlEndMetadataIndex < 0) 
                return xmlFiles;
            
            xmlEndMetadataIndex += xmlEndMetadata.Length + 1;

            var xmlFile = array.Skip(xmlStartIndex).Take(xmlEndMetadataIndex - xmlStartIndex).ToArray();
            xmlFiles.Add(xmlFile);

            array = array.Skip(xmlEndMetadataIndex).ToArray();

            var hasData = array.Any(b => b != 0);

            if(!hasData)
                return xmlFiles;

            return GetXmlFiles(xmlFiles, array, markerBit);
        }
        /// <summary>
        /// Search a collection of arrays in a bigger array.
        /// It returns the start index if found, -1 otherwise
        /// </summary>
        /// <param name="arrayToSearchThrough">Base array to search through</param>
        /// <param name="patternToFind">Array to find in the provided base array</param>
        /// <returns></returns>
        private int IndexOf(byte[] array, byte[] pattern)
        {
            if (pattern.Length > array.Length)
                return -1;

            var len = pattern.Length;
            var limit = array.Length - len;
            for (var i = 0; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                {
                    if (pattern[k] != array[i + k]) break;
                }
                if (k == len) return i;
            }
            return -1;
        }
        /// <summary>
        /// If values needs to reset between byte segments, use this method
        /// </summary>
        public override void ResetState()
        {
        }
    }
}
