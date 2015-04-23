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
    struct ResultsPair<T>
    {
        public T nonVR;
        public T vr;
    }

    class UserScore
    {
        public int score;
        public string name;

        public UserScore(string user)
        {
            name = user;
        }

        public override string ToString()
        {
            return name + " (" + score + ")";
        }
    }

    class Program
    {
        const string MAIN_DIR = @"J:\Users\Peter\SkyDrive\Documents\CSC 4001\ExperimentResults\";
        const string RESULTS_FOLDER = "CombinedResults";
        const string PARTICIPANT_FOLDER = "Users";
        const string FIREWORK_FOLDER = "Firework";
        const string TAG_FOLDER = "Tag";

        Application _xlApp;

        #region Changed on a per-particpant basis

        Worksheet _participantWs;

        int _baselineAttention;
        int _baselineMeditation;

        ResultsPair<int> _exp1Attention;
        ResultsPair<int> _exp1Meditation;

        ResultsPair<int> _exp2Attention;
        ResultsPair<int> _exp2Meditation;

        ResultsPair<int> _exp3Attention;
        ResultsPair<int> _exp3Meditation;

        int _relaxScore;
        int _fireworkScore;
        int _tagScore;
        int _gameExperience;
        int _vrMapScore;
        int _controlSchemeRate;

        #endregion

        #region Rolling results

        ExpResultStore _exp1Store;
        Exp2Store _exp2Store;
        Exp3Store _exp3Store;

        QuestionaireResultStore<ExpResultStore> _exp1QStore;
        QuestionaireResultStore<Exp2Store> _exp2QStore;
        QuestionaireResultStore<Exp3Store> _exp3QStore;

        int _numFemales = 0;
        int _numMales = 0;

        List<int> _relaxScores;
        List<int> _fireworkScores;
        List<int> _tagScores;
        List<int> _gameExperiences;
        List<int> _vrMapScores;
        List<int> _controlSchemeRates;
        List<int> _vrExperiences;

        #endregion

        #region awards

        UserScore _lowestMedBase;
        UserScore _lowestAttBase;
        UserScore _mostFireworks;
        UserScore _leastFireworks;
        UserScore _fastestTagVR;
        UserScore _fastestTagKB;
        UserScore _slowestTagKB;
        UserScore _mostRelaxed;
        UserScore _longestEEG;

        UserScore _plowestMedBase;
        UserScore _plowestAttBase;
        UserScore _pmostFireworks;
        UserScore _pleastFireworks;
        UserScore _pfastestTagVR;
        UserScore _pfastestTagKB;
        UserScore _plowestTagKB;
        UserScore _pmostRelaxed;
        UserScore _plongestEEG;

        #endregion

        static void Main(string[] args)
        {
            new Program();
        }

        Program()
        {
            setupEnvironment();

            string[] participantFiles = Directory.GetFiles(MAIN_DIR + PARTICIPANT_FOLDER);

            if (participantFiles.Length == 0)
                throw new FileNotFoundException("No Users folder found");

            bool errorParsing = false;

            for (int i = 0; i < participantFiles.Length; i++)
            //for (int i = 0; i < 3; i++)
            {
                   
                //each excel file has a deconstructor file associated with it for some reason
                if (participantFiles[i].Contains("~") == true)
                    continue;

                string resultsFilepath = MAIN_DIR + RESULTS_FOLDER + "\\" + Path.GetFileName(participantFiles[i]);

                if (File.Exists(resultsFilepath) == true)
                    File.Delete(resultsFilepath);

                File.Copy(participantFiles[i], resultsFilepath);

                Workbook participantWb = _xlApp.Workbooks.Open(Path.GetFullPath(resultsFilepath), Type.Missing, false);

                _baselineAttention = 0;
                _baselineMeditation = 0;
                _participantWs = null;

                string fileNameNoExt = Path.GetFileNameWithoutExtension(participantFiles[i]);
                Console.WriteLine("Processing " + fileNameNoExt);

                _plowestMedBase = new UserScore(fileNameNoExt);
                _plowestAttBase = new UserScore(fileNameNoExt);
                _pmostFireworks = new UserScore(fileNameNoExt);
                _pfastestTagVR = new UserScore(fileNameNoExt);
                _pfastestTagKB = new UserScore(fileNameNoExt);
                _pmostRelaxed = new UserScore(fileNameNoExt);
                _pmostRelaxed.score = 100;
                _plongestEEG = new UserScore(fileNameNoExt);
                _pleastFireworks = new UserScore(fileNameNoExt);
                _plowestTagKB = new UserScore(fileNameNoExt);

                try
                {
                    //do a 1st pass to find the baseline experiment to get averages
                    foreach (Worksheet loopedWorksheet in participantWb.Worksheets)
                    {
                        if (loopedWorksheet.Name == "Exp Baseline")
                        {
                            Console.WriteLine("Generating Baseline EEG.......");
                            processBaseline(loopedWorksheet);
                        }
                        else if (loopedWorksheet.Name == "Participant")
                        {
                            Console.WriteLine("Gathering Participant Questionaaire results.......");

                            _participantWs = loopedWorksheet;

                            _relaxScore = (int)_participantWs.Cells[3, 4].Value;
                            _fireworkScore = (int)_participantWs.Cells[4, 4].Value;
                            _tagScore = (int)_participantWs.Cells[5, 4].Value;
                            _gameExperience = (int)_participantWs.Cells[8, 4].Value;
                            _vrMapScore = (int)_participantWs.Cells[7, 4].Value;
                            _controlSchemeRate = (int)_participantWs.Cells[10, 4].Value;

                            _relaxScores.Add(_relaxScore);
                            _fireworkScores.Add(_fireworkScore);
                            _tagScores.Add(_tagScore);
                            _gameExperiences.Add(_gameExperience);
                            _vrMapScores.Add(_vrMapScore);
                            _controlSchemeRates.Add(_controlSchemeRate);
                            _vrExperiences.Add((int)_participantWs.Cells[9, 4].Value);

                            if ((string)_participantWs.Cells[1, 2].Value == "Male")
                                _numMales++;
                            else
                                _numFemales++;

                            addBasicScoresToStore(_exp1QStore, _relaxScore - 1, _controlSchemeRate, _vrMapScore, _gameExperience);
                            addBasicScoresToStore(_exp2QStore, _fireworkScore - 1, _controlSchemeRate, _vrMapScore, _gameExperience);
                            addBasicScoresToStore(_exp3QStore, _tagScore - 1, _controlSchemeRate, _vrMapScore, _gameExperience);
                        }
                    }

                    foreach (Worksheet loopedWorksheet in participantWb.Worksheets)
                    {
                        //the 4th character in the name dictates the experiment type
                        switch (loopedWorksheet.Name[4])
                        {
                            case '1':
                                Console.WriteLine("Processing Relaxation Experiment.......");
                                processRelaxExperiment(loopedWorksheet);
                                break;
                            case '2':
                                Console.WriteLine("Processing Firework Experiment.......");
                                processFireworkExperiment(loopedWorksheet);
                                break;
                            case '3':
                                Console.WriteLine("Processing Tag Experiment.......");
                                processTagExperiment(loopedWorksheet);
                                break;
                        }

                        if (loopedWorksheet.Name.Contains("Sanity"))
                        {
                            //read how many values it took
                            _plongestEEG.score += (int)loopedWorksheet.Cells[2, 2].Value;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error parsing " + participantWb.Name + ": " + e.ToString());
                    participantWb.Close(true);
                    errorParsing = true;
                    break;
                }

                Console.WriteLine("***Calculating Standard Deviations***");

                //now calculate the other variables in the stores for each experiment
                calulcateStoreValues(_exp1Attention, _exp1Meditation, _exp1Store);
                calulcateStoreValues(_exp2Attention, _exp2Meditation, _exp2Store);
                calulcateStoreValues(_exp3Attention, _exp3Meditation, _exp3Store);

                Console.WriteLine("***Calculating Global Running Total and Average***");

                calulcateStoreValues(_exp1Attention, _exp1Meditation, _exp1QStore.feedbackStores[_relaxScore - 1]);
                calulcateStoreValues(_exp2Attention, _exp2Meditation, _exp2QStore.feedbackStores[_fireworkScore - 1]);
                calulcateStoreValues(_exp3Attention, _exp3Meditation, _exp3QStore.feedbackStores[_tagScore - 1]);

                //write their personal results down
                if (_participantWs != null)
                {
                    Console.WriteLine("Recording Summary data for participant");
                    writeLastResultStoreValToWS(_participantWs, 2, 6);
                }

                participantWb.Close(true);

                Console.WriteLine();
                Console.WriteLine("============================================");
                Console.WriteLine("============================================");
                Console.WriteLine("============================================");
                Console.WriteLine();

                if (_plongestEEG.name.Contains("Meier") || _plongestEEG.name.Contains("Lockhart"))
                    continue;

                //calulate high scores
                _lowestMedBase = chooseLowestScore(_plowestMedBase, _lowestMedBase);
                _lowestAttBase = chooseLowestScore(_plowestAttBase, _lowestAttBase);
                _mostFireworks = chooseHighestScore(_pmostFireworks, _mostFireworks);
                _leastFireworks = chooseLowestScore(_pleastFireworks, _leastFireworks);
                _fastestTagVR = chooseLowestScore(_pfastestTagVR, _fastestTagVR);
                _fastestTagKB = chooseLowestScore(_pfastestTagKB, _fastestTagKB);
                _slowestTagKB = chooseHighestScore(_plowestTagKB, _slowestTagKB);
                _longestEEG = chooseHighestScore(_plongestEEG, _longestEEG);
                _mostRelaxed = chooseLowestScore(_pmostRelaxed, _mostRelaxed);
            }

            if (errorParsing == false)
            {
                //now create a new excel document based off the values
                Workbook workBook = _xlApp.Workbooks.Add(1);
                Worksheet mainSheet = workBook.Worksheets.get_Item(1);

                Console.WriteLine("***Writing Experiment results to Summary Sheet***");
                writeResultStoresToWorksheet(mainSheet, 2, 1);

                Console.WriteLine("***Writing Results sorted by rating to Summary Sheet***");
                writeOverallResultSummary(mainSheet, 1, 28);

                Console.WriteLine("***Generating charts for Summary Sheet***");
                generateChartsForSummaryData(mainSheet);

                string resultsFilePath = MAIN_DIR + RESULTS_FOLDER + "\\" + "CombinedResults" + ".xlsx";

                if (File.Exists(resultsFilePath) == true)
                    File.Delete(resultsFilePath);

                workBook.SaveAs(resultsFilePath);
                workBook.Close();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("lowest med base: " + _lowestMedBase);
            Console.WriteLine("lowest att base: " + _lowestAttBase);
            Console.WriteLine("most fireworks: " + _mostFireworks);
            Console.WriteLine("least fireworks: " + _leastFireworks);
            Console.WriteLine("tag VR fast: " + _fastestTagVR);
            Console.WriteLine("tag KB fast: " + _fastestTagKB);
            Console.WriteLine("tag KB slow: " + _slowestTagKB);
            Console.WriteLine("longest eeg: " + _longestEEG);
            Console.WriteLine("most relaxed:  " + _mostRelaxed);

            //while (true) { }
        }

        //write some summary information about the participants
        private void writeOverallResultSummary(Worksheet mainSheet, int x, int y)
        {
            mainSheet.Cells[x, y] = "Gender Breakdown";
            mainSheet.Cells[x + 1, y] = "Male";
            mainSheet.Cells[x + 2, y] = _numMales;
            mainSheet.Cells[x + 1, y + 1] = "Female";
            mainSheet.Cells[x + 2, y + 1] = _numFemales;

            //work out the SDs for each participant's stat
            mainSheet.Cells[x, y + 4] = "Average";
            mainSheet.Cells[x, y + 5] = "Rating SD";

            mainSheet.Cells[x + 1, y + 3] = "Relax Average";
            mainSheet.Cells[x + 2, y + 3] = "Firework Average";
            mainSheet.Cells[x + 3, y + 3] = "Tag Average";
            mainSheet.Cells[x + 4, y + 3] = "Game Experience Average";
            mainSheet.Cells[x + 5, y + 3] = "VR Mapping Average";
            mainSheet.Cells[x + 6, y + 3] = "Control Scheme Average";
            mainSheet.Cells[x + 7, y + 3] = "VR Experience Average";
            
            mainSheet.Cells[x + 1, y + 4] = _relaxScores.getAverage();
            mainSheet.Cells[x + 2, y + 4] = _fireworkScores.getAverage();
            mainSheet.Cells[x + 3, y + 4] = _tagScores.getAverage();
            mainSheet.Cells[x + 4, y + 4] = _gameExperiences.getAverage();
            mainSheet.Cells[x + 5, y + 4] = _vrMapScores.getAverage();
            mainSheet.Cells[x + 6, y + 4] = _controlSchemeRates.getAverage();
            mainSheet.Cells[x + 7, y + 4] = _vrExperiences.getAverage();

            mainSheet.Cells[x + 1, y + 5] = _relaxScores.getSDfromValues();
            mainSheet.Cells[x + 2, y + 5] = _fireworkScores.getSDfromValues();
            mainSheet.Cells[x + 3, y + 5] = _tagScores.getSDfromValues();
            mainSheet.Cells[x + 4, y + 5] = _gameExperiences.getSDfromValues();
            mainSheet.Cells[x + 5, y + 5] = _vrMapScores.getSDfromValues();
            mainSheet.Cells[x + 6, y + 5] = _controlSchemeRates.getSDfromValues();
            mainSheet.Cells[x + 7, y + 5] = _vrExperiences.getSDfromValues();
        }
        UserScore chooseLowestScore(UserScore u1, UserScore u2)
        {
            return (u1.score <= u2.score ? u1 : u2);
        }

        UserScore chooseHighestScore(UserScore u1, UserScore u2)
        {
            return (u1.score > u2.score ? u1 : u2);
        }

        #region Combining the data and experiment stores

        private void addBasicScoresToStore<T>(QuestionaireResultStore<T> store, int index, int csScores, int mapScores, int gameScores) where T : ExpResultStore, new()
        {
            store.controlSchemeScores[index].Add(csScores);
            store.vrMapScores[index].Add(mapScores);
            store.gameExperienceScores[index].Add(gameScores);
        }

        /// <summary>
        /// Writes all of the data in the results store to the given work sheet.
        /// Coordinates should point to the top left of the store template
        /// </summary>
        /// <param name="x">Cell x coordinates, starting at 1</param>
        /// <param name="y">Cell y coordinates, starting at 1</param>
        private void writeResultStoresToWorksheet(Worksheet ws, int x, int y)
        {
            //column labels
            ws.Cells[x, y + 1] = "Experiment 1";
            ws.Cells[x, y + 2] = "Experiment 2";
            ws.Cells[x, y + 3] = "Experiment 3";

            ExpResultStore.writeResultStoreTemplate(ws, x, y);
            Exp2Store.writeResultStoreTemplate(ws, x + 20, y);
            Exp3Store.writeResultStoreTemplate(ws, x + 28, y);

            //write the basic experiment store data down a column
            _exp1Store.writeAverageDownColumn(x, y + 1, ws);
            _exp2Store.writeAverageDownColumn(x, y + 2, ws);
            _exp3Store.writeAverageDownColumn(x, y + 3, ws);

            ws.Cells[x - 1, y + 6] = "Experiment 1";
            ws.Cells[x - 1, y + 13] = "Experiment 2";
            ws.Cells[x - 1, y + 20] = "Experiment 3";
            ExpResultStore.writeResultStoreTemplate(ws, x + 3, y + 6);
            ExpResultStore.writeResultStoreTemplate(ws, x + 3, y + 13);
            ExpResultStore.writeResultStoreTemplate(ws, x + 3, y + 20);
            Exp2Store.writeResultStoreTemplate(ws, x + 23, y + 13);
            Exp3Store.writeResultStoreTemplate(ws, x + 31, y + 20);

            _exp1QStore.writeTemplate(ws, x, y + 6);
            _exp1QStore.writeAverage(ws, x, y + 6);
            _exp2QStore.writeTemplate(ws, x, y + 13);
            _exp2QStore.writeAverage(ws, x, y + 13);
            _exp3QStore.writeTemplate(ws, x, y + 20);
            _exp3QStore.writeAverage(ws, x, y + 20);
        }

        /// <summary>
        /// Writes all of the data in the results store to the given work sheet
        /// </summary>
        /// <param name="x">Cell x coordinates, starting at 1</param>
        /// <param name="y">Cell y coordinates, starting at 1</param>
        private void writeLastResultStoreValToWS(Worksheet ws, int x, int y)
        {
            //column labels
            ws.Cells[x, y + 1] = "Experiment 1";
            ws.Cells[x, y + 2] = "Experiment 2";
            ws.Cells[x, y + 3] = "Experiment 3";

            ExpResultStore.writeResultStoreTemplate(ws, x, y);
            Exp2Store.writeResultStoreTemplate(ws, x + 20, y);
            Exp3Store.writeResultStoreTemplate(ws, x + 28, y);

            //write the basic experiment store data down a column
            _exp1Store.writeLastResultDownColumn(x, y + 1, ws);
            _exp2Store.writeLastResultDownColumn(x, y + 2, ws);
            _exp3Store.writeLastResultDownColumn(x, y + 3, ws);

            ws.Cells[x - 1, y + 6] = "Experiment 1";
            ws.Cells[x - 1, y + 13] = "Experiment 2";
            ws.Cells[x - 1, y + 20] = "Experiment 3";

            ExpResultStore.writeResultStoreTemplate(ws, x + 3, y + 6);
            ExpResultStore.writeResultStoreTemplate(ws, x + 3, y + 13);
            ExpResultStore.writeResultStoreTemplate(ws, x + 3, y + 20);
            Exp2Store.writeResultStoreTemplate(ws, x + 23, y + 13);
            Exp3Store.writeResultStoreTemplate(ws, x + 31, y + 20);

            _exp1QStore.writeTemplate(ws, x, y + 6);
            _exp1QStore.writeLast(ws, x, y + 6);
            _exp2QStore.writeTemplate(ws, x, y + 13);
            _exp2QStore.writeLast(ws, x, y + 13);
            _exp3QStore.writeTemplate(ws, x, y + 20);
            _exp3QStore.writeLast(ws, x, y + 20);
        }

        /// <summary>
        /// Calculates and adds the different variables for the data store for the provided experiment, based off the baseline variables currently assigned
        /// </summary>
        private void calulcateStoreValues(ResultsPair<int> attention, ResultsPair<int> meditation, ExpResultStore store)
        {
            store.deltaNonVRandBaselineAttention.Add(attention.nonVR - _baselineAttention);
            store.deltaNonVRandBaselineMeditation.Add(meditation.nonVR - _baselineMeditation);

            store.deltaVRandBaselineAttention.Add(attention.vr - _baselineAttention);
            store.deltaVRandBaselineMeditation.Add(meditation.vr - _baselineMeditation);

            store.deltaVRnonVRAttention.Add(attention.vr - attention.nonVR);
            store.deltaVRnonVRMeditation.Add(meditation.vr - meditation.nonVR);

            store.nonVRAttention.Add(attention.nonVR);
            store.nonVRMeditation.Add(meditation.nonVR);

            store.vrAttention.Add(attention.vr);
            store.vrMeditation.Add(meditation.vr);

            store.deltaAttentRelativeVRAndNonVR.Add((attention.vr - _baselineAttention) - (attention.nonVR - _baselineAttention));
            store.deltaMeditationRelativeVRAndNonVR.Add((meditation.vr - _baselineMeditation) - (meditation.nonVR - _baselineMeditation));

            store.calculateStandardDeviations();
        }

#endregion

        #region Manipulating data results

        /// <summary>
        /// Omits meditation and attention values that are before the given time
        /// </summary>>
        private void omitEEGResultsBeforeTime(double time, Worksheet ws)
        {
            //attention
            omitEEGDataGroupBeforeTime(time, ws, 2, 1);
            //meditation
            omitEEGDataGroupBeforeTime(time, ws, 2, 4);
        }

        /// <summary>
        /// Omits results from the given data group and recalculates the total and average values.
        /// </summary>
        /// <param name="x">Start row num of the data group</param>
        /// <param name="y">Start column num of the data group</param>
        private static void omitEEGDataGroupBeforeTime(double time, Worksheet ws, int x, int y)
        {
            //attention
            int dataStartRow = x + 2;
            int cutoffRow = dataStartRow;
            int numValues = (int)ws.Cells[x, y + 1].Value;
            int numToOmit = 0;

            //loop until we have found the data that is past to cutoff point or until we have reached the end of the column
            while ((float)ws.Cells[cutoffRow, y].Value < time && cutoffRow < numValues + dataStartRow)
            {
                cutoffRow++;
                numToOmit++;
            }

            int newEEGTotal = 0;

            //now write over the results
            for (int i = dataStartRow + numToOmit; i < dataStartRow + numValues; i++)
            {
                //the time stamp
                ws.Cells[i - numToOmit, y].Value = ws.Cells[i, y].Value;

                int replacementVal = (int)ws.Cells[i, y + 1].Value;
                //the raw value
                ws.Cells[i - numToOmit, y + 1].Value = replacementVal;

                newEEGTotal += replacementVal;
            }

            numValues = numValues - numToOmit;
            int newAverage = newEEGTotal / (numValues);
            //recalculate the totals and average
            ws.Cells[x, y + 1] = numValues;
            ws.Cells[x, y + 2] = newAverage;

            //rewrite the new average down the average column
            writeDataDownColumn(ws, Enumerable.Repeat<int>(newAverage, numValues).ToArray(), dataStartRow, y + 2);
        }

        private void subtractAllIndexesByValue(ref double[] target, double amount)
        {
            for (int i = 0; i < target.Length; i++)
                target[i] -= amount;
        }

        #endregion

        #region Time Methods

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
            return DateTime.ParseExact(cellTimeStr, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
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

                if (eegStartTime == eegEndTime || (tempDate > eegStartTime && tempDate < eegEndTime))
                    //now change the timestamps to now be timestamps from the experiment start time, rather than the unreal engine start time
                    timeStamps.Add((tempDate - eegStartTime).TotalSeconds);
            }

            return timeStamps.ToArray();
        }

        #endregion

        #region Processing Experiments

        private void processBaseline(Worksheet ws)
        {
            //average all the attention values
            _baselineAttention = (int)ws.Cells[2, 3].Value;
            _baselineMeditation = (int)ws.Cells[2, 6].Value;

            _plowestAttBase.score = _baselineAttention;
            _plowestMedBase.score = _baselineMeditation;
            
            Chart generatedChart = generateChartForMeditationAndAttention(ws);
        }

        private void processRelaxExperiment(Worksheet ws)
        {
            generateChartForMeditationAndAttention(ws);
            recordAverageValues(ws, ref _exp1Attention, ref _exp1Meditation);

            _pmostRelaxed.score = Math.Min(_pmostRelaxed.score, (int)ws.Cells[2, 6].Value);
        }


        private void processTagExperiment(Worksheet ws)
        {
            string[] tagFiles = Directory.GetFiles(MAIN_DIR + TAG_FOLDER);

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
                File.ReadAllText(MAIN_DIR + TAG_FOLDER + "\\" + filePrefix + ".txt"));

            double lowestStamp = tagStamps[0];
            //data will start from the the first of this time stamp. Previous EEG values will be ommited later on
            subtractAllIndexesByValue(ref tagStamps, lowestStamp);

            //add the stamps to the sheet
            ws.Cells[3, 10] = "Tag times";
            ws.Cells[3, 11] = tagStamps.Length;
            writeDataDownColumn(ws, tagStamps, 4, 10);
            writeDataDownColumn(ws, Enumerable.Repeat<int>(50, tagStamps.Length).ToArray(), 4, 11);

            omitEEGResultsBeforeTime(lowestStamp, ws);

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

            recordAverageValues(ws, ref _exp3Attention, ref _exp3Meditation);

            if (ws.Name.Contains("KB") == true)
            {
                _exp3Store.nonVRTimesTaken.Add(tagStamps[tagStamps.Length - 1]);
                _exp3QStore.feedbackStores[_tagScore - 1].nonVRTimesTaken.Add(tagStamps[tagStamps.Length - 1]);

                _pfastestTagKB.score = (int)tagStamps[tagStamps.Length - 1];
                _plowestTagKB.score = (int)tagStamps[tagStamps.Length - 1];
            }
            else if (ws.Name.Contains("VR") == true)
            {
                _exp3Store.vrTimesTaken.Add(tagStamps[tagStamps.Length - 1]);
                _exp3QStore.feedbackStores[_tagScore - 1].vrTimesTaken.Add(tagStamps[tagStamps.Length - 1]);

                _pfastestTagVR.score = (int)tagStamps[tagStamps.Length - 1];
            }
        }

        private void processFireworkExperiment(Worksheet ws)
        {
            string[] fireworkFiles = Directory.GetFiles(MAIN_DIR + FIREWORK_FOLDER);

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
                File.ReadAllText(MAIN_DIR + FIREWORK_FOLDER + "\\" + filePrefix + " Explode.txt"));
            double[] spawnStamps = getNormalisedTimeStamps(eegStartTime, eegEndTime,
                File.ReadAllText(MAIN_DIR + FIREWORK_FOLDER + "\\" + filePrefix + " Spawn.txt"));

            double lowestStamp = spawnStamps[0];
            //data will start from the the first of this time stamp. Previous EEG values will be ommited later on
            subtractAllIndexesByValue(ref spawnStamps, lowestStamp);
            subtractAllIndexesByValue(ref explosionStamps, explosionStamps[0]);

            //add the stamps to the sheet
            ws.Cells[3, 10] = "Spawn times";
            ws.Cells[3, 11] = spawnStamps.Length;
            writeDataDownColumn(ws, spawnStamps, 4, 10);
            writeDataDownColumn(ws, Enumerable.Repeat<int>(40, spawnStamps.Length).ToArray(), 4, 11);

            ws.Cells[3, 13] = "Explosion times";
            ws.Cells[3, 14] = explosionStamps.Length;
            writeDataDownColumn(ws, explosionStamps, 4, 13);
            writeDataDownColumn(ws, Enumerable.Repeat<int>(60, explosionStamps.Length).ToArray(), 4, 14);

            omitEEGResultsBeforeTime(lowestStamp, ws);

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

            recordAverageValues(ws, ref _exp2Attention, ref _exp2Meditation);

            if (ws.Name.Contains("KB") == true)
            {
                _exp2Store.nonVRFireworksSpawned.Add(spawnStamps.Length);
                _exp2Store.nonVRTotalInteractionTimes.Add(explosionStamps[explosionStamps.Length - 1]);

                _exp2QStore.feedbackStores[_fireworkScore - 1].nonVRFireworksSpawned.Add(spawnStamps.Length);
                _exp2QStore.feedbackStores[_fireworkScore - 1].nonVRTotalInteractionTimes.Add(explosionStamps[explosionStamps.Length - 1]);
            }
            else if (ws.Name.Contains("VR") == true)
            {
                _exp2Store.vrFireworksSpawned.Add(spawnStamps.Length);
                _exp2Store.vrTotalInteractionTimes.Add(explosionStamps[explosionStamps.Length - 1]);

                _exp2QStore.feedbackStores[_fireworkScore - 1].vrFireworksSpawned.Add(spawnStamps.Length);
                _exp2QStore.feedbackStores[_fireworkScore - 1].vrTotalInteractionTimes.Add(explosionStamps[explosionStamps.Length - 1]);
            }

            _pmostFireworks.score += spawnStamps.Length;
            _pleastFireworks.score += spawnStamps.Length;
        }

#endregion

        private void recordAverageValues(Worksheet ws, ref ResultsPair<int> attentionPair, ref ResultsPair<int> meditationPair)
        {
            //read the average attention and meditation
            int averageAttention = (int)ws.Cells[2, 3].Value;
            int averageMeditation = (int)ws.Cells[2, 6].Value;

            if (ws.Name.Contains("KB") == true)
            {
                attentionPair.nonVR = averageAttention;
                meditationPair.nonVR = averageMeditation;

            }
            else if (ws.Name.Contains("VR") == true)
            {
                attentionPair.vr = averageAttention;
                meditationPair.vr = averageMeditation;
            }
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

        #region Graph Generation

        private void generateChartsForSummaryData(Worksheet ws)
        {
            //create a graph from the data range
            ChartObjects xlCharts = ws.ChartObjects(Type.Missing);

            #region Summary Data

            //Summary VR and nonVR relative meditation
            {
                ChartObject chartObject = xlCharts.Add(5, 600, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Change in Meditation";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.XValues = ws.get_Range("B2", "D2");
                series1.Values = ws.get_Range("B11", "D11");
                series1.Name = "VR Meditation - Baseline";
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("B17", "D17"), ws.get_Range("B17", "D17"));

                Series series2 = sc.NewSeries();
                series2.XValues = ws.get_Range("B2", "D2");
                series2.Values = ws.get_Range("B12", "D12");
                series2.Name = "Non-VR Meditation - Baseline";
                series2.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("B18", "D18"), ws.get_Range("B18", "D18"));
            }

            //Summary VR and nonVR relative attention
            {
                ChartObject chartObject = xlCharts.Add(5, 850, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Change in Attention";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.XValues = ws.get_Range("B2", "D2");
                series1.Values = ws.get_Range("B8", "D8");
                series1.Name = "VR Attention - Baseline";
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("B15", "D15"), ws.get_Range("B15", "D15"));

                Series series2 = sc.NewSeries();
                series2.XValues = ws.get_Range("B2", "D2");
                series2.Values = ws.get_Range("B9", "D9");
                series2.Name = "Non-VR Attention - Baseline";
                series2.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("B16", "D16"), ws.get_Range("B16", "D16"));
            }

            //relative meditation comparison
            {
                ChartObject chartObject = xlCharts.Add(5, 1050, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Comparison of change in meditation, VR - non-VR";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.Name = "Relative VR - Relative non-VR";
                series1.XValues = ws.get_Range("B2", "D2");
                series1.Values = ws.get_Range("B14", "D14");
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("B20", "D20"), ws.get_Range("B20", "D20"));
            }

            //relative attention comparison
            {
                ChartObject chartObject = xlCharts.Add(5, 1300, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Comparison of change in attention, VR - non-VR";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.Name = "Relative VR - Relative non-VR";
                series1.XValues = ws.get_Range("B2", "D2");
                series1.Values = ws.get_Range("B13", "D13");
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("B19", "D19"), ws.get_Range("B19", "D19"));
            }

            //firework summary
            {
                ChartObject chartObject = xlCharts.Add(5, 1550, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Fireworks Spawned";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.XValues = ws.get_Range("C2", "C2");
                series1.Values = ws.get_Range("C23", "C23");
                series1.Name = "VR";
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("C26", "C26"), ws.get_Range("C26", "C26"));

                Series series2 = sc.NewSeries();
                series2.XValues = ws.get_Range("C2", "C2");
                series2.Values = ws.get_Range("C22", "C22");
                series2.Name = "Non-VR";
                series2.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("C27", "C27"), ws.get_Range("C27", "C27"));
            }

            //tag summary
            {
                ChartObject chartObject = xlCharts.Add(5, 1800, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Time Taken";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.XValues = ws.get_Range("D2", "D2");
                series1.Values = ws.get_Range("D31", "D31");
                series1.Name = "VR";
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("D32", "D32"), ws.get_Range("D32", "D32"));

                Series series2 = sc.NewSeries();
                series2.XValues = ws.get_Range("D2", "D2");
                series2.Values = ws.get_Range("D30", "D30");
                series2.Name = "Non-VR";
                series2.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("D33", "D33"), ws.get_Range("D33", "D33"));
            }

            #endregion

            #region Experiment 1

            //experiment 1 relative meditation comparison
            {
                ChartObject chartObject = xlCharts.Add(400, 600, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Comparison of change in meditation, VR - non-VR";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.Name = "Relative VR - Relative non-VR";
                series1.XValues = ws.get_Range("H2", "L2");
                series1.Values = ws.get_Range("H17", "L17");
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("H23", "L23"), ws.get_Range("H23", "L23"));
            }

            //experiment 1 VR and nonVR relative meditation
            {
                ChartObject chartObject = xlCharts.Add(400, 850, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Change in Meditation";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.XValues = ws.get_Range("H2", "L2");
                series1.Values = ws.get_Range("H14", "L14");
                series1.Name = "VR Meditation - Baseline";
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("H20", "L20"), ws.get_Range("H20", "L20"));

                Series series2 = sc.NewSeries();
                series2.XValues = ws.get_Range("H2", "L2");
                series2.Values = ws.get_Range("H15", "L15");
                series2.Name = "Non-VR Meditation - Baseline";
                series2.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("H21", "L21"), ws.get_Range("H21", "L21"));
            }

            //experiment 1 relative attetion comparison
            {
                ChartObject chartObject = xlCharts.Add(400, 1050, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Comparison of change in attention, VR - non-VR";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.Name = "Relative VR - Relative non-VR";
                series1.XValues = ws.get_Range("H2", "L2");
                series1.Values = ws.get_Range("H17", "L17");
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("H22", "L22"), ws.get_Range("H22", "L22"));
            }

            //experiment 1 VR and nonVR attention relative
            {
                ChartObject chartObject = xlCharts.Add(400, 1300, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Change in Attention";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.XValues = ws.get_Range("H2", "L2");
                series1.Values = ws.get_Range("H11", "L11");
                series1.Name = "VR Attention - Baseline";
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("H18", "L18"), ws.get_Range("H18", "L18"));

                Series series2 = sc.NewSeries();
                series2.XValues = ws.get_Range("H2", "L2");
                series2.Values = ws.get_Range("H12", "L12");
                series2.Name = "Non-VR Attention - Baseline";
                series2.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("H19", "L19"), ws.get_Range("H19", "L19"));
            }

            #endregion

            #region Experiment 2

            //experiment 2 relative attetion comparison
            {
                ChartObject chartObject = xlCharts.Add(800, 600, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Comparison of change in attention, VR - non-VR";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.Name = "Relative VR - Relative non-VR";
                series1.XValues = ws.get_Range("O2", "S2");
                series1.Values = ws.get_Range("O17", "S17");
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("O22", "S22"), ws.get_Range("O22", "S22"));
            }

            //experiment 2 VR and nonVR attention relative
            {
                ChartObject chartObject = xlCharts.Add(800, 850, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Change in Attention";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.XValues = ws.get_Range("O2", "S2");
                series1.Values = ws.get_Range("O11", "S11");
                series1.Name = "VR Attention - Baseline";
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("O18", "S18"), ws.get_Range("O18", "S18"));

                Series series2 = sc.NewSeries();
                series2.XValues = ws.get_Range("O2", "S2");
                series2.Values = ws.get_Range("O12", "S12");
                series2.Name = "Non-VR Attention - Baseline";
                series2.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("O19", "S19"), ws.get_Range("O19", "S19"));
            }

            //experiment 2 fireworks spawned
            {
                ChartObject chartObject = xlCharts.Add(800, 1050, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Fireworks Spawned";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.XValues = ws.get_Range("O2", "S2");
                series1.Values = ws.get_Range("O26", "S26");
                series1.Name = "VR";
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("O29", "S29"), ws.get_Range("O29", "S29"));

                Series series2 = sc.NewSeries();
                series2.XValues = ws.get_Range("O2", "S2");
                series2.Values = ws.get_Range("O25", "S25");
                series2.Name = "Non-VR";
                series2.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("O30", "S30"), ws.get_Range("O30", "S30"));
            }

            //experiment 2 firework duration
            {
                ChartObject chartObject = xlCharts.Add(800, 1300, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Firework Experiment Duration";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.XValues = ws.get_Range("O2", "S2");
                series1.Values = ws.get_Range("O28", "S28");
                series1.Name = "VR";
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("O31", "S31"), ws.get_Range("O31", "S31"));

                Series series2 = sc.NewSeries();
                series2.XValues = ws.get_Range("O2", "S2");
                series2.Values = ws.get_Range("O27", "S27");
                series2.Name = "Non-VR";
                series2.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("O32", "S32"), ws.get_Range("O32", "S32"));
            }

            #endregion

            #region Experiment 3

            //experiment 3 relative attetion comparison
            {
                ChartObject chartObject = xlCharts.Add(1200, 600, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Comparison of change in attention, VR - non-VR";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.Name = "Relative VR - Relative non-VR";
                series1.XValues = ws.get_Range("V2", "Z2");
                series1.Values = ws.get_Range("V17", "Z17");
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("V22", "Z22"), ws.get_Range("V22", "Z22"));
            }

            //experiment 3 VR and nonVR attention relative
            {
                ChartObject chartObject = xlCharts.Add(1200, 850, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Change in Attention";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.XValues = ws.get_Range("V2", "Z2");
                series1.Values = ws.get_Range("V11", "Z11");
                series1.Name = "VR Attention - Baseline";
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("V18", "Z18"), ws.get_Range("V18", "Z18"));

                Series series2 = sc.NewSeries();
                series2.XValues = ws.get_Range("V2", "Z2");
                series2.Values = ws.get_Range("V12", "Z12");
                series2.Name = "Non-VR Attention - Baseline";
                series2.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("V19", "Z19"), ws.get_Range("V19", "Z19"));
            }

            //experiment 3 duration
            {
                ChartObject chartObject = xlCharts.Add(1200, 1050, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlColumnClustered;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Time Taken";

                SeriesCollection sc = chartPage.SeriesCollection();

                Series series1 = sc.NewSeries();
                series1.XValues = ws.get_Range("V2", "Z2");
                series1.Values = ws.get_Range("V34", "Z34");
                series1.Name = "VR";
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("V35", "Z35"), ws.get_Range("V35", "Z35"));

                Series series2 = sc.NewSeries();
                series2.XValues = ws.get_Range("V2", "Z2");
                series2.Values = ws.get_Range("V33", "Z33");
                series2.Name = "Non-VR";
                series2.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("V36", "Z36"), ws.get_Range("V36", "Z36"));
            }

            #endregion

            #region Participants and Rating

            //Gender breakdown
            {
                ChartObject chartObject = xlCharts.Add(1600, 600, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlPie;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Gender Breakdown";
                chartPage.HasLegend = true;

                SeriesCollection sc = chartPage.SeriesCollection();
                Series series1 = sc.NewSeries();
                series1.Values = ws.get_Range("AB3", "AC3");
                series1.XValues = ws.get_Range("AB2", "AC2");
                series1.Name = "Sex";

            }
            //average scores
            {
                ChartObject chartObject = xlCharts.Add(2000, 600, 375, 220);
                Chart chartPage = chartObject.Chart;
                chartPage.ChartType = XlChartType.xlBarStacked;
                chartPage.HasTitle = true;
                chartPage.ChartTitle.Text = "Post-experiment survey ratings";
                chartPage.HasLegend = true;

                SeriesCollection sc = chartPage.SeriesCollection();
                Series series1 = sc.NewSeries();
                series1.Values = ws.get_Range("AF2", "AF8");
                series1.XValues = ws.get_Range("AE2", "AE8");
                series1.Name = "Preference";
                series1.ErrorBar(XlErrorBarDirection.xlY, XlErrorBarInclude.xlErrorBarIncludeBoth, XlErrorBarType.xlErrorBarTypeCustom, ws.get_Range("AG2", "AG8"), ws.get_Range("AG2", "AG8"));
            }

            #endregion

        }

        private Series createSeriesForSingleData(SeriesCollection sc, Worksheet ws, string valueCoord, string xValueCoord, string name)
        {
            Series series1 = sc.NewSeries();
            series1.Values = ws.get_Range(valueCoord, valueCoord);
            series1.XValues = ws.get_Range(xValueCoord, xValueCoord);
            series1.Name = name;

            return series1;
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

            createSeriesForColumnDataWithBaseline(ws, sc, 2, 1, "Attention", _baselineAttention);
            createSeriesForColumnDataWithBaseline(ws, sc, 2, 4, "Meditation", _baselineMeditation);

            return chartPage;
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
        private void createSeriesForColumnDataWithBaseline(Worksheet ws, SeriesCollection sc, int templateStartRow, int templateStartColumn, string seriesName, int average)
        {
            Series series1 = sc.NewSeries();

            int dataStartRow = templateStartRow + 2;

            //the raw values
            int numValues = (int)ws.Cells[templateStartRow, templateStartColumn + 1].Value;
            //-1 because if there is only 1 data value for example, the end cell should equal the start cell 
            string endCell = (dataStartRow + numValues - 1).ToString();

            string rawDataColumn = getColNameFromIndex(templateStartColumn + 1);
            string rawDataTimeColumn = getColNameFromIndex(templateStartColumn);

            //create series from the raw data
            series1.Values = ws.get_Range(rawDataColumn + dataStartRow.ToString(), rawDataColumn + endCell);
            series1.XValues = ws.get_Range(rawDataTimeColumn + dataStartRow.ToString(), rawDataTimeColumn + endCell);
            series1.Name = seriesName;

            //repeat the baseline data beside it
            writeDataDownColumn(ws, Enumerable.Repeat<int>(average, numValues).ToArray(), dataStartRow, templateStartColumn + 2);

            //create a new series for average values
            Series baselineASeries = sc.NewSeries();
            endCell = (dataStartRow + numValues).ToString();

            string rawDataAverageColumn = getColNameFromIndex(templateStartColumn + 2);

            //create a series from the repeated average value
            baselineASeries.Values = ws.get_Range(rawDataAverageColumn + dataStartRow.ToString(), rawDataAverageColumn + endCell);
            baselineASeries.XValues = ws.get_Range(rawDataTimeColumn + dataStartRow.ToString(), rawDataTimeColumn + endCell);
            baselineASeries.Name = "Baseline " + seriesName;
            baselineASeries.ChartType = XlChartType.xlLine;
        }

        #endregion


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


        private void setupEnvironment()
        {
            _xlApp = new Application();
            if (_xlApp == null)
                Console.WriteLine("Excel is not properly installed, cannot save experiments");

            if (Directory.Exists(MAIN_DIR + RESULTS_FOLDER) == false)
                Directory.CreateDirectory(MAIN_DIR + RESULTS_FOLDER);

            _exp1Store = new ExpResultStore();
            _exp2Store = new Exp2Store();
            _exp3Store = new Exp3Store();

            _exp1QStore = new QuestionaireResultStore<ExpResultStore>(5);
            _exp2QStore = new QuestionaireResultStore<Exp2Store>(5);
            _exp3QStore = new QuestionaireResultStore<Exp3Store>(5);

            _lowestMedBase = new UserScore("none");
            _lowestMedBase.score = 100;
            _lowestAttBase = new UserScore("none");
            _lowestAttBase.score = 100;
            _mostFireworks = new UserScore("none");
            _leastFireworks = new UserScore("none");
            _leastFireworks.score = 100;
            _fastestTagVR = new UserScore("none");
            _fastestTagVR.score = 100;
            _fastestTagKB = new UserScore("none");
            _fastestTagKB.score = 100;
            _slowestTagKB = new UserScore("none");
            _mostRelaxed = new UserScore("none");
            _mostRelaxed.score = 100;
            _longestEEG = new UserScore("none");

            _relaxScores = new List<int>();
            _fireworkScores = new List<int>();
            _tagScores = new List<int>();
            _gameExperiences = new List<int>();
            _vrMapScores = new List<int>();
            _controlSchemeRates = new List<int>();
            _vrExperiences = new List<int>();
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
