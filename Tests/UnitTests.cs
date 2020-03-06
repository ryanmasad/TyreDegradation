using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyreDegradation;
using System;
using System.Collections.Generic;
using System.IO;

namespace Tests
{
    [TestClass]
    public class UnitTests
    {
        string basePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        string tyrePath = "\\TyresXML.xml";
        string trackPath ="\\TrackDegradationCoefficients.txt";
        /// <summary>
        /// Test that Tyre data loads successfully without throwing an exception
        /// </summary>
        [TestMethod]
        public void LoadTyreBuildsSuccessfully()
        {
            ViewModel viewmodel = new ViewModel();
            Dictionary<string, Tyre> dict = viewmodel.LoadTyreData(basePath + tyrePath);
            Assert.IsTrue(true);
        }
        /// <summary>
        /// Tests that all Tyre data loads correctly
        /// </summary>
        [TestMethod]
        public void LoadTyreCorrectSize()
        {
            ViewModel viewmodel = new ViewModel();
            Dictionary<string, Tyre> dict = viewmodel.LoadTyreData(basePath + tyrePath);
            Assert.IsTrue(dict.Count == 32);
        }
        /// <summary>
        /// Tests that Tyre data values loads correctly
        /// </summary>
        [TestMethod]
        public void LoadTyreWellFormed()
        {
            ViewModel viewmodel = new ViewModel();
            Dictionary<string, Tyre> dict = viewmodel.LoadTyreData(basePath + tyrePath);
            Assert.IsTrue(dict["SuperSoft - Front Tyre 4"].DegredationCoefficient == 15);
        }
        /// <summary>
        /// Tests that Track data loads successfully
        /// </summary>
        [TestMethod]
        public void LoadTrackCorrectSize()
        {
            ViewModel viewmodel = new ViewModel();
            Dictionary<string, Track> dict = viewmodel.LoadTrackData(basePath + trackPath);
            Assert.IsTrue(dict.Count == 7);
        }

        /// <summary>
        /// Tests that track temperature retrieved from API successfully
        /// </summary>
        [TestMethod]
        public void TrackTempSuccess()
        {
            ViewModel viewmodel = new ViewModel();
            viewmodel.LoadTrackData(basePath + trackPath);
            int temp = (viewmodel.Tracks["Silverstone"].Temperature);
            Assert.IsNotNull(temp);
        }
        /// <summary>
        /// Tests that Track data values loads correctly
        /// </summary>
        [TestMethod]
        public void LoadTrackWellFormed()
        {
            ViewModel viewmodel = new ViewModel();
            Dictionary<string, Track> dict = viewmodel.LoadTrackData(basePath + trackPath);
            Assert.IsTrue(dict["Silverstone"].Location == "Towcester");
        }

        /// <summary>
        /// Tests that CalculateDegradation returns results
        /// </summary>
        [TestMethod]
        public void TyreDegCalc()
        {
            ViewModel viewmodel = new ViewModel();
            Track track = viewmodel.Tracks["Silverstone"];
            Tyre tyre = viewmodel.Tyres["SuperSoft - Front Tyre 1"];
            int[] results = viewmodel.CalculateDegradation(track.Name, tyre.Name, 10);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void ModeTest()
        {
            ViewModel viewmodel = new ViewModel();

            int[] arr = { 1, 2, 3, 4, 4, 4, 5, 7, 8, 9, 9 };

            int result = viewmodel.getMode(arr);
            Assert.IsTrue(result == 4);
        }


    }
}
