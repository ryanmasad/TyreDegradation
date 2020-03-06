using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TyreDegradation
{
    public class ViewModel
    {
        public Dictionary<string, Tyre> Tyres { set; get; }
        public Dictionary<string, Track> Tracks { set; get; }

        // Methods to retrieve tyre and track data from files
        /// <summary>
        /// Loads tyre data from provided .xml file
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Dictionary with tyre data</returns>
        public Dictionary<string, Tyre> LoadTyreData(string path)

        {
            Dictionary<String, Tyre> TheseTyres = new Dictionary<String, Tyre>();
            XDocument tyreTypes = XDocument.Load(@path);

            foreach (XElement tyretype in tyreTypes.Elements("Tyres").Elements("Tyre"))
            {


                int degCo = 0;
                try
                {
                    degCo = Int32.Parse(tyretype.Element("DegradationCoefficient").Value);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }

                TheseTyres.Add(tyretype.Element("Name").Value, new Tyre(tyretype.Element("Name").Value,
                    tyretype.Element("Placement").Value,
                    tyretype.Element("Type").Value,
                    tyretype.Element("Family").Value,
                    degCo
                    ));

            }

            return TheseTyres;

        }
        /// <summary>
        /// Loads tyre data from provided .txt file
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Dictionary with Track data</returns>
        public Dictionary<string, Track> LoadTrackData(string path)
        {
            string line;
            Dictionary<String, Track> theseTracks = new Dictionary<string, Track>();

            System.IO.StreamReader file = new System.IO.StreamReader(@path);

            while ((line = file.ReadLine()) != null)
            {
                String name = "";
                String location = "";
                String degCos = "";

                int[] degCoefficients = new int[0];
                int temperature = 0;

                String[] tokens = line.Split("|");
                if (tokens.Length != 3)
                {
                    continue;
                }
                else
                {
                    name = tokens[0];
                    location = tokens[1];
                    degCos = tokens[2];

                    String[] degCosArray = degCos.Split(",");
                    degCoefficients = Array.ConvertAll(degCosArray, int.Parse);
                    
                }

                Track newTrack = new Track(name, location, degCoefficients, temperature);
                UpdateTrackTemp(newTrack);
                theseTracks.Add(name, newTrack);

            }
            return theseTracks;


        }

        /// <summary>
        /// Retrieves the current temperature at the currently selected circuit from the openweathermap API and posts it to the temperature field - used for testing purposes only
        /// </summary>
        /// <param name="location"></param>
        /// <returns>Current temperature at given location from API designated in specification</returns>
        public void UpdateTrackTemp(Track track)
        {
            track.GetCurrentTempAsync();

        }

        /// <summary>
        /// Calculates the mean, mode and range of the set of point tyre degradation values for each track deg point,
        /// temperature and tyre coefficient, using the formula given in the specification
        /// </summary>
        /// <param name="track"></param>
        /// <param name="tyreType"></param>
        /// <param name="temperature"></param>
        /// <returns>An array of size 3 containing the results for each run</returns>
        public int[] CalculateDegradation(String track, String tyreType, int temperature)
        {
            Track currentTrack = Tracks[track];
            int[] degPoints = currentTrack.DegCoefficients;
    

            Tyre currentTyre = Tyres[tyreType];
            int tyreCoef = currentTyre.DegredationCoefficient;


            int[] pointDegs = new int[degPoints.Length];
            int index = 0;
            foreach (int degPointValue in degPoints)
            {
                double pointDeg = (degPointValue * temperature) / tyreCoef;
                pointDegs[index] = (int)Math.Round(pointDeg);
                index++;
            }

            //resultArray = {<average>,<mode>,<range>}
            int[] resultArray = new int[3];
            resultArray[0] = (int)Math.Round(pointDegs.Average());
            resultArray[1] = getMode(pointDegs);
            resultArray[2] = pointDegs.Max() - pointDegs.Min();

            return resultArray;

        }

        /// <summary>
        /// Gets the mode of a given int array
        /// </summary>
        /// <param name="arr"></param>
        /// <returns>mode of input array</returns>
        public int getMode(int[] arr)
        {
            int maxValue = arr.Max();

            int sizeOfPlaceholderArray = maxValue + 1;
            int[] countingArray = new int[sizeOfPlaceholderArray];
            for (int index = 0; index < sizeOfPlaceholderArray; index++)
                countingArray[index] = 0;

            for (int index = 0; index < arr.Length; index++)
                countingArray[arr[index]]++;

            // mode is the index with maximum count 
            int mode = 0;
            int currentHighCount = countingArray[0];
            for (int index = 1; index < sizeOfPlaceholderArray; index++)
            {
                if (countingArray[index] > currentHighCount)
                {
                    currentHighCount = countingArray[index];
                    mode = index;
                }
            }
            return mode;
        }

        /// <summary>
        /// Constructor of the ViewModel loads tyre and track data from given files
        /// </summary>
        public ViewModel()
        {
            Tyres = new Dictionary<string, Tyre>();
            Tracks = new Dictionary<string, Track>();
            Tyres = LoadTyreData(@"C:\Users\User\Documents\Renault Coding Challenge\TyreDegradation\TyreDegradation\TyresXML.xml");
            Tracks = LoadTrackData(@"C:\Users\User\Documents\Renault Coding Challenge\TyreDegradation\TyreDegradation\TrackDegradationCoefficients.txt");
        }
    }
}
