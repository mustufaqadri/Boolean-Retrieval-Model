using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// HASH MAP
using System.Collections;
// FOR FILING
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
// Regexr
using System.Text.RegularExpressions;
namespace Boolean_Retrieval_Model
{
    class PositionalIndex
    {
        public Text TermList;

        // Dictionary is equivalent to Hashmap of JAVA
        public Dictionary<string,LinkedList<PostingNode>> Table; 
        public String Temp;
        public String PositonalIndexString;
        public String Query;
        public String Result;
        public String LexiconSize;

        public PositionalIndex()
        {
            Table = new Dictionary<string, LinkedList<PostingNode>>();
            TermList = new Text();
            //TermList.CreateLexicon();
            TermList.RetrieveLexicon();
        }
        public void ConstructPositionalIndex()
        {
            String[] NewStr;

            NewStr = TermList.FinalString.Split(' ');
            foreach (String word in NewStr)
            {
                LinkedList<PostingNode> P = new LinkedList<PostingNode>();
                for (int FileNo = 1; FileNo < 51; FileNo++)
                {
                    if (TermList.ArrangedFile[FileNo].Contains(" " + word + " ") == true)
                    {
                        if (Table.ContainsKey(word) == false)
                        {
                            PostingNode NewNode = new PostingNode();
                            NewNode.InsertFileNo(FileNo,word);
                            P.AddLast(NewNode);
                            Table.Add(word,P);
                            Temp += FileNo.ToString() +"( "+NewNode.PositionStr+" ) ,";
                        }
                        else
                        {
                            PostingNode NewNode = new PostingNode();
                            NewNode.InsertFileNo(FileNo, word);
                            P.AddLast(NewNode);
                            Temp += FileNo.ToString() + "( " + NewNode.PositionStr + " ) ,";
                        }
                    }
                }
                PositonalIndexString = PositonalIndexString + word + " : " + Temp + "\n";
                Temp = "";
            }

            // Lexicon Size 
            int ChCount = 0, WordCount = 0;
            int Len = TermList.FinalString.Length;
            while (ChCount < Len)
            {
                if (TermList.FinalString[ChCount] == ' ')
                {
                    WordCount++;
                }
                ChCount++;
            }
            LexiconSize = WordCount.ToString();

            // Writing Inverted Index on file
            File.WriteAllText("Positional Index.txt", PositonalIndexString);
        }
        
        public void QueryProcessing()
        {
            Query = Query.ToLower();
            Result = "";
            var Str1 = "";
            var Str2 = "";
            if (Query != " " && Query != "\0")
            {
                String[] Keyword;
                String Gap = "";
                int K = Query.IndexOf("/");
                Gap = Query[K + 1].ToString();
                K = Convert.ToInt32(Gap);
                K++; ;
                MatchCollection MC = Regex.Matches(Query, @"([A-Z]*[a-z]*[0-9]*)\w+");
                Query = "";
                foreach (Match M in MC)
                {
                    Query += M + " ";
                }
                Keyword = Query.Split(' ');

                int DocCount = 0;

                LinkedList<PostingNode> P1 = new LinkedList<PostingNode>();
                LinkedList<PostingNode> P2 = new LinkedList<PostingNode>();


                bool Flag = true;
                foreach(String word in Keyword)
                {
                    if(Table.ContainsKey(word) && Flag==true)
                    {
                        P1 = Table[word];
                        Flag = false;
                    }
                    else if(Table.ContainsKey(word) && Flag == false)
                    {
                        P2 = Table[word];
                        break;
                    }
                }

                LinkedList<PostingNode>.Enumerator It1 = P1.GetEnumerator();
                LinkedList<PostingNode>.Enumerator It2 = P2.GetEnumerator();
                LinkedList<int> P1Posting = new LinkedList<int>();
                LinkedList<int> P2Posting = new LinkedList<int>();
                LinkedList<int>.Enumerator PIt1 = P1Posting.GetEnumerator();
                LinkedList<int>.Enumerator PIt2 = P2Posting.GetEnumerator();
                It1.MoveNext();
                It2.MoveNext();
                while (true)
                { 
                    if (It1.Current.DocID == It2.Current.DocID)
                    {
                        P1Posting = It1.Current.PositionList;
                        P2Posting = It2.Current.PositionList;
                        
                        PIt1 = P1Posting.GetEnumerator();
                        PIt2 = P2Posting.GetEnumerator();
                        PIt1.MoveNext();
                        PIt2.MoveNext();
                        while (true)
                        {
                            if (Math.Abs(PIt1.Current - PIt2.Current) <= K)
                            {
                                Result += It1.Current.DocID + " ";
                                if(PIt1.MoveNext() && PIt2.MoveNext())
                                {
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else if(PIt1.Current>PIt2.Current)
                            { 
                                if(PIt2.MoveNext())
                                {
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (PIt1.MoveNext())
                                {
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (It1.MoveNext() && It2.MoveNext())
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (It1.Current.DocID > It2.Current.DocID)
                    {
                        if (It2.MoveNext())
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (It1.MoveNext())
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                Result = string.Join(" ", Result.Split(' ').Distinct());
                // Doc Count
                int ChCount = 0;
                int Len = Result.Length;
                while (ChCount < Len)
                {
                    if (Result[ChCount] == ' ')
                    {
                        DocCount++;
                    }
                    ChCount++;
                }
                Result += "\n\nDocuments Retreived : " + DocCount;
                
            }
            else
            {
                Result = "Invalid Query";
            }
        }
        public void WriteDictionary()
        {
            BinaryFormatter formatter;
            formatter = new BinaryFormatter();
            try
            {
                FileStream writerFileStream = new FileStream("Positional Index.dat", FileMode.Create, FileAccess.Write);
                formatter.Serialize(writerFileStream, Table);
                writerFileStream.Close();
            }
            catch (Exception)
            {
                Temp = "Unable";
            }
        }
        public void ReadDictionary()
        {
            // Lexicon Size
            int ChCount = 0, WordCount = 0;
            int Len = TermList.FinalString.Length;
            while (ChCount < Len)
            {
                if (TermList.FinalString[ChCount] == ' ')
                {
                    WordCount++;
                }
                ChCount++;
            }
            LexiconSize = WordCount.ToString();

            if (File.Exists("Positional Index.dat"))
            {
                BinaryFormatter formatter;
                formatter = new BinaryFormatter();
                try
                {
                    FileStream readerFileStream = new FileStream("Positional Index.dat", FileMode.Open, FileAccess.Read);
                    Table = (Dictionary<String, LinkedList<PostingNode>>)formatter.Deserialize(readerFileStream);
                    readerFileStream.Close();
                    PositonalIndexString = File.ReadAllText("Positional Index.txt");
                }
                catch (Exception)
                {
                    Temp = "Unable";
                }
            }
        }
    }
}