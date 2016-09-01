using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//=====================================================

//(c) Copyright 2014 Rishabh Katiyar
//All Rights Reserved
               
//This program is the proprietary information

//=====================================================
namespace Poker
{
    public partial class Form1 : Form
    {
        public Form1 ob;
        public static int smallBlind;
        public static int smallBlindDone = 0;
        public static int bigBlindDone = 0;
        public static int numberOfPlayers = 8;
        public static int[] initialMoney = new int[numberOfPlayers];
        public static int[] onTable = new int[numberOfPlayers];
        public static int[] allInDone = new int[numberOfPlayers];
        public static int[] folded = new int[numberOfPlayers];
        public static int maxBet()
        {
            int i = 0;
            int max = 0;
            for (i = 0; i < numberOfPlayers; i++)
            {
                if (max < onTable[i])
                {
                    max = onTable[i];
                }
            }
            return max;
        }
        public int isLoser(int player, int[] players, int LEN)
        {
            int i = 0;
            int val = -1;
            for (i = 0; i < LEN; i++)
            {
                if (player == players[i])
                {
                    val = i;
                }
            }
            return val;
        }
        public static int total()
        {
            int sum = 0, i;
            for (i = 0; i < numberOfPlayers; i++)
            {
                if (onTable[i] > 0)
                {
                    sum += onTable[i];
                    onTable[i] = 0;
                }
            }
            return sum;
        }
        public void blind(int player, int mul)
        {
            int temp = initialMoney[player];
            temp -= smallBlind * mul;
            if (temp < 0)
            {
                MessageBox.Show("Can not play blind, insufficient balance");
                return;
            }
            initialMoney[player] = temp;
            if (mul == 1)
                smallBlindDone = 1;
            else
                bigBlindDone = 1;
            onTable[player] = smallBlind * mul;
        }
        public void check(int player)
        {
            int max = maxBet();
            int bet = max - onTable[player];
            int temp = initialMoney[player];
            temp -= bet;
            if (temp < 0)
            {
                MessageBox.Show("Can not check, insufficient balance");
                return;
            }
            initialMoney[player] = temp;
            onTable[player] += bet;
        }
        public void raise(int player, int amount)
        {
            int bet = maxBet() - onTable[player];
            int temp = initialMoney[player];
            temp -= bet;
            if (temp < 0)
            {
                MessageBox.Show("Can not check, insufficient balance");
                return;
            }
            temp -= amount;
            if (temp < 0)
            {
                MessageBox.Show("Only check allowed");
                return;
            }
            initialMoney[player] = temp;
            onTable[player] += (amount + bet);
            displayTotalAndOnTable();
            refreshRaise();
        }
        public void allIn(int player)
        {
            int temp = initialMoney[player];
            allInDone[player] = 1;
            onTable[player] += temp;
            initialMoney[player] = 0;
            displayTotalAndOnTable();
            refreshRaise();
        }
        public void win(int player)
        {
            int sum = 0, i, takeAmount;
            if (allInDone[player] != 1)
            {
                sum = total();
                initialMoney[player] += sum;
            }
            else
            {
                takeAmount = onTable[player];
                for (i = 0; i < numberOfPlayers; i++)
                {
                    if (onTable[i] > 0)
                    {
                        if (onTable[i] > takeAmount)
                        {
                            onTable[i] -= takeAmount;
                            initialMoney[i] += onTable[i];
                            sum += takeAmount;
                        }
                        else
                        {
                            sum += onTable[i];
                            onTable[i] = 0;
                        }
                    }
                }
                initialMoney[player] += sum;
            }
            refresh();
        }
        public void fold(int player)
        {
            folded[player] = 1;
        }
        public void draw()
        {
            char[] ch = textBox34.Text.ToString().ToCharArray();
            int count = ch.Length;
            int[] players = new int[count];
            int i;

            try
            {
                for (i = 0; i < count; i++)
                {
                    players[i] = int.Parse(ch[i].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            if (count == 0)
            {
                return;
            }
            int sum = total();
            sum /= count;
            for (i = 0; i < count; i++)
            {
                initialMoney[players[i] - 1] += sum;
            }
            refresh();
        }
        public void allInDraw()
        {
            int j, temp1, temp2, temp3;
            char[] ch1 = textBox36.Text.ToString().ToCharArray();
            char[] ch2 = textBox37.Text.ToString().ToCharArray();
            int LEN1 = ch1.Length;
            int LEN2 = ch2.Length;
            int LEN = LEN1;
            int count = LEN;
            int[] Players = new int[LEN1];
            int[] Priority = new int[LEN2];
            if (LEN1 != LEN2)
            {
                MessageBox.Show("Syntactically Wrong input");
                return;
            }
            int i;
            try
            {
                for (i = 0; i < LEN; i++)
                {
                    Players[i] = int.Parse(ch1[i].ToString());
                    Players[i] -= 1;
                    Priority[i] = int.Parse(ch2[i].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Syntactically Wrong input");
                return;
            }
            int[] onBet = new int[count];
            for (i = 0; i < count; i++)
            {
                onBet[i] = onTable[Players[i]];
            }
            for (i = 0; i < count - 1; i++)
            {
                for (j = 0; j < count - i - 1; j++)
                {
                    if (onBet[j] > onBet[j + 1])
                    {
                        temp1 = onBet[j];
                        temp2 = Players[j];
                        temp3 = Priority[j];
                        onBet[j] = onBet[j + 1];
                        Players[j] = Players[j + 1];
                        Priority[j] = Priority[j + 1];
                        onBet[j + 1] = temp1;
                        Players[j + 1] = temp2;
                        Priority[j + 1] = temp3;
                    }
                }
            }
            //sorted to create some data about data, to know who wins or grabs the main pot

            // implemented logic of side pots (but this is a draw format so count will be increased till priority is same plus onBet is same)
            int start, end, pot, k, sum, sumPot = 0;
            start = 0;
            end = 0;
            pot = 0;
            sum = 0;
            count = 1;
            for (i = 0; i < numberOfPlayers; i++)
            {
                if (folded[i] == 1)
                {
                    pot += onTable[i];
                    onTable[i] = 0;
                }
            }
            for (i = start; i < LEN; i++)
            {
                end = i;
                if (i < LEN - 1)
                {
                    if (onBet[i] == onBet[i + 1])
                    {
                        count += 1;
                        continue;
                    }
                }
                for (j = 0; j < numberOfPlayers; j++)
                {
                    int temp;
                    if (onTable[j] - onBet[i] >= 0)
                    {
                        onTable[j] = onTable[j] - onBet[i];
                        temp = onBet[i];
                    }
                    else
                    {
                        temp = onTable[j];
                        onTable[j] = 0;
                    }
                    sum += temp;
                }
                for (j = end + 1; j < LEN; j++)
                {
                    onBet[j] -= onBet[i];
                }
                pot += sum;
                k = end + 1;

                int prio = Priority[end];
                while (k < LEN)
                {
                    if (Priority[k] == prio)
                    {
                        count += 1;
                    }
                    k++;
                }
                //MANY CHECKS ARE THERE - FOR ARRAY OUT OF BOUNDS EXCEPTION
                sumPot = 0;
                for (k = start; k <= end; k++)
                {
                    initialMoney[Players[k]] += (pot / count);
                    sumPot += (pot / count);
                }
                k = end + 1;
                if (k < LEN)
                {
                    for (k = end + 1; Priority[k] == prio; k++)
                    {
                        if (k < LEN)
                        {
                            initialMoney[Players[k]] += (pot / count);
                            sumPot += (pot / count);
                        }
                        if (k + 1 == LEN)
                            break;
                    }
                }
                pot -= sumPot;
                start = end + 1;
                sum = 0;
                count = 1;
                if (pot != 0)
                { MessageBox.Show("Money lost in this BET = " + pot.ToString()); }
                pot = 0;  // as money must also go to same priority with more money on bet
            }
            for (i = 0; i < numberOfPlayers; i++)
            {
                initialMoney[i] += onTable[i];
            }
            refresh();
        }
        public void allInWinners()
        {
            int j, temp1, temp2;
            char[] ch = textBox35.Text.ToString().ToCharArray();
            int count = ch.Length;
            int LEN = count;
            int[] Players = new int[count];
            int i;
            try
            {
                for (i = 0; i < count; i++)
                {
                    Players[i] = int.Parse(ch[i].ToString());
                    Players[i] -= 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            int[] onBet = new int[count];
            for (i = 0; i < count; i++)
            {
                onBet[i] = onTable[Players[i]];
            }
            for (i = 0; i < count - 1; i++)
            {
                for (j = 0; j < count - i - 1; j++)
                {
                    if (onBet[j] > onBet[j + 1])
                    {
                        temp1 = onBet[j];
                        temp2 = Players[j];
                        onBet[j] = onBet[j + 1];
                        Players[j] = Players[j + 1];
                        onBet[j + 1] = temp1;
                        Players[j + 1] = temp2;
                    }
                }
            } //sorted to create some data about data, to know who wins or grabs the main pot

            // implemented logic of side pots
            int start, end, pot, k, sum;
            start = 0;
            end = 0;
            pot = 0;
            sum = 0;
            count = 1;
            for (i = 0; i < numberOfPlayers; i++)
            {
                if (folded[i] == 1)
                {
                    pot += onTable[i];
                    onTable[i] = 0;
                }
            }
            for (i = start; i < LEN; i++)
            {
                end = i;
                if (i < LEN - 1)
                {
                    if (onBet[i] == onBet[i + 1])
                    {
                        count += 1;
                        continue;
                    }
                }
                for (j = 0; j < numberOfPlayers; j++)
                {
                    int temp;
                    if (onTable[j] - onBet[i] >= 0)
                    {
                        onTable[j] = onTable[j] - onBet[i];
                        temp = onBet[i];
                    }
                    else
                    {
                        temp = onTable[j];
                        onTable[j] = 0;
                    }
                    sum += temp;
                }
                for (j = end + 1; j < LEN; j++)
                {
                    onBet[j] -= onBet[i];
                }
                pot += sum;

                for (k = start; k <= end; k++)
                {
                    initialMoney[Players[k]] += (pot / count);
                }
                start = end + 1;
                sum = 0;
                count = 1;
                pot = 0;
            }
            for (i = 0; i < numberOfPlayers; i++)
            {
                initialMoney[i] += onTable[i];
            }
            refresh();
        }
        public void refreshRaise()
        {
            textBox8.Text = smallBlind.ToString();
            textBox7.Text = smallBlind.ToString();
            textBox6.Text = smallBlind.ToString();
            textBox5.Text = smallBlind.ToString();
            textBox4.Text = smallBlind.ToString();
            textBox3.Text = smallBlind.ToString();
            textBox2.Text = smallBlind.ToString();
            textBox1.Text = smallBlind.ToString();
        }
        public void displayLoadTotalMoney()
        {
            textBox9.Text = initialMoney[7].ToString();
            textBox10.Text = initialMoney[6].ToString();
            textBox11.Text = initialMoney[5].ToString();
            textBox12.Text = initialMoney[4].ToString();
            textBox13.Text = initialMoney[3].ToString();
            textBox14.Text = initialMoney[2].ToString();
            textBox15.Text = initialMoney[1].ToString();
            textBox16.Text = initialMoney[0].ToString();
        }
        public void displayOnTable()
        {
            textBox33.Text = onTable[0].ToString();
            textBox32.Text = onTable[1].ToString();
            textBox31.Text = onTable[2].ToString();
            textBox30.Text = onTable[3].ToString();
            textBox29.Text = onTable[4].ToString();
            textBox28.Text = onTable[5].ToString();
            textBox27.Text = onTable[6].ToString();
            textBox26.Text = onTable[7].ToString();
        }
        public void displayTotalAndOnTable()
        {
            displayOnTable();
            displayLoadTotalMoney();
        }

        public void refresh()
        {
            smallBlindDone = 0;
            bigBlindDone = 0;
            
            for (int i = 0; i < numberOfPlayers; i++)
            { 
                onTable[i] = 0; 
                allInDone[i] = 0;
                folded[i] = 0;
            }                               //reset some variables
            
            displayLoadTotalMoney();
            displayOnTable();
            refreshRaise();
            textBox34.Text = "";            //clear text box for draw
            textBox35.Text = "";            //clear text box for win
            textBox36.Text = "";            //clear text box for WIN
            textBox37.Text = "";            //clear text box for PRIORITY
            button1.Enabled = true;
            button3.Enabled = true;
            button33.Enabled = true;
            button6.Enabled = true;
            button4.Enabled = true;
            button32.Enabled = true;
            button9.Enabled = true;
            button7.Enabled = true;
            button31.Enabled = true;
            button12.Enabled = true;
            button10.Enabled = true;
            button30.Enabled = true;
            button15.Enabled = true;
            button13.Enabled = true;
            button29.Enabled = true;
            button18.Enabled = true;
            button16.Enabled = true;
            button28.Enabled = true;
            button21.Enabled = true;
            button19.Enabled = true;
            button27.Enabled = true;
            button24.Enabled = true;
            button22.Enabled = true;
            button26.Enabled = true;
            button36.Enabled = true;
            button37.Enabled = true;
            button38.Enabled = true;
            button39.Enabled = true;
            button40.Enabled = true;
            button41.Enabled = true;
            button42.Enabled = true;
            button43.Enabled = true;
        }
        public Form1()
        {
            InitializeComponent();
        }
        private void button25_Click(object sender, EventArgs e)
        {
            int i;
            int[] money = new int[numberOfPlayers];
            try
            {
                money[7] = int.Parse(textBox9.Text);
                money[6] = int.Parse(textBox10.Text);
                money[5] = int.Parse(textBox11.Text);
                money[4] = int.Parse(textBox12.Text);
                money[3] = int.Parse(textBox13.Text);
                money[2] = int.Parse(textBox14.Text);
                money[1] = int.Parse(textBox15.Text);
                money[0] = int.Parse(textBox16.Text);
                smallBlind = int.Parse(textBox25.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fill Money and Small Blind with some numbers to proceed.");
                return;
            }
            for (i = 0; i < numberOfPlayers; i++)
            {
                initialMoney[i] = money[i];
            }
            refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (smallBlindDone == 0)
                blind(0, 1);
            else if (bigBlindDone == 0)
                blind(0, 2);
            else
                check(0);
            displayTotalAndOnTable();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (smallBlindDone == 0)
                blind(1, 1);
            else if (bigBlindDone == 0)
                blind(1, 2);
            else
                check(1);
            displayTotalAndOnTable();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (smallBlindDone == 0)
                blind(2, 1);
            else if (bigBlindDone == 0)
                blind(2, 2);
            else
                check(2);
            displayTotalAndOnTable();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (smallBlindDone == 0)
                blind(3, 1);
            else if (bigBlindDone == 0)
                blind(3, 2);
            else
                check(3);
            displayTotalAndOnTable();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (smallBlindDone == 0)
                blind(4, 1);
            else if (bigBlindDone == 0)
                blind(4, 2);
            else
                check(4);
            displayTotalAndOnTable();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (smallBlindDone == 0)
                blind(5, 1);
            else if (bigBlindDone == 0)
                blind(5, 2);
            else
                check(5);
            displayTotalAndOnTable();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (smallBlindDone == 0)
                blind(6, 1);
            else if (bigBlindDone == 0)
                blind(6, 2);
            else
                check(6);
            displayTotalAndOnTable();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (smallBlindDone == 0)
                blind(7, 1);
            else if (bigBlindDone == 0)
                blind(7, 2);
            else
                check(7);
            displayTotalAndOnTable();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int amount;
            try
            {
                amount = int.Parse(textBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            raise(0, amount);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int amount;
            try
            {
                amount = int.Parse(textBox2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            raise(1, amount);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int amount;
            try
            {
                amount = int.Parse(textBox3.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            raise(2, amount);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int amount;
            try
            {
                amount = int.Parse(textBox4.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            raise(3, amount);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            int amount;
            try
            {
                amount = int.Parse(textBox5.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            raise(4, amount);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            int amount;
            try
            {
                amount = int.Parse(textBox6.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            raise(5, amount);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            int amount;
            try
            {
                amount = int.Parse(textBox7.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            raise(6, amount);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            int amount;
            try
            {
                amount = int.Parse(textBox8.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            raise(7, amount);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            win(0);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            win(1);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            win(2);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            win(3);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            win(4);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            win(5);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            win(6);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            win(7);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fold(0);
            button1.Enabled = false;
            button3.Enabled = false;
            button33.Enabled = false;
            button36.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            fold(1);
            button6.Enabled = false;
            button4.Enabled = false;
            button32.Enabled = false;
            button37.Enabled = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            fold(2);
            button9.Enabled = false;
            button7.Enabled = false;
            button31.Enabled = false;
            button38.Enabled = false;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            fold(3);
            button12.Enabled = false;
            button10.Enabled = false;
            button30.Enabled = false;
            button39.Enabled = false;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            fold(4);
            button15.Enabled = false;
            button13.Enabled = false;
            button29.Enabled = false;
            button40.Enabled = false;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            fold(5);
            button18.Enabled = false;
            button16.Enabled = false;
            button28.Enabled = false;
            button41.Enabled = false;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            fold(6);
            button21.Enabled = false;
            button19.Enabled = false;
            button27.Enabled = false;
            button42.Enabled = false;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            fold(7);
            button24.Enabled = false;
            button22.Enabled = false;
            button26.Enabled = false;
            button43.Enabled = false;
        }

        private void button34_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private void button35_Click(object sender, EventArgs e)
        {
            try
            {
                draw();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Wrong input.");
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            allIn(0);
        }

        private void button37_Click(object sender, EventArgs e)
        {
            allIn(1);
        }

        private void button38_Click(object sender, EventArgs e)
        {
            allIn(2);
        }

        private void button39_Click(object sender, EventArgs e)
        {
            allIn(3);
        }

        private void button40_Click(object sender, EventArgs e)
        {
            allIn(4);
        }

        private void button41_Click(object sender, EventArgs e)
        {
            allIn(5);
        }

        private void button42_Click(object sender, EventArgs e)
        {
            allIn(6);
        }

        private void button43_Click(object sender, EventArgs e)
        {
            allIn(7);
        }

        private void button44_Click(object sender, EventArgs e)
        {
            try
            {
                allInWinners();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wrong input.");
            }
        }

        private void button45_Click(object sender, EventArgs e)
        {
            try
            {
                allInDraw();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wrong input.");
            }
        }

        private void pokerHandsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 ob = new Form2();
            ob.Show();
        }
    }
}
