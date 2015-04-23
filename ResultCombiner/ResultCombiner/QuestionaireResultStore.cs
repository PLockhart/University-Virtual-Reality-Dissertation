using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Interop.Excel;

namespace ResultCombiner
{

    class QuestionaireResultStore<StoreType> where StoreType : ExpResultStore, new()
    {
        /// <summary>
        /// A list of result stores, for each answer to a feedback question
        /// 1) list of results
        /// 2) list of results
        /// 3) etc
        /// </summary>
        public List<StoreType> feedbackStores;

        public List<List<int>> controlSchemeScores;
        public List<List<int>> vrMapScores;
        public List<List<int>> gameExperienceScores;

        public QuestionaireResultStore(int possibleAnswers)
        {
            feedbackStores = new List<StoreType>();
            controlSchemeScores = new List<List<int>>();
            vrMapScores = new List<List<int>>();
            gameExperienceScores = new List<List<int>>();

            for (int i = 0; i < possibleAnswers; i++)
            {
                feedbackStores.Add(new StoreType());
                controlSchemeScores.Add(new List<int>());
                vrMapScores.Add(new List<int>());
                gameExperienceScores.Add(new List<int>());
            }
        }

        protected void writeDataWithDel(GetValFromIntListDel del, Worksheet ws, int x, int y, int i)
        {
            ws.Cells[x, y + 1 + i] = i + 1;
            if (controlSchemeScores[i].Count != 0)
                ws.Cells[x + 1, y + i + 1] = del(controlSchemeScores[i]);
            if (vrMapScores[i].Count != 0)
                ws.Cells[x + 2, y + i + 1] = del(vrMapScores[i]);
            if (gameExperienceScores[i].Count != 0)
                ws.Cells[x + 3, y + i + 1] = del(gameExperienceScores[i]);
        }

        public void writeAverage(Worksheet ws, int x, int y)
        {
            for (int i = 0; i < feedbackStores.Count; i++)
            {
                writeDataWithDel(ListAddons.getAverage, ws, x, y, i);

                writeAverageData(ws, x + 3, y + i + 1, i);
            }
        }

        protected virtual void writeAverageData(Worksheet ws, int x, int y, int scoreResult)
        {
            feedbackStores[scoreResult].writeAverageDownColumn(x, y, ws);
        }

        public void writeLast(Worksheet ws, int x, int y)
        {
            for (int i = 0; i < feedbackStores.Count; i++)
            {
                writeDataWithDel(ListAddons.getLast, ws, x, y, i);

                writeLastData(ws, x + 3, y + i + 1, i);
            }
        }

        public void writeTemplate(Worksheet ws, int x, int y)
        {
            for (int i = 0; i < feedbackStores.Count; i++)
            {
                ws.Cells[x, y] = "Rated:";
                ws.Cells[x + 3, y] = "Game Experience Score:";
                ws.Cells[x + 2, y] = "Mapping Score:";
                ws.Cells[x + 1, y] = "Control Scheme Score:";
            }
        }

        protected virtual void writeLastData(Worksheet ws, int x, int y, int scoreResult)
        {
            feedbackStores[scoreResult].writeLastResultDownColumn(x, y, ws);
        }
    }

    
}
