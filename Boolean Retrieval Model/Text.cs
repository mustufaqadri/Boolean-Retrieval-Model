using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
// FILILNG
using System.IO;
// Regexr
using System.Text.RegularExpressions;

namespace Boolean_Retrieval_Model
{
    class Text
    {
        public String[] TempDocs;
        public String[] RawDocs;
        public String[] ArrangedFile;
        public String TempString;
        public String FinalString;

        public Text()
        {
            TempDocs=new String[51];
            RawDocs=new String[51];
            ArrangedFile = new String[51];
        }

        public void CreateLexicon()
        {
            RawDocs[0] = "\0";
            TempDocs[0] = "\0";
            ArrangedFile[0] = "\0";

            int TotalFiles = 51;
            // OBTAINING RAW FILES IN LOWERCASE
            for (int FileNo = 1; FileNo < TotalFiles; FileNo++)
            {
                RawDocs[FileNo] = File.ReadAllText(FileNo + ".txt");
                RawDocs[FileNo] = RawDocs[FileNo].ToLower();
            }

            // REMOVAL OF SPECIAL SYMBOLS 
            for (int FileNo = 1; FileNo < TotalFiles; FileNo++)
            {
                MatchCollection MC = Regex.Matches(RawDocs[FileNo], @"([A-Z]*[a-z]*[0-9]*)\w+");
                foreach (Match M in MC)
                {
                    TempDocs[FileNo] += M + " ";
                }
            }

            // STOP WORDS REMOVAL
            String[] StopWords = new String[] { "a", "is", "the", "of", "all", "and", "to", "can", "be", "as", "once", "for", "at", "am", "are", "has", "have", "had", "up", "his", "her", "in", "on", "no", "no", "we", "do","_a","_exactly_"};
            String[] NewStr;


            for (int FileNo = 1; FileNo < TotalFiles; FileNo++)
            {
                NewStr = TempDocs[FileNo].Split(' ');

                foreach (String word in NewStr)
                {
                    if (!StopWords.Contains(word))
                    {
                        TempString += word + " ";
                        ArrangedFile[FileNo] += word + " ";
                    }
                }
            }

            // Remove Duplicates
            FinalString = string.Join(" ", TempString.Split(' ').Distinct());

            // Sort Lexicon
            String[] NewStr1;

            NewStr1 = FinalString.Split(' ');
            ArrayList SortedLexicon = new ArrayList();
            foreach (String word in NewStr1)
            {
                SortedLexicon.Add(word);
            }
            SortedLexicon.Sort();
            FinalString = "";
            foreach (String word in SortedLexicon)
            {
                FinalString += word + " ";
            }

            // Writing Lexicon to file
            File.WriteAllText("Lexicon File.txt", FinalString);

            // Creating Arranged Files
            for (int FileNo = 1; FileNo < TotalFiles; FileNo++)
            {
                File.WriteAllText("Arranged File "+FileNo+".txt", ArrangedFile[FileNo]);
            }
        }

        public void RetrieveLexicon()
        {
            FinalString = File.ReadAllText("Lexicon File.txt");
            ArrangedFile[0] = "\0";
            
            // OBTAINING Arranged Files
            for (int FileNo = 1; FileNo < 51 ; FileNo++)
            {
                ArrangedFile[FileNo] = File.ReadAllText("Arranged File "+FileNo + ".txt");
            }
             
        }
    }
}