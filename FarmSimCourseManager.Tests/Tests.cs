using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using FarmSimCourseManager.CourseFileData;
using FarmSimCourseManager.Tools.CanvasEx;
using NUnit.Framework;

namespace FarmSimCourseManager.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestOpenSaveFiles()
        {
            var path = "E:\\Temp\\xml";
            var fileOpen = "courseplay.xml";
            var fileSave = "courseplay_gen.xml";
            var coursePlay = CourseFileManger.OpenFile(Path.Combine(path, fileOpen));
            CourseFileManger.SaveFile(Path.Combine(path, fileSave), coursePlay);

        }

        [Test]
        public void TestWindowRects()
        {
            var value = -2d;
            var size = 3;
            var rez = IndexLT(value, size);
            Assert.AreEqual(rez, -1);

            value = -5d;
            size = 3;
            rez = IndexLT(value, size);
            Assert.AreEqual(rez, -2);

            value = 2d;
            size = 3;
            rez = IndexLT(value, size);
            Assert.AreEqual(rez, 0);

            value = 5d;
            size = 3;
            rez = IndexLT(value, size);
            Assert.AreEqual(rez, 1);
            // RB
            value = -2d;
            size = 3;
            rez = IndexRB(value, size);
            Assert.AreEqual(rez, 0);

            value = -5d;
            size = 3;
            rez = IndexRB(value, size);
            Assert.AreEqual(rez, -1);

            value = 2d;
            size = 3;
            rez = IndexRB(value, size);
            Assert.AreEqual(rez, 1);

            value = 5d;
            size = 3;
            rez = IndexRB(value, size);
            Assert.AreEqual(rez, 2);

            //var rect = new Rect(new Point(-900, -900), new Point(900, 900));
            //var sizeWindow = 1000;
            //var startPoint = new Point(IndexLT(rect.X, sizeWindow), IndexLT(rect.Y, sizeWindow));
            //Assert.AreEqual(startPoint.X, -1);
            //Assert.AreEqual(startPoint.Y, -1);

            //rect = new Rect(new Point(-1100, -1100), new Point(900, 900));
            //startPoint = new Point(IndexLT(rect.X, sizeWindow), IndexLT(rect.Y, sizeWindow));
            //Assert.AreEqual(startPoint.X, -2);
            //Assert.AreEqual(startPoint.Y, -2);
        }

        private int IndexLT(double point, int size)
        {
            var index = (int)(point / size);
            return point < 0 ? index - 1 : index;
        }
        private int IndexRB(double point, int size)
        {
            var index = (int)(point / size);
            return point > 0 ? index + 1 : index;
        }
    }
}
