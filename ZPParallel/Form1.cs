using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Numerics;

namespace ZPParallel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            long[] V;
            double[] S;
            long N;
            ZEROPROOF.GenerateKeys(out V, out S, out N);
            var Tmpstr = "";
            foreach (var a in V)
                Tmpstr += a.ToString() + " ";
            textBoxAN.Text = Convert.ToString(N);
            textBoxAV.Text = Convert.ToString(Tmpstr);
            textBoxBN.Text = Convert.ToString(N);
            textBoxBV.Text = Convert.ToString(Tmpstr);
            Tmpstr = "";
            foreach (var a in S)
                Tmpstr += a.ToString() + " ";
            textBoxAS.Text = Convert.ToString(Tmpstr);
        }

        private void buttonIdentif_Click(object sender, EventArgs e)
        {
            textBoxLog.Clear();
            textBoxLog.AppendText("=====================" + Environment.NewLine);
            textBoxLog.AppendText("Начало проверки" + Environment.NewLine);
            textBoxLog.AppendText("=====================" + Environment.NewLine);
            var Rnd = new Random();
            var Proof = true;

            var AN = Convert.ToInt64(textBoxAN.Text);
            var TmpStr = textBoxAS.Text.Split(' ');
            var AS = new List<int>();
            foreach(var s in TmpStr)
            {
                if(s != "")
                    AS.Add(Convert.ToInt32(s));
            }
            var BN = Convert.ToInt64(textBoxBN.Text);
            TmpStr = textBoxBV.Text.Split(' ');
            var BV = new List<long>();
            foreach (var s in TmpStr)
            {
                if (s != "")
                    BV.Add(Convert.ToInt32(s));
            }

            var NeBudetPovtornoR = new HashSet<int>();
                textBoxLog.AppendText("---------------------" + Environment.NewLine);
                textBoxLog.AppendText("Сторона А(доказывает)" + Environment.NewLine);
                textBoxLog.AppendText("---------------------" + Environment.NewLine);
                var AXMass = new List<long>();
                var ARMass = new List<long>();
                for (var i = 0; i < AS.Count; i++)
                {
                    var AR = 0;
                    do
                    {
                        AR = Rnd.Next(1, (int)AN);
                    }
                    while (NeBudetPovtornoR.Contains(AR));
                    NeBudetPovtornoR.Add(AR);
                    textBoxLog.AppendText("Случайное R = " + Convert.ToString(AR) + Environment.NewLine);
                    var AX = ZEROPROOF.FastPowFunc(AR, 2, AN);
                    AXMass.Add(AX);
                    ARMass.Add(AR);
                    textBoxLog.AppendText("X = " + Convert.ToString(AX) + Environment.NewLine);

                }
                textBoxLog.AppendText("Отправка X стороне B" + Environment.NewLine);

                //---------------------------------------------

                textBoxLog.AppendText("---------------------" + Environment.NewLine);
                textBoxLog.AppendText("Сторона B(проверяет)" + Environment.NewLine);
                textBoxLog.AppendText("---------------------" + Environment.NewLine);

                textBoxLog.AppendText("Получение X от стороны A" + Environment.NewLine);
                var BX = AXMass;
                foreach(var X in BX)
                    textBoxLog.AppendText("X = " + Convert.ToString(X) + Environment.NewLine);
                var BbArr = new List<int>();
                for (var i = 0; i < AS.Count; i++)
                {
                    var Bb = Rnd.Next(0, 2);
                    BbArr.Add(Bb);
                    textBoxLog.AppendText("Случайный бит b = " + Convert.ToString(Bb) + Environment.NewLine);
                }
                textBoxLog.AppendText("Отправка b стороне A" + Environment.NewLine);

                //---------------------------------------------

                textBoxLog.AppendText("---------------------" + Environment.NewLine);
                textBoxLog.AppendText("Сторона А(доказывает)" + Environment.NewLine);
                textBoxLog.AppendText("---------------------" + Environment.NewLine);

                textBoxLog.AppendText("Получение b от стороны B" + Environment.NewLine);
                var Ab = BbArr;
                foreach (var B in Ab)
                    textBoxLog.AppendText("b = " + Convert.ToString(B) + Environment.NewLine);
                var Otpravka = new List<long>();
                for (var i = 0; i < AS.Count; i++)
                {
                    long AY = 0;
                    if (Ab[i] == 0)
                    {
                        textBoxLog.AppendText("Отправка R стороне B" + Environment.NewLine);
                        Otpravka.Add(ARMass[i]);
                    }
                    else
                    {
                        AY = ARMass[i] * AS[i] % AN;
                        textBoxLog.AppendText("Y = " + Convert.ToString(AY) + Environment.NewLine);
                        textBoxLog.AppendText("Отправка Y стороне B" + Environment.NewLine);
                        Otpravka.Add(AY);
                    }
                }
                //---------------------------------------------

                textBoxLog.AppendText("---------------------" + Environment.NewLine);
                textBoxLog.AppendText("Сторона B(проверяет)" + Environment.NewLine);
                textBoxLog.AppendText("---------------------" + Environment.NewLine);
                for (var i = 0; i < AS.Count; i++)
                {
                    if (BbArr[i] == 0)
                    {
                        textBoxLog.AppendText("Получение R от стороны A" + Environment.NewLine);
                        var BR = Otpravka[i];
                        textBoxLog.AppendText("R = " + Convert.ToString(BR) + Environment.NewLine);
                        textBoxLog.AppendText("Вычисленный X = " + Convert.ToString(ZEROPROOF.FastPowFunc(BR, 2, BN)) + Environment.NewLine);
                        textBoxLog.AppendText("Полученный X = " + Convert.ToString(BX) + Environment.NewLine);
                        if (BX[i] == ZEROPROOF.FastPowFunc(BR, 2, BN))
                            textBoxLog.AppendText("Сторона A знает sqrt(X)" + Environment.NewLine);
                        else
                        {
                            textBoxLog.AppendText("Сторона A не знает sqrt(X). Сторона A не является подлинной!" + Environment.NewLine);
                            Proof = false;
                            break;
                        }
                    }
                    else
                    {
                        textBoxLog.AppendText("Получение Y от стороны A" + Environment.NewLine);
                        var BY = Otpravka[i];
                        textBoxLog.AppendText("Y = " + Convert.ToString(BY) + Environment.NewLine);
                        textBoxLog.AppendText("Вычисленный X = " + (BigInteger.Pow(BY, 2) * BV[i] % BN).ToString() + Environment.NewLine);
                        textBoxLog.AppendText("Полученный X = " + Convert.ToString(BX) + Environment.NewLine);
                        if (BX[i] == Convert.ToInt64((BigInteger.Pow(BY, 2) * BV[i] % BN).ToString()))
                            textBoxLog.AppendText("Сторона A знает sqrt(V-1)" + Environment.NewLine);
                        else
                        {
                            textBoxLog.AppendText("Сторона A не знает sqrt(V-1). Сторона A не является подлинной!" + Environment.NewLine);
                            Proof = false;
                            break;
                        }
                    }
                    if (!Proof)
                        break;
                }
            if (Proof)
                textBoxLog.AppendText("Сторона A идентифицирована!" + Environment.NewLine);
            else
                textBoxLog.AppendText("Сторона A не идентифицирована!" + Environment.NewLine);
        }
    }

    static public class ZEROPROOF
    {

        private static List<int> ReshetoEratosfena(out int P, out int Q)
        {

            var IntArr = new List<int>();
            for (var i = 2; i <= 1000; i++)
                IntArr.Add(i);

            for (var i = 0; i < IntArr.Count; i++)
            {
                for (var j = i; j < IntArr.Count; j++)
                {
                    if ((IntArr[i] != IntArr[j]) && (IntArr[j] % IntArr[i] == 0))
                    {
                        IntArr.RemoveAt(j);
                        j--;
                    }
                }
            }

            var ResList = new List<int>(IntArr);

            for (var i = 0; i < IntArr.Count;)
                if (IntArr[i] < 100)
                    IntArr.Remove(IntArr[i]);
                else
                    break;

            P = IntArr[new Random().Next(0, IntArr.Count - 1)];
            do
            {
                Q = IntArr[new Random().Next(0, IntArr.Count - 1)];
            }
            while (Q == P);

            return ResList;
        }

        private static bool EuclidIsNOD1(long a, long b)
        {

            while (a != 0 && b != 0)
            {
                if (a > b)
                    a -= b;
                else
                    b -= a;
            }
            if (a == 1 || b == 1)
                return true;
            else
                return false;
        }

        static public Int64 FastPowFunc(Int64 Number, Int64 Pow, Int64 Mod)
        {
            Int64 Result = 1;
            Int64 Bit = Number % Mod;

            while (Pow > 0)
            {
                if ((Pow & 1) == 1)
                {
                    Result *= Bit;
                    Result %= Mod;
                }
                Bit *= Bit;
                Bit %= Mod;
                Pow >>= 1;
            }
            return Result;
        }

        static public void GenerateKeys(out long[] V, out double[] S, out long N)
        {
            int P, Q;
            SortedSet<long> Vi;
            do
            {
                ReshetoEratosfena(out P, out Q);
                N = P * Q;

                Vi = new SortedSet<long>();
                long X = 1;
                do
                {
                    X++;
                    if (EuclidIsNOD1((long)Math.Pow(X, 2) % N, N))
                        Vi.Add((long)Math.Pow(X, 2) % N);
                }
                while (X < N);
            }
            while (Vi.Count == 1);
            if (Vi.Count > 10000)
            {
                var ArrTmp = Vi.ToArray();
                Vi = new SortedSet<long>();
                for (var i = 1; i < 10000; i++)
                    Vi.Add(ArrTmp[i]);
            }
            var VArr = new List<long>();
            var Vminus1 = new List<long>();
            long Tmp;
            var Rnd = new Random();
            var M = (P - 1) * (Q - 1);

            for (var i = 0; i < Vi.Count; i++)
            {
                var VTmp = Vi.ToArray()[i];
                var Vminus1Tmp = FastPowFunc(VTmp, M - 1, N);
                if (long.TryParse(Convert.ToString(Math.Sqrt(Vminus1Tmp)), out Tmp))
                {
                    VArr.Add(VTmp);
                    Vminus1.Add(Vminus1Tmp);
                }
            }

            S = new double[Vminus1.Count];

            var j = 0;
            foreach (var Vm1 in Vminus1)
            {
                S[j] = Math.Sqrt(Vm1) % N;
                j++;
            }

            V = VArr.ToArray();

        }

    }
}
