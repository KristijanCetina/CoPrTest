using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CoPrTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Open();
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Probaj zatvoriti COM port\n" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Write(CoPrCommand.restoreDefaultMachineParametar);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.Write(CoPrCommand.disableNumberPrintOfTheBarcode);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty (textBox1.Text) )
            {
                serialPort1.Write(CoPrCommand.defineBarcode);
            }
            else
            {
                string textString = CoPrCommand.defineBarcodeRazduzivac;
                textString += textBox1.Text + "\x0003";
                serialPort1.Write(textString);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                string textString = CoPrCommand.defineAString;
                textString += DateTime.Now.ToString();
                textString += "\x0003";

                listBox1.Items.Insert(0, textString);
                serialPort1.Write(textString.ToString());
            }
            else
            {
                string textString = CoPrCommand.defineAStringRazduzivac;
                textString += textBox1.Text + "\x0003";
                serialPort1.Write(textString);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            serialPort1.Write(CoPrCommand.movementWithPrint);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            serialPort1.Write(CoPrCommand.ejectTicket);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            serialPort1.Write(CoPrCommand.selectLargeImage);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            serialPort1.Write(CoPrCommand.softReset);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            serialPort1.Write(CoPrCommand.movementWithPrintFromMouth);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            serialPort1.Close();
            serialPort1.Dispose();
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            this.Invoke(new EventHandler(AddRecieve));
        }

        private void AddRecieve(object s, EventArgs e)
        {
            System.Threading.Thread.Sleep(50);
            int lenght = serialPort1.BytesToRead;
            byte[] b = new byte[lenght];
            serialPort1.Read(b, 0, lenght);

            parse2Message(b);
            parseMessage(b);
            serialPort1.DiscardInBuffer();
        }

        private void parse2Message(byte[] receivedMsg)
        {
            byte[] byteovi = new byte[] {};
            List<byte> parsaniByteovi = new List<byte> { };
            string poruka;
            int warning = 0;
            int error = 0;
            byte stx = 0x02;

            if (receivedMsg.First() != stx)
            {
                listBox1.Items.Insert(0, "Prvi byte je: " + receivedMsg.First().ToString());
                foreach (var item in receivedMsg)
                {
                    listBox1.Items.Insert(0, item);
                }
            }
            if (!receivedMsg.Contains(stx))
            {
                MessageBox.Show("Nema STX");
                return;
            }

            foreach (byte b in receivedMsg)
            {
                if (b != 0x02 && b!= 0x03 && b!= 0x0A && b!= 0x0D)
                    parsaniByteovi.Add(b);
            }
            byteovi = parsaniByteovi.ToArray();
            //foreach (byte b in byteovi)
            //{
            //    listBox1.Items.Insert(0, b.ToString());
            //}

            poruka = Encoding.ASCII.GetString(byteovi);
            listBox1.Items.Insert(0,"Trim2: "+ poruka);

            warning = Convert.ToInt32(poruka.Substring(poruka.Length - 5, 2));
            error = Convert.ToInt32(poruka.Substring(poruka.Length - 2, 2));

            listBox1.Items.Insert(0, "Warning: " + warning.ToString() + "Error: " + error.ToString());

            
        }

        private void parseMessage(byte[] receivedMsg)
        {
            try
            {
                byte stx = 0x02;
                if (!receivedMsg.Contains(stx))
                {
                    MessageBox.Show("Nema STX");
                    return;
                }
                string msg = System.Text.Encoding.ASCII.GetString(receivedMsg);
                string trimanaPoruka = msg.Trim((char)0x02, (char)0x03, (char)0x0A, (char)0x0D);
                listBox1.Items.Insert(0, "TRIMANA: " + trimanaPoruka);

                string cmd = trimanaPoruka.Substring(0, trimanaPoruka.Length - 6);
                int warning = 99;
                int error = 99;
                if (trimanaPoruka.Length >= 8)
                    warning = Convert.ToInt32(trimanaPoruka.Substring(trimanaPoruka.Length - 5, 2));
                if (trimanaPoruka.Length >= 4)
                    error = Convert.ToInt32(trimanaPoruka.Substring(trimanaPoruka.Length - 2, 2));

                if (warning == 0 && error == 0)
                {
                    listBox1.Items.Insert(0, cmd + " Operation successful");
                }

                if (cmd.StartsWith("1101161213122821032")) //za sada ovako ali treba napraviti parsiranje za dobar odgovor
                    serialPort1.Write(CoPrCommand.ejectTicket);





                switch (warning)
                {
                    case 0:
                        break;
                    case 3:
                        listBox1.Items.Insert(0, cmd + " No ticket there");
                        break;
                    case 5:
                        listBox1.Items.Insert(0, cmd + " The command does not exist");
                        break;
                    case 10:
                        listBox1.Items.Insert(0, cmd + " The machine arrives from reset");
                        break;
                    default:
                        listBox1.Items.Insert(0, cmd + " Nepoznati warning: " + warning.ToString());
                        break;
                }

                switch (error)
                {
                    case 0:
                        break;
                    case (int)Error.impossible:
                        listBox1.Items.Insert(0, cmd + " The command is impossible");
                        break;
                    case (int)Error.Fanfold:
                        listBox1.Items.Insert(0, cmd + " The Fan-fold is empty");
                        break;
                    case 42:
                        listBox1.Items.Insert(0, cmd + "Barcode reader error");
                        break;
                    default:
                        listBox1.Items.Insert(0, cmd + " Nepoznati error: " + error.ToString());
                        break;
                }
            }
            catch (Exception e)
            {
                listBox1.Items.Insert(0, e.ToString());
            }
        }

        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            serialPort1.Write("\x0002120\x0003");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            serialPort1.Write(CoPrCommand.selectMediumImage);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.Write(CoPrCommand.readBarcodePrintedByBCTIM);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            serialPort1.Write(CoPrCommand.readBarcodePrintedByBCTCM);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            serialPort1.Write(CoPrCommand.defineBarcodeAsCM);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            string textString = CoPrCommand.defineAStringAsValidator;
            textString += DateTime.Now.ToString();
            textString += "\x0003";

            listBox1.Items.Insert(0, textString);
            serialPort1.Write(textString.ToString());
        }

        private void button16_Click(object sender, EventArgs e)
        {
            serialPort1.Write(CoPrCommand.movementWithPrintRead);
            //System.Threading.Thread.Sleep(2800);
            //serialPort1.Write(CoPrCommand.ejectTicket);
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
    [Flags]public enum Error
    {
        impossible = 5,
        Fanfold = 22
    }
}
