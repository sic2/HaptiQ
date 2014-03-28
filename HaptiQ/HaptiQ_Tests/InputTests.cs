using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Input_API;

namespace HaptiQ_Tests
{
    [TestClass]
    public class InputTests
    {
        [TestMethod]
        public void DeviceTouchTest1()
        {
            byte[,] data = InputIdentifier.intToBinaryArray(1, 5);
            byte[,] expectedData = { { 0, 0, 0, 0, 0 }, 
                                     { 0, 0, 0, 0, 0 },
                                     { 0, 0, 0, 0, 0 },
                                     { 0, 0, 0, 0, 0 },
                                     { 0, 0, 0, 0, 1 }};

            CollectionAssert.AreEqual(expectedData, data);

            byte[,] data1 = InputIdentifier.intToBinaryArray(16777216UL, 5);
            byte[,] expectedData1 = { { 1, 0, 0, 0, 0 }, 
                                     { 0, 0, 0, 0, 0 },
                                     { 0, 0, 0, 0, 0 },
                                     { 0, 0, 0, 0, 0 },
                                     { 0, 0, 0, 0, 0 }};

            CollectionAssert.AreEqual(expectedData1, data1);

            byte[,] data2 = InputIdentifier.intToBinaryArray(16777217UL, 5);
            byte[,] expectedData2 = { { 1, 0, 0, 0, 0 }, 
                                     { 0, 0, 0, 0, 0 },
                                     { 0, 0, 0, 0, 0 },
                                     { 0, 0, 0, 0, 0 },
                                     { 0, 0, 0, 0, 1 }};

            CollectionAssert.AreEqual(expectedData2, data2);

            byte[,] data3 = InputIdentifier.intToBinaryArray(201024, 5);
            byte[,] expectedData3 = { { 0, 0, 0, 0, 0 }, 
                                     { 0, 0, 1, 1, 0 },
                                     { 0, 0, 1, 0, 0 },
                                     { 0, 1, 0, 1, 0 },
                                     { 0, 0, 0, 0, 0 }};

            CollectionAssert.AreEqual(expectedData3, data3);
        }

        [TestMethod]
        public void DeviceTouchTest2()
        {
            byte[,] data = { { 0, 0, 0, 0, 0 }, 
                            { 0, 0, 1, 1, 0 },
                            { 0, 0, 1, 0, 0 },
                            { 0, 1, 0, 1, 0 },
                            { 0, 0, 0, 0, 0 }};
            UInt64 result = InputIdentifier.binaryArrayToInt(data);
            Assert.AreEqual(201024UL, result);

            byte[,] data1 = { { 0, 0, 0, 0, 0 }, 
                            { 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0 }};
            result = InputIdentifier.binaryArrayToInt(data1);
            Assert.AreEqual(0UL, result);

            byte[,] data2 = { { 0, 0, 0, 0, 0 }, 
                            { 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 1 }};
            result = InputIdentifier.binaryArrayToInt(data2);
            Assert.AreEqual(1UL, result);

            byte[,] data3 = { { 1, 0, 0, 0, 0 }, 
                            { 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0 }};
            result = InputIdentifier.binaryArrayToInt(data3);
            Assert.AreEqual(16777216UL, result);
        }

        [TestMethod]
        public void DeviceTouchTest3()
        {
            Assert.AreEqual(InputIdentifier.TYPE.finger, InputIdentifier.stringToType("finger"));
            Assert.AreEqual(InputIdentifier.TYPE.finger, InputIdentifier.stringToType("FINGER"));
            Assert.AreEqual(InputIdentifier.TYPE.finger, InputIdentifier.stringToType("Finger"));
            Assert.AreEqual(InputIdentifier.TYPE.finger, InputIdentifier.stringToType("fingeR"));
            Assert.AreEqual(InputIdentifier.TYPE.finger, InputIdentifier.stringToType("finger "));
            Assert.AreEqual(InputIdentifier.TYPE.finger, InputIdentifier.stringToType(" finger"));
            Assert.AreEqual(InputIdentifier.TYPE.finger, InputIdentifier.stringToType(" finger "));
            Assert.AreEqual(InputIdentifier.TYPE.finger, InputIdentifier.stringToType("finger"));

            Assert.AreEqual(InputIdentifier.TYPE.tag, InputIdentifier.stringToType("tag"));
            Assert.AreEqual(InputIdentifier.TYPE.blob, InputIdentifier.stringToType("blob"));
            Assert.AreEqual(InputIdentifier.TYPE.glyph, InputIdentifier.stringToType("glyph"));
            Assert.AreEqual(InputIdentifier.TYPE.error, InputIdentifier.stringToType("---"));
            Assert.AreEqual(InputIdentifier.TYPE.error, InputIdentifier.stringToType(""));
            Assert.AreEqual(InputIdentifier.TYPE.error, InputIdentifier.stringToType(" "));
        }
    }
}
