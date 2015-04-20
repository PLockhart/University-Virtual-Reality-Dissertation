using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Interop.Excel;

namespace ResultCombiner
{

    class QuestionaireResultStore
    {
        /// <summary>
        /// A list of lists, for each answer
        /// 1) list of results
        /// 2) list of results
        /// 3) etc
        /// </summary>
        public List<ExpResultStore> feedbackScores;

        public List<List<int>> controlSchemeScores;
        public List<List<int>> vrMapScores;
        public List<List<int>> gameExperienceScores;

        public QuestionaireResultStore(int possibleAnswers)
        {
            feedbackScores = new List<ExpResultStore>();
            controlSchemeScores = new List<List<int>>();
            vrMapScores = new List<List<int>>();
            gameExperienceScores = new List<List<int>>();

            for (int i = 0; i < possibleAnswers; i++)
            {
                feedbackScores.Add(new ExpResultStore());
                controlSchemeScores.Add(new List<int>());
                vrMapScores.Add(new List<int>());
                gameExperienceScores.Add(new List<int>());
            }
        }

        public void writeAverageWithTemplate(Worksheet ws, int x, int y)
        {
            writeTemplate(ws, x, y);

            for (int i = 0; i < feedbackScores.Count; i++)
            {
                ws.Cells[x, y + 1 + i] = i + 1;
                if (controlSchemeScores[i].Count != 0)
                    ws.Cells[x + 1, y + i + 1] = getAverage(controlSchemeScores[i]);
                if (vrMapScores[i].Count != 0)
                    ws.Cells[x + 2, y + i + 1] = getAverage(vrMapScores[i]);
                if (gameExperienceScores[i].Count != 0)
                    ws.Cells[x + 3, y + i + 1] = getAverage(gameExperienceScores[i]);

                writeAverageData(ws, x + 3, y + i + 1, i);
            }
        }

        protected virtual void writeAverageData(Worksheet ws, int x, int y, int scoreResult)
        {
            feedbackScores[scoreResult].writeAverageDownColumn(x, y, ws);
        }

        public void writeLastWithTemplate(Worksheet ws, int x, int y)
        {
            writeTemplate(ws, x, y);

            for (int i = 0; i < feedbackScores.Count; i++)
            {
                ws.Cells[x, y + 1 + i] = i + 1;
                if (controlSchemeScores[i].Count != 0)
                    ws.Cells[x + 1, y + i + 1] = getLast(controlSchemeScores[i]);
                if (vrMapScores[i].Count != 0)
                    ws.Cells[x + 2, y + i + 1] = getLast(vrMapScores[i]);
                if (gameExperienceScores[i].Count != 0)
                    ws.Cells[x + 3, y + i + 1] = getLast(gameExperienceScores[i]);

                writeLastData(ws, x + 3, y + i + 1, i);
            }
        }

        public void writeTemplate(Worksheet ws, int x, int y)
        {
            for (int i = 0; i < feedbackScores.Count; i++)
            {
                ws.Cells[x, y] = "Rated:";
                ws.Cells[x + 3, y] = "Game Experience Score:";
                ws.Cells[x + 2, y] = "Mapping Score:";
                ws.Cells[x + 1, y] = "Control Scheme Score:";
            }
        }

        protected virtual void writeLastData(Worksheet ws, int x, int y, int scoreResult)
        {
            feedbackScores[scoreResult].writeLastResultDownColumn(x, y, ws);
        }

        protected int getAverage(List<int> list)
        {
            return list.Sum() / list.Count;
        }

        protected int getLast(List<int> list)
        {
            return list[list.Count - 1];
        }
    }

    
}
