using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Interop.Excel;
using NeuroSky.ThinkGear;
using NeuroSky.ThinkGear.Algorithms;

namespace MindWaveExperimentRecorder.CSCExperimentor
{
    class MindwaveExperiment
    {
        List<TypedDataPoint<double>> _attentionReadings;
        List<TypedDataPoint<double>> _meditationReadings;
        List<TypedDataPoint<double>> _blinkRecordings;

        int _id;

        public MindwaveExperiment(int id)
        {
            _id = id;
            _attentionReadings = new List<TypedDataPoint<double>>();
            _meditationReadings = new List<TypedDataPoint<double>>();
            _blinkRecordings = new List<TypedDataPoint<double>>();
        }

        /// <summary>
        /// Records a new datapoint for attention
        /// </summary>
        public void addAttention(TypedDataPoint<double> input)
        {
            _attentionReadings.Add(input);
        }

        /// <summary>
        /// Records a new datapoint for meditation
        /// </summary>
        public void addMeditation(TypedDataPoint<double> input)
        {
            _meditationReadings.Add(input);
        }

        public void addBlinkHit(TypedDataPoint<double> input)
        {
            _blinkRecordings.Add(input);
        }

        /// <summary>
        /// Populates the worksheet with the data being held in this experiment
        /// </summary>
        /// <param name="ws"></param>
        public virtual void populateWorksheet(Worksheet ws)
        {
            ws.Name = "Exp " + _id;
            ws.Cells[1, 1] = "Experiment " + _id;

            //find the lowest date time
            DateTime lowestTime = getLowestDateTime();

            addDataListToWorksheet(_attentionReadings, "Attention", 2, 1, ws, lowestTime);
            addDataListToWorksheet(_meditationReadings, "Meditation", 2, 4, ws, lowestTime);
            addDataListToWorksheet(_blinkRecordings, "Blink Hits", 2, 7, ws, lowestTime);
        }

        /// <summary>
        /// Finds the lowest datetime amoungst all of the starting records
        /// </summary>
        /// <returns>The lowest date time across the records</returns>
        DateTime getLowestDateTime()
        {
            DateTime runningLowest = default(DateTime);
            if (_attentionReadings.Count > 0)
                runningLowest = _attentionReadings[0].TimeStamp;

            if (_meditationReadings.Count > 0 && (default(DateTime) == runningLowest || _meditationReadings[0].TimeStamp < runningLowest))
                runningLowest = _meditationReadings[0].TimeStamp;

            if (_blinkRecordings.Count > 0 && (default(DateTime) == runningLowest || _blinkRecordings[0].TimeStamp < runningLowest))
                runningLowest = _blinkRecordings[0].TimeStamp;

            return runningLowest;
        }

        /// <summary>
        /// Adds the data list to the parameter worksheet, listing the values in columns
        /// Format is Title /n TimeFromStartOfExperiment /t Value
        /// </summary>
        /// <param name="input">Input to add</param>
        /// <param name="title">Title for the input</param>
        /// <param name="startRow">What row to start writing from</param>
        /// <param name="startColumn">What column to start writing from</param>
        /// <param name="ws">Where the data is going</param>
        /// <param name="startTime">What all the values should be based off</param>
        protected void addDataListToWorksheet(List<TypedDataPoint<double>> input, string title, int startRow, int startColumn, Worksheet ws, DateTime startTime)
        {
            ws.Cells[startRow, startColumn] = title;
            ws.Cells[startRow + 1, startColumn] = "Time Stamp";
            ws.Cells[startRow + 1, startColumn + 1] = "Value";

            for (int i = 0; i < input.Count; i++)
            {
                ws.Cells[startRow + 2 + i, startColumn] = (input[i].TimeStamp - startTime).TotalSeconds;
                ws.Cells[startRow + 2 + i, startColumn + 1] = input[i].Value;
            }
        }
    }

    class CSCExperimentManager : IExperimentManager
    {

#region Variables

        List<MindwaveExperiment> _experiments;
        MindwaveExperiment _activeExperiment;

        Participant _curParticipant;

        //flag for whether eeg sensor data should be recorded
        bool _shouldRecordData = false;
        Connector _eegSensor;
        string _outputDir = @"C:\CSC4001Results";

        IExperimentorView _view;

        Microsoft.Office.Interop.Excel.Application _xlApp;
#endregion

        public CSCExperimentManager(IExperimentorView view)
        {
            _experiments = new List<MindwaveExperiment>();
            _view = view;

            _xlApp = new Application();
            if (_xlApp == null)
            {
                _view.addLogMessage("Excel is not properly installed, cannot save experiments");
            }

            _eegSensor = new Connector();
            _eegSensor.DeviceConnected += new EventHandler(OnDeviceConnected);
            _eegSensor.DeviceConnectFail += new EventHandler(OnDeviceFail);
            _eegSensor.DeviceValidating += new EventHandler(OnDeviceValidating);
            _eegSensor.ConnectScan("COM40");
            _eegSensor.setBlinkDetectionEnabled(true);

            reset();
        }

        ~CSCExperimentManager()
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

        /// <summary>
        /// Saves all the experiments in the experiments list, to an autogenerated file based off the participant
        /// </summary>
        public void saveExperiments()
        {
            string outputFile = _outputDir + @"\" + _curParticipant.Name;
            exportAllExperiments(outputFile);
        }

        /// <summary>
        /// Resets the experiments and participant
        /// </summary>
        void reset()
        {
            _activeExperiment = null;
            _experiments.Clear();
            _shouldRecordData = false;
            _curParticipant = new Participant("Unnamed", Participant.ExperienceLevels.None);

            setRecordData(false);

            _view.clearGraph();
            _view.updateParticipantLabel(_curParticipant);
            _view.updateExperimentLabel("No Experiment Running");
        }

        #region Mindwave Methods

        void OnDeviceConnected(object sender, EventArgs e)
        {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;

            _view.addLogMessage("Device found on: " + de.Device.PortName);
            de.Device.DataReceived += new EventHandler(OnDataReceived);
        }

        void OnDeviceFail(object sender, EventArgs e)
        {
            _view.addLogMessage("No devices found!");
        }

        void OnDeviceValidating(object sender, EventArgs e)
        {
           _view.addLogMessage("Validating: ");
        }

        // Called when data is received from a device

        void OnDataReceived(object sender, EventArgs e)
        {
            if (_shouldRecordData == true)
            {
                Device.DataEventArgs de = (Device.DataEventArgs)e;
                DataRow[] tempDataRowArray = de.DataRowArray;

                TGParser tgParser = new TGParser();
                tgParser.Read(de.DataRowArray);

                for (int i = 0; i < tgParser.ParsedData.Length; i++)
                {
                    if (tgParser.ParsedData[i].ContainsKey("Attention"))
                        recordPoint(new TypedDataPoint<double>(tgParser.ParsedData[i]["Attention"], DateTime.Now), "Attention");

                    if (tgParser.ParsedData[i].ContainsKey("Meditation"))
                        recordPoint(new TypedDataPoint<double>(tgParser.ParsedData[i]["Meditation"], DateTime.Now), "Meditation");

                    if (tgParser.ParsedData[i].ContainsKey("BlinkStrength"))
                        recordPoint(new TypedDataPoint<double>(tgParser.ParsedData[i]["BlinkStrength"], DateTime.Now), "BlinkStrength");
                }
            }
        }

        #endregion

        #region IExperimentManager methods

        public void setNewParticipant(string name, Participant.ExperienceLevels level, bool autoSaveExperiments = true)
        {
            if (autoSaveExperiments == true)
                saveExperiments();

            reset();

            _curParticipant = new Participant(name, level);

            _view.updateParticipantLabel(_curParticipant);
        }

        public void startNewExperiment(int id)
        {
            MindwaveExperiment newExp = new MindwaveExperiment(id);
            _activeExperiment = newExp;
            _experiments.Add(_activeExperiment);

            _view.updateExperimentLabel("Experiment " + id);
            _view.clearGraph();
            setRecordData(false);
        }

        public void recordPoint(DataPoint point, string id)
        {
            if (_activeExperiment == null)
                _view.addLogMessage("Cannot record point " + id + ": no active experiment running");
            else if (_shouldRecordData == true)
            {
                switch (id)
                {
                    case "Attention":
                        _activeExperiment.addAttention(point as TypedDataPoint<double>);
                        _view.plotGraphPoint(point, id);
                        break;
                    case "Meditation":
                        _activeExperiment.addMeditation(point as TypedDataPoint<double>);
                        _view.plotGraphPoint(point, id);
                        break;
                    case "BlinkStrength":
                        _activeExperiment.addBlinkHit(point as TypedDataPoint<double>);
                        _view.plotGraphPoint(point, id);
                        break;
                }
            }
        }

        public void exportAllExperiments(String filePath)
        {
            if (System.IO.Directory.Exists(_outputDir) == false)
                System.IO.Directory.CreateDirectory(_outputDir);

            if (_experiments.Count != 0)
            {
                //1st sheet should be about the participant
                Workbook workBook = _xlApp.Workbooks.Add(1);

                Worksheet mainSheet = workBook.Worksheets.get_Item(1);
                mainSheet.Name = "Participant";
                mainSheet.Cells[1, 1] = _curParticipant.Name;
                mainSheet.Cells[1, 2] = Enum.GetName(typeof(Participant.ExperienceLevels), _curParticipant.ExperienceLevel);

                //future sheets are about each experiment
                foreach (MindwaveExperiment loopedExperiment in _experiments)
                {
                    Worksheet loopedSheet = workBook.Worksheets.Add();
                    loopedExperiment.populateWorksheet(loopedSheet);
                }

                workBook.SaveAs(filePath + ".xlsx");
                workBook.Close();
                _view.addLogMessage("Saved " + filePath);
            }
        }

        public void shutdown()
        {
            _eegSensor.DeviceConnected -= new EventHandler(OnDeviceConnected);
            _eegSensor.DeviceConnectFail -= new EventHandler(OnDeviceFail);
            _eegSensor.DeviceValidating -= new EventHandler(OnDeviceValidating);
            _eegSensor.StopScan();
            _eegSensor.Disconnect();
            _eegSensor.Close();
            _xlApp.Quit();
        }

        #endregion

        #region Getters and Setters

        public void setRecordData(bool state)
        {
            _shouldRecordData = state;
            _view.updateIsRecordingUI(state);
        }

        /// <summary>
        /// Returns whether the system is recording data as it gets it
        /// </summary>
        /// <returns>True if recording data, false if not</returns>
        public bool isRecordingData()
        {
            return _shouldRecordData;
        }

        public void setOutputDirectory(string dir)
        {
            _outputDir = dir;
        }

        public string getOutputDir()
        {
            return _outputDir;
        }


        #endregion
    }
}
