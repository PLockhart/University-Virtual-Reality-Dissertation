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

        public virtual void writeLastResultDownColumn(int x, int y, Worksheet ws)
        {
            ws.Cells[x, y] = getLast(nonVRAttention);
            ws.Cells[x + 1, y] = getLast(vrAttention);
            ws.Cells[x + 2, y] = getLast(nonVRMeditation);
            ws.Cells[x + 3, y] = getLast(vrMeditation);
            ws.Cells[x + 4, y] = getLast(deltaVRnonVRAttention);
            ws.Cells[x + 5, y] = getLast(deltaVRandBaselineAttention);
            ws.Cells[x + 6, y] = getLast(deltaNonVRandBaselineAttention);
            ws.Cells[x + 7, y] = getLast(deltaVRnonVRMeditation);
            ws.Cells[x + 8, y] = getLast(deltaVRandBaselineMeditation);
            ws.Cells[x + 9, y] = getLast(deltaNonVRandBaselineMeditation);
        }

        public virtual void writeDownColumn(int x, int y, Worksheet ws)
        {
            ws.Cells[x, y] = getAverage(nonVRAttention);
            ws.Cells[x + 1, y] = getAverage(vrAttention);
            ws.Cells[x + 2, y] = getAverage(nonVRMeditation);
            ws.Cells[x + 3, y] = getAverage(vrMeditation);
            ws.Cells[x + 4, y] = getAverage(deltaVRnonVRAttention);
            ws.Cells[x + 5, y] = getAverage(deltaVRandBaselineAttention);
            ws.Cells[x + 6, y] = getAverage(deltaNonVRandBaselineAttention);
            ws.Cells[x + 7, y] = getAverage(deltaVRnonVRMeditation);
            ws.Cells[x + 8, y] = getAverage(deltaVRandBaselineMeditation);
            ws.Cells[x + 9, y] = getAverage(deltaNonVRandBaselineMeditation);
        }  

        private int getAverage(List<int> list)
        {
            return list.Sum() / list.Count;
        }

        private int getLast(List<int> list)
        {
            return list.Count == 0 ? -1 : list[list.Count - 1];
        }
    }

    class Exp2Store : ExpResultStore
    {
        public List<int> vrFireworksSpawned;
        public List<int> nonVRFireworksSpawned;

        public Exp2Store()
            : base()
        {
            vrFireworksSpawned = new List<int>();
            nonVRFireworksSpawned = new List<int>();
        }

        public override void writeDownColumn(int x, int y, Worksheet ws)
        {
            base.writeDownColumn(x, y, ws);

            //ws.Cells[x + 11, y] = //TODO: write the spawn times
        }

        public override void writeLastResultDownColumn(int x, int y, Worksheet ws)
        {
            base.writeLastResultDownColumn(x, y, ws);
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

        public override void writeDownColumn(int x, int y, Worksheet ws)
        {
            base.writeDownColumn(x, y, ws);
        }

        public override void writeLastResultDownColumn(int x, int y, Worksheet ws)
        {
            base.writeLastResultDownColumn(x, y, ws);
        }
    }
}
