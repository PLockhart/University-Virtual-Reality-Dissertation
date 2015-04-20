using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace ResultCombiner
{
    class ExpResultStore
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

        public static void writeResultStoreTemplate(Worksheet ws, int x, int y)
        {
            //row labels
            ws.Cells[x + 1, y] = "Non-VR Attention";
            ws.Cells[x + 2, y] = "VR Attention";
            ws.Cells[x + 3, y] = "Non-VR Meditation";
            ws.Cells[x + 4, y] = "VR Meditation";
            ws.Cells[x + 5, y] = "VR Attention - Non-Vr Attention";
            ws.Cells[x + 6, y] = "VR Attention - Baseline Attention";
            ws.Cells[x + 7, y] = "Non-VR Attention - Baseline Attention";
            ws.Cells[x + 8, y] = "VR Meditation - Non-Vr Meditation";
            ws.Cells[x + 9, y] = "VR Meditation - Baseline Meditation";
            ws.Cells[x + 10, y] = "Non-VR Meditation - Baseline Meditation";
        }

        public void writeLastResultDownColumn(int x, int y, Worksheet ws)
        {
            if (nonVRAttention.Count != 0)
                ws.Cells[x + 1, y] = getLast(nonVRAttention);
            if (vrAttention.Count != 0)
                ws.Cells[x + 2, y] = getLast(vrAttention);
            if (nonVRMeditation.Count != 0)
                ws.Cells[x + 3, y] = getLast(nonVRMeditation);
            if (vrMeditation.Count != 0)
                ws.Cells[x + 4, y] = getLast(vrMeditation);
            if (deltaVRnonVRAttention.Count != 0)
                ws.Cells[x + 5, y] = getLast(deltaVRnonVRAttention);
            if (deltaVRandBaselineAttention.Count != 0)
                ws.Cells[x + 6, y] = getLast(deltaVRandBaselineAttention);
            if (deltaNonVRandBaselineAttention.Count != 0)
                ws.Cells[x + 7, y] = getLast(deltaNonVRandBaselineAttention);
            if (deltaVRnonVRMeditation.Count != 0)
                ws.Cells[x + 8, y] = getLast(deltaVRnonVRMeditation);
            if (deltaVRandBaselineMeditation.Count != 0)
                ws.Cells[x + 9, y] = getLast(deltaVRandBaselineMeditation);
            if (deltaNonVRandBaselineMeditation.Count != 0)
                ws.Cells[x + 10, y] = getLast(deltaNonVRandBaselineMeditation);
        }

        public void writeAverageDownColumn(int x, int y, Worksheet ws)
        {
            if (nonVRAttention.Count != 0)
                ws.Cells[x + 1, y] = getAverage(nonVRAttention);
            if (vrAttention.Count != 0)
                ws.Cells[x + 2, y] = getAverage(vrAttention);
            if (nonVRMeditation.Count != 0)
                ws.Cells[x + 3, y] = getAverage(nonVRMeditation);
            if (vrMeditation.Count != 0)
                ws.Cells[x + 4, y] = getAverage(vrMeditation);
            if (deltaVRnonVRAttention.Count != 0)
                ws.Cells[x + 5, y] = getAverage(deltaVRnonVRAttention);
            if (deltaVRandBaselineAttention.Count != 0)
                ws.Cells[x + 6, y] = getAverage(deltaVRandBaselineAttention);
            if (deltaNonVRandBaselineAttention.Count != 0)
                ws.Cells[x + 7, y] = getAverage(deltaNonVRandBaselineAttention);
            if (deltaVRnonVRMeditation.Count != 0)
                ws.Cells[x + 8, y] = getAverage(deltaVRnonVRMeditation);
            if (deltaVRandBaselineMeditation.Count != 0)
                ws.Cells[x + 9, y] = getAverage(deltaVRandBaselineMeditation);
            if (deltaNonVRandBaselineMeditation.Count != 0)
                ws.Cells[x + 10, y] = getAverage(deltaNonVRandBaselineMeditation);
        }  

        protected int getAverage(List<int> list)
        {
            return list.Count == 0 ? -1 : list.Sum() / list.Count;
        }

        protected int getLast(List<int> list)
        {
            return list.Count == 0 ? -1 : list[list.Count - 1];
        }

        protected double getAverage(List<double> list)
        {
            return list.Count == 0 ? -1 : list.Sum() / list.Count;
        }

        protected double getLast(List<double> list)
        {
            return list.Count == 0 ? -1 : list[list.Count - 1];
        }
    }

    class Exp2Store : ExpResultStore
    {
        public List<int> vrFireworksSpawned;
        public List<int> nonVRFireworksSpawned;

        public List<double> vrTotalInteractionTimes;
        public List<double> nonVRTotalInteractionTimes;

        public Exp2Store()
            : base()
        {
            vrFireworksSpawned = new List<int>();
            nonVRFireworksSpawned = new List<int>();

            vrTotalInteractionTimes = new List<double>();
            nonVRTotalInteractionTimes = new List<double>();
        }

        public static void writeResultStoreTemplate(Worksheet ws, int x, int y)
        {
            ws.Cells[x, y] = "Non-VR Fireworks Spawned";
            ws.Cells[x + 1, y] = "VR Fireworks Spawned";
            ws.Cells[x + 2, y] = "Non-VR Interaction Duration";
            ws.Cells[x + 3, y] = "VR Interaction Duration";
        }

        public new void writeAverageDownColumn(int x, int y, Worksheet ws)
        {
            ws.Cells[x, y] = getAverage(nonVRFireworksSpawned);
            ws.Cells[x + 1, y] = getAverage(vrFireworksSpawned);
            ws.Cells[x + 2, y] = getAverage(vrTotalInteractionTimes);
            ws.Cells[x + 3, y] = getAverage(nonVRTotalInteractionTimes);
        }

        public new void writeLastResultDownColumn(int x, int y, Worksheet ws)
        {
            ws.Cells[x, y] = getLast(nonVRFireworksSpawned);
            ws.Cells[x + 1, y] = getLast(vrFireworksSpawned);
            ws.Cells[x + 2, y] = getLast(vrTotalInteractionTimes);
            ws.Cells[x + 3, y] = getLast(nonVRTotalInteractionTimes);
        }
    }

    class Exp3Store : ExpResultStore
    {
        public List<double> vrTimesTaken;
        public List<double> nonVRTimesTaken;

        public Exp3Store()
            : base()
        {
            vrTimesTaken = new List<double>();
            nonVRTimesTaken = new List<double>();
        }

        public new static void writeResultStoreTemplate(Worksheet ws, int x, int y)
        {
            ws.Cells[x, y] = "Non-VR Tag Duration";
            ws.Cells[x + 1, y] = "VR Tag Duration";
        }

        public new void writeAverageDownColumn(int x, int y, Worksheet ws)
        {
            ws.Cells[x, y] = getAverage(nonVRTimesTaken);
            ws.Cells[x + 1, y] = getAverage(vrTimesTaken);
        }

        public new void writeLastResultDownColumn(int x, int y, Worksheet ws)
        {
            ws.Cells[x, y] = getLast(nonVRTimesTaken);
            ws.Cells[x + 1, y] = getLast(vrTimesTaken);
        }
    }
}
