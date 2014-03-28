using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Input_API;
using HaptiQ_API;

// mock framework: https://github.com/Moq/moq4 
namespace HaptiQ_Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {

        public UnitTest1()
        {}

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
        }

        [ClassCleanup()]
        public static void cleanup()
        {
       
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void HelperDistanceBetweenTwoPointsTest()
        {
            double dist = Helper.distanceBetweenTwoPoints(new Point(0, 0), new Point(0, 10));
            Assert.AreEqual(10, dist);

            dist = Helper.distanceBetweenTwoPoints(new Point(0, 0), new Point(10, 0));
            Assert.AreEqual(10, dist);

            dist = Helper.distanceBetweenTwoPoints(new Point(10, 0), new Point(0, 0));
            Assert.AreEqual(10, dist);

            dist = Helper.distanceBetweenTwoPoints(new Point(0, 10), new Point(0, 0));
            Assert.AreEqual(10, dist);

            dist = Helper.distanceBetweenTwoPoints(new Point(0, 0), new Point(4, 3));
            Assert.AreEqual(5, dist);

            dist = Helper.distanceBetweenTwoPoints(new Point(0, -10), new Point(0, 0));
            Assert.AreEqual(10, dist);

            dist = Helper.distanceBetweenTwoPoints(new Point(0, 0), new Point(-10, 0));
            Assert.AreEqual(10, dist);

            dist = Helper.distanceBetweenTwoPoints(new Point(-10, 0), new Point(0, 0));
            Assert.AreEqual(10, dist);

            dist = Helper.distanceBetweenTwoPoints(new Point(0, 0), new Point(0, -10));
            Assert.AreEqual(10, dist);
        }

        [TestMethod]
        public void HelperFindNearestPointsTest()
        {
            Tuple<Point, Point> nearestPoints = Helper.findNearestPoints(null, null);
            Assert.IsNull(nearestPoints);

            List<Point> list1 = new List<Point>();
            List<Point> list2 = new List<Point>();
            nearestPoints = Helper.findNearestPoints(list1, list2);
            Assert.IsNull(nearestPoints);

            list1.Add(new Point(0, 0));
            nearestPoints = Helper.findNearestPoints(list1, list2);
            Assert.IsNull(nearestPoints);
            nearestPoints = Helper.findNearestPoints(list2, list1);
            Assert.IsNull(nearestPoints);

            list2.Add(new Point(100, 100));
            nearestPoints = Helper.findNearestPoints(list1, list2);
            Assert.AreNotEqual(nearestPoints, new Tuple<Point, Point>(new Point(0, 0), new Point(0, 0)));
            Assert.AreEqual(nearestPoints, new Tuple<Point, Point>(new Point(0, 0), new Point(100, 100)));

            list2.Add(new Point(10, 10));
            nearestPoints = Helper.findNearestPoints(list1, list2);
            Assert.AreNotEqual(nearestPoints, new Tuple<Point, Point>(new Point(0, 0), new Point(100, 100)));
            Assert.AreEqual(nearestPoints, new Tuple<Point, Point>(new Point(0, 0), new Point(10, 10)));
        }
    }
}
