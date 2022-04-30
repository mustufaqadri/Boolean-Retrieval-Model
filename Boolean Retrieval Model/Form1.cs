using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;


namespace Boolean_Retrieval_Model
{
    public partial class Form1 : Form
    {
        //Text T = new Text();
        InvertedIndex I = new InvertedIndex();
        PositionalIndex P = new PositionalIndex();

        int InvertedFlag = 0;
        int PostingFlag = 0;
        bool PostingFlagQuery = true;
        bool InvertedFlagQuery = false;
        public Form1()
        {
            InitializeComponent();
        }

        // LOADING
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
            this.AcceptButton = button5;
        }

        // AND
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + " AND";
        }

        // OR
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + " OR";
        }

        // NOT
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + " NOT";
        }

        // FORWARD SLASH
        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text + " /";
        }

        // SEARCH
        private void button5_Click(object sender, EventArgs e)
        {
            if (InvertedFlagQuery == true)
            {
                I.Query = textBox1.Text;
                I.QueryProcessing();
                richTextBox1.Text = I.Result;
                textBox2.Text = I.LexiconSize;
            }
            else if(PostingFlagQuery==true)
            {
                P.Query = textBox1.Text;
                P.QueryProcessing();
                richTextBox1.Text = P.Result;
                textBox2.Text = P.LexiconSize;
            }
        }

        // CLEAR
        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = "\0";
            richTextBox1.Text = "\0";
        }

        // CONSTRUCT Inverted INDEX
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //I.ConstructInvertedIndex();
            //I.WriteDictionary();
            I.ReadDictionary();
            textBox2.Text = I.LexiconSize;
            MessageBox.Show("Inverted Index Created", "Succeeded", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
            richTextBox2.Text = I.InvertedIndexString;
            PostingFlagQuery = false;
            InvertedFlagQuery = true;
        }

        // CONSTRUCT Positional Index
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (PostingFlag == 0)
            {
                //P.ConstructPositionalIndex();
                //P.WriteDictionary();
                P.ReadDictionary();
                PostingFlag = 1;
            }

            textBox2.Text = I.LexiconSize;
            MessageBox.Show("Positional Index Created", "Succeeded", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
            richTextBox2.Text = P.PositonalIndexString;
            PostingFlagQuery = true;
            InvertedFlagQuery = false;
        }
    }
}