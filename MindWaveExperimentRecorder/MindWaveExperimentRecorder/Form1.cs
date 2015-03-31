using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MindWaveExperimentRecorder.CSCExperimentor;

namespace MindWaveExperimentRecorder
{
    public partial class Form1 : Form , IExperimentorView
    {
    
        bool _firstGraphPointPlotted = false;
        DateTime _firstGraphPointTime;

        CSCExperimentManager _manager;

        public Form1()
        {
            InitializeComponent();
            _manager = new CSCExperimentManager(this);

            this.experienceComboBox.DataSource = Enum.GetValues(typeof(Participant.ExperienceLevels));
            this.outputDirTextBox.Text = _manager.getOutputDir();
        }

        #region IExperimentorView methods

        public void updateParticipantLabel(Participant user)
        {
            this.participantLabel.Text = user.Name;
        }

        public void updateExperimentLabel(string name)
        {
            this.experimentLabel.Text = name;
        }

        public void updateIsRecordingUI(bool state)
        {
            this.recordingButton.Text = state ? "Set Recording Off" : "Set Recording On";
        }

        public void addLogMessage(string message)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.logTextBox.AppendText(message + System.Environment.NewLine);
            });
        }
        public void clearGraph()
        {
            _firstGraphPointPlotted = false;

            foreach (System.Windows.Forms.DataVisualization.Charting.Series loopedSeries in this.eegChart.Series) {
            
                loopedSeries.Points.Clear();
            }
        }

        public void plotGraphPoint(DataPoint newPoint, string id)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (_firstGraphPointPlotted == false)
                {

                    _firstGraphPointTime = newPoint.TimeStamp;
                    _firstGraphPointPlotted = true;
                }
                switch (id)
                {
                    case "Attention":
                    case "Meditation":
                    case "BlinkStrength":
                        TypedDataPoint<double> parsedPoint = newPoint as TypedDataPoint<double>;

                        this.eegChart.Series[id].Points.AddXY((parsedPoint.TimeStamp - _firstGraphPointTime).TotalSeconds, parsedPoint.Value);
                        break;
                }
            });
        }

        #endregion

        private void exp1Button_Click(object sender, EventArgs e)
        {
            _manager.startNewExperiment("1 " + (exp1Checkbox.Checked ? "KB" : "VR"), exp1Checkbox.Checked);
        }

        private void exp2Button_Click(object sender, EventArgs e)
        {
            _manager.startNewExperiment("2 " + (exp2Checkbox.Checked ? "KB" : "VR"), exp2Checkbox.Checked);
        }

        private void exp3Button_Click(object sender, EventArgs e)
        {
            _manager.startNewExperiment("3 " + (exp3Checkbox.Checked ? "KB" : "VR"), exp3Checkbox.Checked);
        }

        private void newParticipantButton_Click(object sender, EventArgs e)
        {
            if (this.participantField.Text != "")
            {
                Participant.ExperienceLevels xpLevel;
                Enum.TryParse<Participant.ExperienceLevels>(this.experienceComboBox.SelectedValue.ToString(), out xpLevel);

                _manager.setNewParticipant(this.participantField.Text, xpLevel);
                this.participantField.Text = "";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _manager.shutdown();

            if (System.Windows.Forms.Application.MessageLoop)
                // Use this since we are a WinForms app
                System.Windows.Forms.Application.Exit();
            else
                // Use this since we are a console app
                System.Environment.Exit(1);
        }

        private void recordingButton_Click(object sender, EventArgs e)
        {
            _manager.setRecordData(!_manager.isRecordingData());
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            _manager.saveExperiments();
        }

        private void outputDirTextBox_TextChanged(object sender, EventArgs e)
        {
            this._manager.setOutputDirectory(this.outputDirTextBox.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _manager.startNewExperiment("Baseline", false);
        }

    }
}
