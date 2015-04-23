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

        /// <summary>
        /// is = (attention VR - attention baseline) - (attention nonVR - attention baseline)
        /// </summary>
        public List<int> deltaAttentRelativeVRAndNonVR;
        /// <summary>
        /// is = (meditation VR - meditation baseline) - (meditation nonVR - meditation baseline)
        /// </summary>
        public List<int> deltaMeditationRelativeVRAndNonVR;

        #region basic SD
        /// <summary>
        /// standard deviation for the VR attention - baseline attention
        /// </summary>
        public double relativeVRAttentionSD;
        /// <summary>
        /// standard deviation for non-VR attention - baseline attention
        /// </summary>
        public double relativeNonVRAttentionSD;
        /// <summary>
        /// standard deviation for the VR meditation - baseline meditation
        /// </summary>
        public double relativeVRMeditationSD;
        /// <summary>
        /// standard deviation for non-VR meditation - baseline meditation
        /// </summary>
        public double relativeNonVRMeditationSD;

        /// <summary>
        /// standard deviation for deltaAttentRelativeVRAndNonVR 
        /// </summary>
        public double deltaRelativeAttentionSD;
        /// <summary>
        /// standard deviation for deltaMeditationRelativeVRAndNonVR
        /// </summary>
        public double deltaRelativeMeditationSD;

        #endregion

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
            deltaAttentRelativeVRAndNonVR = new List<int>();
            deltaMeditationRelativeVRAndNonVR = new List<int>();
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
            ws.Cells[x + 11, y] = "Relative Attention VR - Relative Attention Non-VR";
            ws.Cells[x + 12, y] = "Relative Meditation VR - Relative Meditation Non-VR";

            ws.Cells[x + 13, y] = "Relative VR Attention SD";
            ws.Cells[x + 14, y] = "Relative non-VR attention SD";
            ws.Cells[x + 15, y] = "Relative VR Meditation SD";
            ws.Cells[x + 16, y] = "Relative non-VR Meditation SD";
            ws.Cells[x + 17, y] = "Delta relative Vr-NonVr attention SD";
            ws.Cells[x + 18, y] = "Delta relative Vr-NonVr meditation SD";
        }

        public virtual void writeLastResultDownColumn(int x, int y, Worksheet ws)
        {
            writeDownColumnWithDelegate(x, y, ws, ListAddons.getLast);
        }

        public virtual void writeAverageDownColumn(int x, int y, Worksheet ws)
        {
            writeDownColumnWithDelegate(x, y, ws, ListAddons.getAverage);
        }  

        public virtual void writeDownColumnWithDelegate(int x, int y, Worksheet ws, GetValFromIntListDel del)
        {
            if (nonVRAttention.Count != 0)
                ws.Cells[x + 1, y] = del(nonVRAttention);
            if (vrAttention.Count != 0)
                ws.Cells[x + 2, y] = del(vrAttention);
            if (nonVRMeditation.Count != 0)
                ws.Cells[x + 3, y] = del(nonVRMeditation);
            if (vrMeditation.Count != 0)
                ws.Cells[x + 4, y] = del(vrMeditation);
            if (deltaVRnonVRAttention.Count != 0)
                ws.Cells[x + 5, y] = del(deltaVRnonVRAttention);
            if (deltaVRandBaselineAttention.Count != 0)
                ws.Cells[x + 6, y] = del(deltaVRandBaselineAttention);
            if (deltaNonVRandBaselineAttention.Count != 0)
                ws.Cells[x + 7, y] = del(deltaNonVRandBaselineAttention);
            if (deltaVRnonVRMeditation.Count != 0)
                ws.Cells[x + 8, y] = del(deltaVRnonVRMeditation);
            if (deltaVRandBaselineMeditation.Count != 0)
                ws.Cells[x + 9, y] = del(deltaVRandBaselineMeditation);
            if (deltaNonVRandBaselineMeditation.Count != 0)
                ws.Cells[x + 10, y] = del(deltaNonVRandBaselineMeditation);
            if (deltaAttentRelativeVRAndNonVR.Count != 0)
                ws.Cells[x + 11, y] = del(deltaAttentRelativeVRAndNonVR);
            if (deltaMeditationRelativeVRAndNonVR.Count != 0)
                ws.Cells[x + 12, y] = del(deltaMeditationRelativeVRAndNonVR);

            //write the SDs
            ws.Cells[x + 13, y] = relativeVRAttentionSD;
            ws.Cells[x + 14, y] = relativeNonVRAttentionSD;
            ws.Cells[x + 15, y] = relativeVRMeditationSD;
            ws.Cells[x + 16, y] = relativeNonVRMeditationSD;
            ws.Cells[x + 17, y] = deltaRelativeAttentionSD;
            ws.Cells[x + 18, y] = deltaRelativeMeditationSD;
        }

        public virtual void calculateStandardDeviations()
        {
            relativeVRAttentionSD = ListAddons.getSDfromValues(deltaVRandBaselineAttention);
            relativeNonVRAttentionSD = ListAddons.getSDfromValues(deltaNonVRandBaselineAttention);

            relativeVRMeditationSD = ListAddons.getSDfromValues(deltaVRandBaselineMeditation);
            relativeNonVRMeditationSD = ListAddons.getSDfromValues(deltaNonVRandBaselineMeditation);

            deltaRelativeAttentionSD = ListAddons.getSDfromValues(deltaAttentRelativeVRAndNonVR);
            deltaRelativeMeditationSD = ListAddons.getSDfromValues(deltaMeditationRelativeVRAndNonVR);
        }
    }

    class Exp2Store : ExpResultStore
    {
        public List<int> vrFireworksSpawned;
        public List<int> nonVRFireworksSpawned;

        public List<double> vrTotalInteractionTimes;
        public List<double> nonVRTotalInteractionTimes;

        public double vrFireworksSpawnedSD;
        public double nonvrFireworksSpawnedSD;

        public double vrInteractionTimeSD;
        public double nonvrInteractionTimeSD;

        public Exp2Store()
            : base()
        {
            vrFireworksSpawned = new List<int>();
            nonVRFireworksSpawned = new List<int>();
            vrTotalInteractionTimes = new List<double>();
            nonVRTotalInteractionTimes = new List<double>();
        }

        public new static void writeResultStoreTemplate(Worksheet ws, int x, int y)
        {
            ws.Cells[x, y] = "Non-VR Fireworks Spawned";
            ws.Cells[x + 1, y] = "VR Fireworks Spawned";
            ws.Cells[x + 2, y] = "Non-VR Interaction Duration";
            ws.Cells[x + 3, y] = "VR Interaction Duration";

            ws.Cells[x + 4, y] = "VR Fireworks spawned SD";
            ws.Cells[x + 5, y] = "Non-VR Fireworks spawned SD";
            ws.Cells[x + 6, y] = "VR time SD";
            ws.Cells[x + 7, y] = "NonVR time SD";
        }

        public override void writeAverageDownColumn(int x, int y, Worksheet ws)
        {
            base.writeAverageDownColumn(x, y, ws);
            writeFireworkResultsDownColWithDel(x + 20, y, ws, ListAddons.getAverage, ListAddons.getAverage);
        }

        public override void writeLastResultDownColumn(int x, int y, Worksheet ws)
        {
            base.writeLastResultDownColumn(x, y, ws);
            writeFireworkResultsDownColWithDel(x + 20, y, ws, ListAddons.getLast, ListAddons.getLast);
        }

        public void writeFireworkResultsDownColWithDel(int x, int y, Worksheet ws, GetValFromIntListDel delInt, GetValFromDoubleListDel delDouble)
        {
            if (nonVRFireworksSpawned.Count != 0)
                ws.Cells[x, y] = delInt(nonVRFireworksSpawned);
            if (vrFireworksSpawned.Count != 0)
                ws.Cells[x + 1, y] = delInt(vrFireworksSpawned);
            if (vrTotalInteractionTimes.Count != 0)
                ws.Cells[x + 2, y] = delDouble(vrTotalInteractionTimes);
            if (nonVRTotalInteractionTimes.Count != 0)
                ws.Cells[x + 3, y] = delDouble(nonVRTotalInteractionTimes);

            ws.Cells[x + 4, y] = vrFireworksSpawnedSD;
            ws.Cells[x + 5, y] = nonvrFireworksSpawnedSD;
            ws.Cells[x + 6, y] = vrInteractionTimeSD;
            ws.Cells[x + 7, y] = nonvrInteractionTimeSD;
        }

        public override void calculateStandardDeviations()
        {
            base.calculateStandardDeviations();

            vrFireworksSpawnedSD = ListAddons.getSDfromValues(vrFireworksSpawned);
            nonvrFireworksSpawnedSD = ListAddons.getSDfromValues(nonVRFireworksSpawned);

            vrInteractionTimeSD = ListAddons.getSDfromValues(vrTotalInteractionTimes);
            nonvrInteractionTimeSD = ListAddons.getSDfromValues(nonVRTotalInteractionTimes);
        }
    }

    class Exp3Store : ExpResultStore
    {
        public List<double> vrTimesTaken;
        public List<double> nonVRTimesTaken;

        public double vrTimeSD;
        public double nonVRTimeSD;

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

            ws.Cells[x + 2, y] = "VR time SD";
            ws.Cells[x + 3, y] = "NonVR time SD";
        }

        public override void writeAverageDownColumn(int x, int y, Worksheet ws)
        {
            base.writeAverageDownColumn(x, y, ws);
            writeTagResultsDownColWithDel(x + 28, y, ws, ListAddons.getAverage);
        }

        public override void writeLastResultDownColumn(int x, int y, Worksheet ws)
        {
            base.writeLastResultDownColumn(x, y, ws);
            writeTagResultsDownColWithDel(x + 28, y, ws, ListAddons.getLast);
        }

        protected void writeTagResultsDownColWithDel(int x, int y, Worksheet ws, GetValFromDoubleListDel delDouble)
        {
            if (nonVRTimesTaken.Count != 0)
                ws.Cells[x, y] = delDouble(nonVRTimesTaken);
            if (vrTimesTaken.Count != 0)
                ws.Cells[x + 1, y] = delDouble(vrTimesTaken);

            ws.Cells[x + 2, y] = vrTimeSD;
            ws.Cells[x + 3, y] = nonVRTimeSD;
        }

        public override void calculateStandardDeviations()
        {
            base.calculateStandardDeviations();

            vrTimeSD = ListAddons.getSDfromValues(vrTimesTaken);
            nonVRTimeSD = ListAddons.getSDfromValues(nonVRTimesTaken);
        }
    }
}
