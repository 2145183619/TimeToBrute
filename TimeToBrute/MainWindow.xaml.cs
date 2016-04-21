using System;
using System.Collections.Generic;
using System.Windows;

using System.Numerics;

namespace TimeToBrute
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const ulong _KH = 1000;
        const ulong _MH = 1000 * _KH;

        bool isLowerLatin = false;
        bool isUpperLatin = false;
        bool isLowerRussian = false;
        bool isUpperRussian = false;
        bool isDigits = false;
        bool isSpecials = false;

        bool isUnknownSymbol = false;

        BigInteger totalCombinations = 0;
        //ulong totalCombinations = 0;



        List<Videocard> videocards = new List<Videocard>();

        public MainWindow()
        {
            InitializeComponent();

            videocards.Add(new Videocard() { Name = "AMD R9 280X", AlgoSpeeds = new List<AlgorithmAndSpeed>()});
            videocards[0].AlgoSpeeds.Add(new AlgorithmAndSpeed() { Name = "NTLM", Speed = 12300 * _MH });
            videocards[0].AlgoSpeeds.Add(new AlgorithmAndSpeed() { Name = "MD5", Speed = 7200 * _MH });            
            videocards[0].AlgoSpeeds.Add(new AlgorithmAndSpeed() { Name = "IPB2+, MyBB1.2+", Speed = 1455 * _MH });
            videocards[0].AlgoSpeeds.Add(new AlgorithmAndSpeed() { Name = "vBulletin > v3.8.5", Speed = 1400 * _MH });
            videocards[0].AlgoSpeeds.Add(new AlgorithmAndSpeed() { Name = "vBulletin < v3.8.5", Speed = 2000 * _MH });
            videocards[0].AlgoSpeeds.Add(new AlgorithmAndSpeed() { Name = "MySQL4.1 / MySQL5", Speed = 1160 * _MH });
            videocards[0].AlgoSpeeds.Add(new AlgorithmAndSpeed() { Name = "DES(Unix)", Speed = 87388 * _KH });
            videocards[0].AlgoSpeeds.Add(new AlgorithmAndSpeed() { Name = "MD5(Wordpress, phpBB3, Joomla)", Speed = 438 * _KH });
            videocards[0].AlgoSpeeds.Add(new AlgorithmAndSpeed() { Name = "WPA/WPA 2", Speed = 129 * _KH });
        }


        string GetTimePeriodFromSeconds(BigInteger seconds)
        {
            ulong minute = 60;
            ulong hour = 60 * minute;
            ulong day = 24 * hour;
            ulong month = 30 * day;
            ulong year = 12 * month;

            var secondsLeft = seconds;
            
            string result = "";

            if (seconds == 0)
                return "меньше секунды";

            if (secondsLeft / year != 0)
            {
                result += String.Format("лет: {0}, ", secondsLeft / year);
                secondsLeft = secondsLeft % year;
            }

            if (secondsLeft / month != 0)
            {
                result += String.Format("месяцев: {0}, ", secondsLeft / month);
                secondsLeft = secondsLeft % month;
            }

            if (secondsLeft / day != 0)
            {
                result += String.Format("дней: {0}, ", secondsLeft / day);
                secondsLeft = secondsLeft % day;
            }

            if (secondsLeft / hour != 0)
            {
                result += String.Format("часов: {0}, ", secondsLeft / hour);
                secondsLeft = secondsLeft % hour;
            }

            if (secondsLeft / minute != 0)
            {
                result += String.Format("минут: {0}, ", secondsLeft / minute);
                secondsLeft = secondsLeft % minute;
            }

            if (secondsLeft != 0)
            {
                result += String.Format("секунд: {0}", secondsLeft);
            }


            if (result[result.Length - 1] == ' ')               //избавляемся от ", ", если имеется вконце
                return result.Substring(0, result.Length - 2);
            else
                return result;
        }


        void CalculateTotalCombinations(int textLength)
        {
            ulong combinationsPerPosition = 0;
            totalCombinations = 0;

            if (isLowerLatin)
                combinationsPerPosition += 26;

            if (isUpperLatin)
                combinationsPerPosition += 26;

            if (isLowerRussian)
                combinationsPerPosition += 33;

            if (isUpperRussian)
                combinationsPerPosition += 33;

            if (isDigits)
                combinationsPerPosition += 10;

            if (isSpecials)
                combinationsPerPosition += 32;//!@#$%^&*()`~-_=+\|[]{};:'",.<>/?

            for(var i = 1; i <= textLength; i++)
                totalCombinations += (BigInteger)Math.Pow(combinationsPerPosition, i);
            //totalCombinations = (BigInteger)Math.Pow(combinationsPerPosition, textLength);
        }


        void SetupBools(string password)
        {
            isLowerLatin = false;
            isUpperLatin = false;
            isLowerRussian = false;
            isUpperRussian = false;
            isDigits = false;
            isSpecials = false;

            isUnknownSymbol = false;

            var specials = "!@#$%^&*()`~-_=+\\|[]{};:'\",.<>/? ";

            foreach (char ch in password)
            {
                if (Char.IsDigit(ch))
                    isDigits = true;
                else
                if (specials.Contains(ch.ToString()))
                    isSpecials = true;
                else
                if (ch >= 'A' && ch <= 'Z')
                    isUpperLatin = true;
                else
                if (ch >= 'a' && ch <= 'z')
                    isLowerLatin = true;
                else
                if (ch >= 'А' && ch <= 'Я' || ch == 'Ё')
                    isUpperRussian = true;
                else
                if (ch >= 'а' && ch <= 'я' || ch == 'ё')
                    isLowerRussian = true;
                else
                {
                    isUnknownSymbol = true;
                    break;
                }
            }
        }



        private void TextBox_Password_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (TextBox_Password.Text.Length != 0)
            {
                SetupBools(TextBox_Password.Text);

                if (isUnknownSymbol)
                {
                    TextBox_Results.Text = "Поддерживаются только: кириллица, латиница, цифры, спец. символы.";
                    return;
                }
                    

                CalculateTotalCombinations(TextBox_Password.Text.Length);

                TextBox_Results.Text = "";
                foreach(Videocard videocard in videocards)
                {
                    TextBox_Results.Text += String.Format("Для {0}:", videocard.Name) + "\n";

                    foreach(AlgorithmAndSpeed algo in videocard.AlgoSpeeds)
                    {
                        //var test = totalCombinations / algo.Speed;
                        if (totalCombinations == 0 && TextBox_Password.Text.Length > 5)
                            TextBox_Results.Text += String.Format("{0}: {1}", algo.Name, "вечность (=") + "\n";
                        else
                           TextBox_Results.Text += String.Format("{0}: {1}", algo.Name, GetTimePeriodFromSeconds(totalCombinations/algo.Speed)) + "\n";
                    }
                }
            }
            else
                TextBox_Results.Text = "";
        }



        public class AlgorithmAndSpeed
        {
            public string Name;
            public ulong Speed;
        }

        public class Videocard
        {
            public string Name;
            public List<AlgorithmAndSpeed> AlgoSpeeds;
        }


    }
}
