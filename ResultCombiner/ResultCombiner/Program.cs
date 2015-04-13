using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Office.Interop.Excel;

//TODO: create graph of average of all results from all experiments
//compare average increase/decreate between experiment and baseline
//then compare average increase/decreate between VR and non-VR

namespace ResultCombiner
{
    struct ExpResultStore
    {
        /// <summary>
        /// values for the average attention from each participant in this expeirment (non-VR)
        /// </summary>
        public List<int> nonVRAttention;
        /// <summary>
        /// values for the average attention from each participant in this expeirment (VR)
        /// </summary>
        public List<int> vrAttention;

        /// <summary>
        /// values for the average meditation from each participant in this expeirment (non-VR)
        /// </summary>
        public List<int> nonVRMeditation;
        /// <summary>
        /// values for the average meditation from each participant in this expeirment (VR)
        /// </summary>
        public List<int> vrMeditation;

        /// <summary>
        /// values for the difference between each participants average values for attention in the VR and non-VR version of the experiment
        /// </summary>
        public List<int> deltaVRnonVRAttention;
        /// <summary>
        /// values for the difference between each participants average values for attention in the VR compared to the average value for their baseline
        /// </summary>
        public List<int> deltaVRandBaselineAttention;
        /// <summary>
        /// values for the difference between each participants average values for attention in the non-VR compared to the average value for their baseline
        /// </summary>
        public List<int> deltaNonVRandBaselineAttention;

        /// <summary>
        /// values for the difference between each participants average values for meditation in the VR and non-VR version of the experiment
        /// </summary>
        public List<int> deltaVRnonVRMeditation;
        /// <summary>
        /// values for the difference between each participants average values for meditation in the VR compared to the average value for their baseline
        /// </summary>
        public List<int> deltaVRandBaselineMeditation;
        /// <summary>
        /// values for the difference between each participants average values for meditation in the non-VR compared to the average value for their baseline
        /// </summary>
        public List<int> deltaNonVRandBaselineMeditation;

        public ExpResultStore()
        {
            nonVRAttention = new List<int>();
            vrAttention = new List<int>();
            nonVRMeditation = new List<int>();
            vrMeditation = new List<int>();
            deltaVRnonVRAttention = new List<int>();
            deltaVRandBaselineAttention = new List<int>();
            deltaNonVRandBaselineAttention = new List<int>();
            deltaVRnonVRMeditation = new List<int>();
            deltaVRandBaselineMeditation = new List<int>();
            deltaNonVRandBaselineMeditation = new List<int>();
        }
    }

    class Program
    {
        const string RESULTS_FOLDER = "CombinedResults";
        const string PARTICIPANT_FOLDER = "Users";
        const string FIREWORK_FOLDER = "Firework";
        const string TAG_FOLDER = "Tag";

        Application _xlApp;

        #region Changed on a per-particpant basis

        int _baselineAttention;
        int _baselineMeditation;

        #endregion

        #region Rolling results

        ExpResultStore _exp1Store;
        ExpResultStore _exp2Store;
        ExpResultStore _exp3Store;

        #endregion

        static void Main(string[] args)
        {

            new Program();
        }

        Program()
        {
            setupEnvironment();

            string[] participantFiles = Directory.GetFiles(PARTICIPANT_FOLDER);

            if (participantFiles.Length == 0)
                throw new FileNotFoundException("No Users folder found");

            //each excel file has a deconstructor file associated with it for some reason
            for (int i = 0; i < participantFiles.Length; i += 2)
            {
                string resultsFilepath = RESULTS_FOLDER + "\\" + Path.GetFileName(participantFiles[i]);

                if (File.Exists(resultsFilepath) == true)
                    File.Delete(resultsFilepath);

                File.Copy(participantFiles[0], resultsFilepath);

                Workbook participantWb = _xlApp.Workbooks.Open(Path.GetFullPath(resultsFilepath), Type.Missing, false);

                _baselineAttention = 0;
                _baselineMeditation = 0;

                //do a 1st pass to find the baseline experiment to get averages
                foreach (Worksheet loopedWorksheet in participantWb.Worksheets)
                {
                    if (loopedWorksheet.Name == "Participant")
                    {
                        processBaseline(loopedWorksheet);
                    }
                }

                try
                {
                    foreach (Worksheet loopedWorksheet in participantWb.Worksheets)
                    {
                        //the 4th character in the name dictates the experiment type
                        switch (loopedWorksheet.Name[4])
                        {
                            case '1':
                                generateChartForMeditationAndAttention(loopedWorksheet);
                                break;
                            case '2': 
                                processFireworkExperiment(loopedWorksheet);
                                break;
                            case '3':
                                processTagExperiment(loopedWorksheet);
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error parsing " + participantWb.Name + ": " + e.ToString());
                }

                participantWb.Close(true);
            }
        }

        private DateTime getDateTimeFromUnrealDateString(string input)
        {
            string[] timeSplit = input.Split(new char[] { '.' });

            //how far from the end of the array is the seconds and min components
            int secOffset = 1;
            int minOffset = 2;
            bool milisecondsTrimmed = false;

            //if miliseconds are excluded, cut them off and adjust the offset as necessary
            if (timeSplit.Length == 6)
            {
                timeSplit[5] = "";
                secOffset++;
                minOffset++;
                milisecondsTrimmed = true;
            }

            //remove excess seconds precision
            if (timeSplit[timeSplit.Length - secOffset].Length > 2)
                timeSplit[timeSplit.Length - secOffset] = timeSplit[timeSplit.Length - secOffset].Remove(timeSplit[timeSplit.Length - secOffset].Length - 1);

            if (timeSplit[timeSplit.Length - minOffset].Length > 2)
                timeSplit[timeSplit.Length - minOffset] = timeSplit[timeSplit.Length - minOffset].Remove(timeSplit[timeSplit.Length - minOffset].Length - 1);

            input = string.Join(".", timeSplit);

            //remove a redundent fullstop as the last string index will just be an empty string
            if (milisecondsTrimmed == true)
                input = input.Remove(input.Length - 1);

            return DateTime.ParseExact(input, new string[] {
                "yyyy.MM.dd-HH.mm.ss",
                "yyyy.MM.dd-HH.mmm.ss",
                "yyyy.MM.dd-HH.mm.sss" },
                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);

        }

        private DateTime getDateTimeFromCellCoord(int row, int column, Worksheet ws)
        {
            string cellTimeStr = ws.Cells[row, column].Value.ToString();
            return DateTime.ParseExact(cellTimeStr, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        }

        private void processBaseline(Worksheet ws)
        {
            //average all the attention values
            int numAttentionValues = (int)ws.Cells[2, 2].Value;
            _baselineAttention = (int)ws.Cells[2, 3].Value;
            int numMeditationValues = (int)ws.Cells[2, 5].Value;
            _baselineMeditation = (int)ws.Cells[2, 6].Value;

            
            Chart generatedChart = generateChartForMeditationAndAttention(ws);
        }

        private void processTagExperiment(Worksheet ws)
        {
            string[] tagFiles = Directory.GetFiles(TAG_FOLDER);

            if (tagFiles.Length == 0)
                throw new FileNotFoundException("No tag folder found");

            DateTime eegStartTime = getDateTimeFromCellCoord(1, 2, ws);
            DateTime eegEndTime = getDateTimeFromCellCoord(1, 3, ws);
            DateTime closestDate = DateTime.MinValue;
            string closestFile = "";
            string filePrefix = "";

            for (int i = 0; i < tagFiles.Length; i++)
            {
                string loopedFile = tagFiles[i];

                string timeStr = Path.GetFileNameWithoutExtension(loopedFile);
                setLowestTimeFileAndDate(eegStartTime, ref closestDate, ref closestFile, ref filePrefix, loopedFile, timeStr);
            }

            //read the tags file
            double[] tagStamps = getNormalisedTimeStamps(eegStartTime, eegEndTime,
                File.ReadAllText(TAG_FOLDER + "\\" + filePrefix + ".txt"));

            //add the stamps to the sheet
            ws.Cells[3, 10] = "Tag times";
            ws.Cells[3, 11] = tagStamps.Length;
            writeDataDownColumn(ws, tagStamps, 4, 10);
            writeDataDownColumn(ws, Enumerable.Repeat<int>(50, tagStamps.Length).ToArray(), 4, 11);

            Chart generatedChart = generateChartForMeditationAndAttention(ws);

            SeriesCollection sc = generatedChart.SeriesCollection();
            Series series3 = sc.NewSeries();
            int numValues = (int)ws.Cells[3, 11].Value;
            string endCell = (3 + numValues).ToString();

            series3.Values = ws.get_Range("K4", "K" + endCell);
            series3.XValues = ws.get_Range("J4", "J" + endCell);
            series3.Name = "Tag Stamps";
            series3.ChartType = XlChartType.xlXYScatter;
            series3.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypePercent, 100);
        }

        /// <summary>
        /// Generates a chart for the medtitation and attention values in this worksheet, along with the baseline series 
        /// </summary>
        private Chart generateChartForMeditationAndAttention(Worksheet ws)
        {
            //create a graph from the data range
            ChartObjects xlCharts = ws.ChartObjects(Type.Missing);
            ChartObject chartObject = xlCharts.Add(600, 10, 750, 380);
            Chart chartPage = chartObject.Chart;
            chartPage.ChartType = XlChartType.xlLine;

            SeriesCollection sc = chartPage.SeriesCollection();

            createSeriesForColumnDataWithAverage(ws, sc, 2, 1, "Attention", _baselineAttention);
            createSeriesForColumnDataWithAverage(ws, sc, 2, 4, "Meditation", _baselineMeditation);

            return chartPage;
        }

        /// <summary>
        /// Converts an index to Excel column string
        /// </summary>
        public static string getColNameFromIndex(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        /// <summary>
        /// Creates a series from a column of values with the following excel coordinate format:
        /// (0, 1) is a number of the total values
        /// (2, 0) is where the timestamps start
        /// (2, 1) is where the values start
        /// (x, 3) is a blank column beside the values that is free space for writing
        /// </summary>
        /// <param name="ws">The worksheet we will be getting the values from</param>
        /// <param name="sc">The graph series collection we will be adding to</param>
        /// <param name="templateStartRow">The origin row of the data format</param>
        /// <param name="templateStartColumn">The origin column of the data format</param>
        /// <param name="seriesName">The name the raw and average series should take</param>
        /// <param name="average">The average value to use to draw the graph</param>
        private void createSeriesForColumnDataWithAverage(Worksheet ws, SeriesCollection sc, int templateStartRow, int templateStartColumn, string seriesName, int average)
        {
            Series series1 = sc.NewSeries();

            int dataStartRow = templateStartRow + 2;

            //the raw values
            int numValues = (int)ws.Cells[templateStartRow, templateStartColumn + 1].Value;
            string endCell = (dataStartRow + numValues).ToString();

            string rawDataColumn = getColNameFromIndex(templateStartColumn + 1);
            string rawDataTimeColumn = getColNameFromIndex(templateStartColumn);

            //create series from the raw data
            series1.Values = ws.get_Range(rawDataColumn + dataStartRow.ToString(), rawDataColumn + endCell);
            series1.XValues = ws.get_Range(rawDataTimeColumn + dataStartRow.ToString(), rawDataTimeColumn + endCell);
            series1.Name = seriesName;

            //repeat the baseline data beside it
            writeDataDownColumn(ws, Enumerable.Repeat<int>(_baselineAttention, numValues).ToArray(), dataStartRow, templateStartColumn + 2);

            //create a new series for average values
            Series baselineASeries = sc.NewSeries();
            endCell = (dataStartRow + numValues).ToString();

            string rawDataAverageColumn = getColNameFromIndex(templateStartColumn + 2);

            //create a series from the repeated average value
            series1.Values = ws.get_Range(rawDataAverageColumn + dataStartRow.ToString(), rawDataAverageColumn + endCell);
            series1.XValues = ws.get_Range(rawDataTimeColumn + dataStartRow.ToString(), rawDataTimeColumn + endCell);
            baselineASeries.Name = "Average " + seriesName;
            baselineASeries.ChartType = XlChartType.xlLine;
        }

        private void processFireworkExperiment(Worksheet ws)
        {
            string[] fireworkFiles = Directory.GetFiles(FIREWORK_FOLDER);

            if (fireworkFiles.Length == 0)
                throw new FileNotFoundException("No fireworks folder found");

            DateTime eegStartTime = getDateTimeFromCellCoord(1, 2, ws);
            DateTime eegEndTime = getDateTimeFromCellCoord(1, 3, ws);
            DateTime closestDate = DateTime.MinValue;
            string closestFile = "";
            string filePrefix = "";

            //+= 2 because there is an explode and spawn file for each run of the experiment
            for (int i = 0; i < fireworkFiles.Length; i += 2)
            {
                string loopedFile = fireworkFiles[i];

                string[] fileNameSplit = Path.GetFileNameWithoutExtension(loopedFile).Split(new char[] { ' ' });
                string timeStr = fileNameSplit[0];

                setLowestTimeFileAndDate(eegStartTime, ref closestDate, ref closestFile, ref filePrefix, loopedFile, timeStr);
            }
            //read the explosions file
            double[] explosionStamps = getNormalisedTimeStamps(eegStartTime, eegEndTime,
                File.ReadAllText(FIREWORK_FOLDER + "\\" + filePrefix + " Explode.txt"));
            double[] spawnStamps = getNormalisedTimeStamps(eegStartTime, eegEndTime,
                File.ReadAllText(FIREWORK_FOLDER + "\\" + filePrefix + " Spawn.txt"));

            //add the stamps to the sheet
            ws.Cells[3, 10] = "Spawn times";
            ws.Cells[3, 11] = spawnStamps.Length;
            writeDataDownColumn(ws, spawnStamps, 4, 10);
            writeDataDownColumn(ws, Enumerable.Repeat<int>(50, spawnStamps.Length).ToArray(), 4, 11);

            ws.Cells[3, 13] = "Explosion times";
            ws.Cells[3, 14] = explosionStamps.Length;
            writeDataDownColumn(ws, explosionStamps, 4, 13);
            writeDataDownColumn(ws, Enumerable.Repeat<int>(50, explosionStamps.Length).ToArray(), 4, 14);

            Chart generatedChart = generateChartForMeditationAndAttention(ws);

            SeriesCollection sc = generatedChart.SeriesCollection();
            Series series3 = sc.NewSeries();
            int numValues = (int)ws.Cells[3, 11].Value;
            string endCell = (3 + numValues).ToString();

            series3.Values = ws.get_Range("K4", "K" + endCell);
            series3.XValues = ws.get_Range("J4", "J" + endCell);
            series3.Name = "Spawn Stamps";
            series3.ChartType = XlChartType.xlXYScatter;
            series3.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypePercent, 100);

            Series series4 = sc.NewSeries();
            numValues = (int)ws.Cells[3, 14].Value;
            endCell = (3 + numValues).ToString();

            series4.Values = ws.get_Range("N4", "N" + endCell);
            series4.XValues = ws.get_Range("M4", "M" + endCell);
            series4.Name = "Explosion Stamps";
            series4.ChartType = XlChartType.xlXYScatter;
            series4.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypePercent, 100);
        }

        /// <summary>
        /// Sets the lowest datetime out of the parameters in comparions to the startTime.
        /// </summary>
        /// <param name="eegStartTime">The time all dates will be compared to</param>
        /// <param name="closestDate">The closest date so far</param>
        /// <param name="closestFile">The file associated with the closest date</param>
        /// <param name="filePrefix">The original unreal time string</param>
        /// <param name="loopedFile">Canidate for the closest file</param>
        /// <param name="timeStr">Canditate for the closest time</param>
        private void setLowestTimeFileAndDate(DateTime eegStartTime, ref DateTime closestDate, ref string closestFile, ref string filePrefix, string loopedFile, string timeStr)
        {
            DateTime fileTime = getDateTimeFromUnrealDateString(timeStr);

            if (Math.Abs((eegStartTime - fileTime).Ticks) < Math.Abs((eegStartTime - closestDate).Ticks))
            {
                closestDate = fileTime;
                closestFile = loopedFile;
                filePrefix = timeStr;
            }
        }

        /// <summary>
        /// Writes the array of data down the row and column, in excel coordinates
        /// </summary>
        private static void writeDataDownColumn<T>(Worksheet ws, T[] data, int startRow, int startColumn)
        {
            for (int i = 0; i < data.Length; i++)
            {
                ws.Cells[startRow + i, startColumn] = data[i];
            }
        }

        /// <summary>
        /// Sets the timestamps from their own time space to the eeg start time. Method excludes timetamps less than the eeg start
        /// and greater thant the eeg end.
        /// Assumes the data is comma then space seperated, and the 1st data is the timestamp of the start of recording
        /// </summary>
        /// <param name="eegStartTime"></param>
        /// <param name="eegEndTime"></param>
        /// <param name="fileData"></param>
        /// <returns>The timestamps under the domain of eegStartTime</returns>
        private double[] getNormalisedTimeStamps(DateTime eegStartTime, DateTime eegEndTime, string fileData)
        {
            string[] splitData = fileData.Split(new char[] { ' ' });
            //ignore the 1st value as that is always the starting timestmap
            List<double> timeStamps = new List<double>();

            DateTime dataStartDate = getDateTimeFromUnrealDateString(splitData[0].Replace(',', ' '));

            //1st item is the start date
            for (int i = 1; i < splitData.Length; i++)
            {
                double tempStamp = double.Parse(splitData[i].Replace(',', ' '));

                DateTime tempDate = dataStartDate;
                tempDate = tempDate.AddSeconds(tempStamp);

                if (tempDate > eegStartTime && tempDate < eegEndTime)
                    //now change the timestamps to now be timestamps from the experiment start time, rather than the unreal engine start time
                    timeStamps.Add((tempDate - eegStartTime).TotalSeconds);
            }

            return timeStamps.ToArray();
        }

        private void setupEnvironment()
        {
            _xlApp = new Application();
            if (_xlApp == null)
                Console.WriteLine("Excel is not properly installed, cannot save experiments");

            if (Directory.Exists(RESULTS_FOLDER) == false)
                Directory.CreateDirectory(RESULTS_FOLDER);
        }

        ~Program()
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_xlApp);
                _xlApp = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error releasing xml app: " + ex);
            }
        }
    }
}
