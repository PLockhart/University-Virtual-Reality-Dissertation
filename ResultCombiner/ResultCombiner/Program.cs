using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Office.Interop.Excel;

namespace ResultCombiner
{
    class Program
    {
        const string RESULTS_FOLDER = "CombinedResults";
        const string PARTICIPANT_FOLDER = "Users";
        const string FIREWORK_FOLDER = "Firework";
        const string TAG_FOLDER = "Tag";

        Application _xlApp;

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

                try
                {

                    Worksheet exp1;
                    Worksheet exp2;
                    Worksheet exp3;
                    Worksheet exp4;

                    foreach (Worksheet loopedWorksheet in participantWb.Worksheets)
                    {
                        switch (loopedWorksheet.Name[4])
                        {
                            case '1':
                                generateChartForMeditationAndAttention(loopedWorksheet);
                                exp1 = loopedWorksheet;
                                break;
                            case '2': 
                                processFireworkExperiment(loopedWorksheet);
                                exp2 = loopedWorksheet;
                                break;
                            case '3':
                                processTagExperiment(loopedWorksheet);
                                exp3 = loopedWorksheet;
                                break;
                            case '4':
                                processBaseline(loopedWorksheet);
                                exp4 = loopedWorksheet;
                                break;
                                
                        }
                    }
                    /*
                    //now add baselines now that all the sheets have been processed
                    ChartObjects xlCharts = exp1.ChartObjects(Type.Missing);
                    ChartObject chartObject = xlCharts.Item(0);
                    Chart chart = chartObject.Chart;
                    */
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
            double runningTotal = 0;

            for (int i = 0; i < numAttentionValues; i++)
            {
                runningTotal += (int)ws.Cells[4 + i, 2].Value;
            }

            int averageAttention = (int)Math.Round(runningTotal / numAttentionValues);
            writeDataDownColumn(ws, Enumerable.Repeat<int>(averageAttention, numAttentionValues).ToArray(), 4, 3);

            int numMeditationValues = (int)ws.Cells[2, 5].Value;
            runningTotal = 0;

            for (int i = 0; i < numMeditationValues; i++)
            {
                runningTotal += (int)ws.Cells[4 + i, 5].Value;
            }

            int averageMeditation = (int)Math.Round(runningTotal / numMeditationValues);
            writeDataDownColumn(ws, Enumerable.Repeat<int>(averageMeditation, numMeditationValues).ToArray(), 4, 6);

            Chart generatedChart = generateChartForMeditationAndAttention(ws);

            SeriesCollection sc = generatedChart.SeriesCollection();
            Series series3 = sc.NewSeries();
            string endCell = (4 + numAttentionValues).ToString();

            series3.Values = ws.get_Range("C4", "C" + endCell);
            series3.XValues = ws.get_Range("A4", "A" + endCell);
            series3.Name = "Average Attention";
            series3.ChartType = XlChartType.xlLine;

            Series series4 = sc.NewSeries();
            endCell = (4 + numMeditationValues).ToString();

            series4.Values = ws.get_Range("F4", "F" + endCell);
            series4.XValues = ws.get_Range("D4", "D" + endCell);
            series4.Name = "Average Meditation";
            series4.ChartType = XlChartType.xlLine;
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
                setLowestTimeVars(eegStartTime, ref closestDate, ref closestFile, ref filePrefix, loopedFile, timeStr);
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

        private static Chart generateChartForMeditationAndAttention(Worksheet ws)
        {
            //create a graph from the data range
            ChartObjects xlCharts = ws.ChartObjects(Type.Missing);
            ChartObject chartObject = xlCharts.Add(600, 10, 750, 380);
            Chart chartPage = chartObject.Chart;
            chartPage.ChartType = XlChartType.xlLine;

            SeriesCollection sc = chartPage.SeriesCollection();
            Series series1 = sc.NewSeries();
            Series series2 = sc.NewSeries();

            int numValues = (int)ws.Cells[2, 2].Value;
            string endCell = (4 + numValues).ToString();

            series1.Values = ws.get_Range("B4", "B" + endCell);
            series1.XValues = ws.get_Range("A4", "A" + endCell);
            series1.Name = "Attention";

            numValues = (int)ws.Cells[2, 5].Value;
            endCell = (4 + numValues).ToString();

            series2.Values = ws.get_Range("E4", "E" + endCell);
            series2.XValues = ws.get_Range("D4", "D" + endCell);
            series2.Name = "Meditation";

            return chartPage;
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

                setLowestTimeVars(eegStartTime, ref closestDate, ref closestFile, ref filePrefix, loopedFile, timeStr);
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

        private void setLowestTimeVars(DateTime eegStartTime, ref DateTime closestDate, ref string closestFile, ref string filePrefix, string loopedFile, string timeStr)
        {
            DateTime fileTime = getDateTimeFromUnrealDateString(timeStr);

            //long ticks1 = Math.Abs(eegStartTime.Ticks - fileTime.Ticks);
            //long min = Math.Abs(eegStartTime.Ticks - closestDate.Ticks);

            if (Math.Abs((eegStartTime - fileTime).Ticks) < Math.Abs((eegStartTime - closestDate).Ticks))
            {
                closestDate = fileTime;
                closestFile = loopedFile;
                filePrefix = timeStr;
            }
        }

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
