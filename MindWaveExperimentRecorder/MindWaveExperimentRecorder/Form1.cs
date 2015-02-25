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
        public Form1()
        {
            InitializeComponent();
        }

        #region IExperimentorView methods

        public void updateParticipantLabel(Participant user)
        {

        }

        public void updateExperimentLabel(string name)
        {

        }

        public void addLogMessage(string message)
        {

        }
        public void clearGraph()
        {

        }

        public void plotGraphPoint(DataPoint newPoint, string id)
        {

        }

        #endregion
    }
}
