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

namespace Boolean_Retrieval_Model
{
    class InvertedIndex
    {
        public Text TermList;

        // Dictionary is equivalent to Hashmap of JAVA
        public Dictionary<String,LinkedList<int>> Table;
        public String Temp;
        public String InvertedIndexString;
        public String Query;
        public String Result;
        public String LexiconSize;

        public InvertedIndex()
        {
            Table = new Dictionary<String,LinkedList<int>>();
            TermList=new Text();
            //TermList.CreateLexicon();
            TermList.RetrieveLexicon();
        }
        public void ConstructInvertedIndex()
        {
            String[] NewStr;

            NewStr = TermList.FinalString.Split(' ');
            foreach (String word in NewStr)
            {
                LinkedList<int> P = new LinkedList<int>();
                for (int FileNo = 1; FileNo < 51; FileNo++)
                {
                    if (TermList.ArrangedFile[FileNo].Contains(" "+word+" ") == true)
                    {
                        if (Table.ContainsKey(word) == false)
                        {
                            P.AddLast(FileNo);
                            Table.Add(word,P);
                            Temp += FileNo.ToString() + ",";
                        }
                        else
                        {
                            P.AddLast(FileNo);
                            Temp += FileNo.ToString() + ",";
                        }
                    }
                }
                InvertedIndexString = InvertedIndexString + word + " : " + Temp + "\n";
                Temp = " ";
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
            File.WriteAllText("Inverted Index.txt", InvertedIndexString);
        }
        public void QueryProcessing()
        {
            Query = Query.ToLower();
            Result = "";

            // Query Words Count
            int ChCount = 0, WordCount = 0;
            int Len = Query.Length;
            while (ChCount < Len)
            {
                if (Query[ChCount] == ' ')
                {
                    WordCount++;
                }
                ChCount++;
            }

            var Str1 = "";
            var Str2 = "";
            int NotCount = 0;
            bool flagAnd = false,flagNot = false,flagOr=false;

            if (Query != " " && Query != "\0")
            {
                String[] Keyword;

                Keyword = Query.Split(' ');
                int DocCount = 0;
                
                foreach (String word in Keyword)
                {
                    if (word != "and" && word != "or" && word != "not")
                    {
                        if (Table.ContainsKey(word))
                        {
                            foreach (var kvp in Table)
                            {
                                if (kvp.Key == word)
                                {
                                    LinkedList<int>.Enumerator It = kvp.Value.GetEnumerator();
                                    if (flagNot == true)
                                    {
                                        int[] Arr = new int[51];
                                        Arr[0] = 0;
                                        int i=1;
                                        while (It.MoveNext())
                                        {
                                            Arr[i] = It.Current;
                                            i++;
                                        }
                                        int j=1;
                                        for (i = 1; i <= 50; i++)
                                        {
                                            if(Arr[j]!=i)
                                            {
                                                Str1 += i.ToString() + " ";
                                            }
                                            else if(Arr[j]==i)
                                            {
                                                j++;
                                            }
                                        }
                                        flagNot = false;
                                    }
                                    else
                                    {
                                        while (It.MoveNext())
                                        {
                                            Str1 += It.Current.ToString() + " ";
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Result = "No Documents Retrieved";
                        }
                    }
                    else if (word == "not")
                    {
                        flagNot = true;
                        NotCount++;
                  
                        if(NotCount%2==0)
                        {
                            flagNot=false;
                        }
                    }
                    else if (word == "or")
                    {
                        flagOr = true;
                    }
                    else if (word == "and")
                    {
                        Str2 = Str1;
                        Str1 = "";
                        flagAnd = true;                
                    }
                }
                
                if (flagOr == true)
                {
                    Result = string.Join(" ", Str1.Split(' ').Distinct());
                }
                else if (flagNot == true)
                {
                    Result = string.Join(" ", Str1.Split(' ').Distinct());
                }
                else if (flagAnd == true)
                {
                    if (Str1.Length >= Str2.Length)
                    {
                        String[] N1;
                        N1=Str2.Split(' ');
                        foreach (String word in N1)
                        {
                            if(Str1.Contains(word))
                            {
                                if (Result.Contains(word) == false)
                                {
                                    Result += word + " ";
                                }
                            }
                        }
                    }
                    else
                    {
                        String[] N1;
                        N1 = Str1.Split(' ');
                        foreach (String word in N1)
                        {
                            if (Str2.Contains(word))
                            {
                               if (Result.Contains(word) == false)
                               {
                                    Result += word + " ";
                                }
                            }
                        }
                    }
                    Result = string.Join(" ", Result.Split(' ').Distinct());
                }
                else if (flagNot == false && flagAnd == false && flagOr == false)
                {
                    Result = string.Join(" ", Str1.Split(' ').Distinct());
                }

                // Query Documents Count
                DocCount = 0;
                WordCount = 0;
                ChCount = 0;
                Len = Result.Length;
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
                FileStream writerFileStream = new FileStream("Inverted Index.dat", FileMode.Create, FileAccess.Write);
                formatter.Serialize(writerFileStream,Table);
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
            
            if (File.Exists("Inverted Index.dat"))
            {
                BinaryFormatter formatter;
                formatter = new BinaryFormatter();
                try
                {
                    FileStream readerFileStream = new FileStream("Inverted Index.dat",FileMode.Open, FileAccess.Read);
                    Table = (Dictionary<String, LinkedList<int>>)formatter.Deserialize(readerFileStream);
                    readerFileStream.Close();
                    InvertedIndexString = File.ReadAllText("Inverted Index.txt");
                }
                catch (Exception)
                {
                    Temp = "Unable";
                }
            }
        }
    }
}