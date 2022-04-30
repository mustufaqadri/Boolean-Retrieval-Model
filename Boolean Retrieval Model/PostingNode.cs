using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boolean_Retrieval_Model
{
    [Serializable()]
    class PostingNode
    {
        public int DocID;
        public LinkedList<int> PositionList;
        public String PositionStr;

        public void InsertFileNo(int Doc,String Word)
        {
            Text T1 = new Text();
            T1.RetrieveLexicon();
            DocID = Doc;
            int PosCount = 0;
            String[] New=T1.ArrangedFile[Doc].Split(' ');
            PositionList = new LinkedList<int>();
            foreach (String word in New)
            {
                if (word == Word)
                {
                    PositionStr += PosCount.ToString() + ",";
                    PositionList.AddLast(PosCount);
                }
                PosCount++;
            }
        }
    }
}