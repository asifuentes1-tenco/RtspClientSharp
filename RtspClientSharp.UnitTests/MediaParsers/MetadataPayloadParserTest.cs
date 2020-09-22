using Microsoft.VisualStudio.TestTools.UnitTesting;
using RtspClientSharp.MediaParsers;
using RtspClientSharp.RawFrames;
using RtspClientSharp.RawFrames.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RtspClientSharp.UnitTests.MediaParsers
{
    [TestClass]
    public class MetadataPayloadParserTest
    {
        [TestMethod]
        public void Parse_TestDataReception_ReturnsValidFrame()
        {
            string xml = File.ReadAllText(@"xml_test_event.xml", Encoding.UTF8);

            var frames = new List<MetadataFrame>();
            var parser = new MetadataPayloadParser
            {
                FrameGenerated = rawFrame => frames.Add((MetadataFrame)rawFrame)
            };

            parser.Parse(TimeSpan.Zero, new ArraySegment<byte>(Encoding.UTF8.GetBytes(xml)), true);

            Assert.AreEqual(1, frames.Count);
            Assert.AreEqual(FrameType.Metadata, frames[0].Type);
        }
        [TestMethod]
        public void Parse_TestDataReception_ReturnsValidFrameForMultipleXMLFilesInSameChunk()
        {
            string xml = File.ReadAllText(@"xml_test_various_event.xml", Encoding.UTF8);

            var frames = new List<MetadataFrame>();
            var parser = new MetadataPayloadParser
            {
                FrameGenerated = rawFrame => frames.Add((MetadataFrame)rawFrame)
            };

            parser.Parse(TimeSpan.Zero, new ArraySegment<byte>(Encoding.UTF8.GetBytes(xml)), true);

            Assert.AreEqual(3, frames.Count);
            Assert.AreEqual(FrameType.Metadata, frames[0].Type);
        }
    }
}
