using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robots;
using Robots.Controllers;

namespace Robots.Tests.Controllers
{
    [TestClass]
    public class RobotsCollectionTests
    {
        [TestMethod]
        public void FirstTest()
        {
            // Arrange
            RobotsCollection grid = new RobotsCollection();
            grid.InitializeGrid(5, 3);
            grid.AddRobot("1 1 E", "RFRFRFRF");

            // Act

            // Assert
            string output = grid.GetOutput()
                .Replace(System.Environment.NewLine, String.Empty)
                .Trim();

            Assert.AreEqual("1 1 E", output);

            // Write out
            Console.WriteLine(output);
        }

        [TestMethod]
        public void SecondTest()
        {
            // Arrange
            RobotsCollection grid = new RobotsCollection();
            grid.InitializeGrid(5, 3);
            grid.AddRobot("3 2 N", "FRRFLLFFRRFLL");

            // Act

            // Assert
            string output = grid.GetOutput()
                .Replace(System.Environment.NewLine, String.Empty)
                .Trim();

            Assert.AreEqual("3 3 N LOST", output);

            // Write out
            Console.WriteLine(output);
        }

        [TestMethod]
        public void ThirdTest()
        {
            // Arrange
            RobotsCollection grid = new RobotsCollection();
            grid.InitializeGrid(5, 3);
            grid.AddRobot("03 W", "LLFFFLFLFL");

            // Act


            // Assert
            string output = grid.GetOutput()
                .Replace(System.Environment.NewLine, String.Empty)
                .Trim();

            Assert.AreEqual("2 3 S", output);

            // Write out
            Console.WriteLine(output);
        }
    }
}
