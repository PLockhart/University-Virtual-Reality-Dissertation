using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultCombiner
{

    public delegate double GetValFromIntListDel(List<int> list);
    public delegate double GetValFromDoubleListDel(List<double> list);

    public static class ListAddons
    {
        public static double getAverage(this List<int> list)
        {
            return list.Count == 0 ? -1 : list.Sum() / (double)list.Count;
        }

        public static double getLast(this List<int> list)
        {
            return list.Count == 0 ? -1 : list[list.Count - 1];
        }

        public static double getAverage(this List<double> list)
        {
            return list.Count == 0 ? -1 : list.Sum() / list.Count;
        }

        public static double getLast(this List<double> list)
        {
            return list.Count == 0 ? -1 : list[list.Count - 1];
        }


        public static double getSDfromValues(this List<double> list)
        {
            double average = list.getAverage();

            double[] meanDistanceVals = new double[list.Count];
            double distAverage = 0;
            for (int i = 0; i < meanDistanceVals.Length; i++)
            {
                meanDistanceVals[i] = Math.Pow(list[i] - average, 2);
                distAverage += meanDistanceVals[i];
            }

            return Math.Sqrt(distAverage / list.Count);
        }

        public static double getSDfromValues(this List<int> list)
        {
            double average = list.getAverage();

            double[] meanDistanceVals = new double[list.Count];
            double distAverage = 0;
            for (int i = 0; i < meanDistanceVals.Length; i++)
            {
                meanDistanceVals[i] = Math.Pow(list[i] - average, 2);
                distAverage += meanDistanceVals[i];
            }

            return Math.Sqrt(distAverage / list.Count);
        }
    }

    public static class MathAddons
    {
        public static double getSDFromArray(double[] list)
        {
            double average = 0;
            foreach (double loopedDouble in list)
                average += loopedDouble;

            average = average / list.Length;

            double[] meanDistanceVals = new double[list.Length];
            double distAverage = 0;
            for (int i = 0; i < meanDistanceVals.Length; i++)
            {
                meanDistanceVals[i] = Math.Pow(list[i] - average, 2);
                distAverage += meanDistanceVals[i];
            }

            return Math.Sqrt(distAverage / list.Length);
        }
    }
}
