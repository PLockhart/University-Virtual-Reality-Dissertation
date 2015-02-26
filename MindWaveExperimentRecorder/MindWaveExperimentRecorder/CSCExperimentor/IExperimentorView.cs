using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWaveExperimentRecorder.CSCExperimentor
{

    interface IExperimentorView
    {
        /// <summary>
        /// Updates the UI to reflect the participant
        /// </summary>
        /// <param name="user"></param>
        void updateParticipantLabel(Participant user);

        /// <summary>
        /// Updates the UI for the current experiment
        /// </summary>
        /// <param name="name"></param>
        void updateExperimentLabel(string name);

        /// <summary>
        /// Updates the UI according to whether data is being recorded or not
        /// </summary>
        /// <param name="state">If true, data is being recorded</param>
        void updateIsRecordingUI(bool state);

        /// <summary>
        /// Adds a message to the streaming log
        /// </summary>
        /// <param name="message"></param>
        void addLogMessage(string message);

        /// <summary>
        /// Clears the UI graph
        /// </summary>
        void clearGraph();

        /// <summary>
        /// Adds a new point to the graph
        /// </summary>
        /// <param name="newPoint">new point to be plotted</param>
        /// <param name="id">id for what this data point is</param>
        void plotGraphPoint(DataPoint newPoint, string id);
    }
}
