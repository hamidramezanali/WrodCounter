using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WrodCounter
{
    class Program
    {
        static List<string> output = new List<string>();
        static void Main(string[] args)
        {
            string pat = @"(def)\s(\w+)\s(-?(1000|[0-9][0-9][0-9]|[0-9][0-9]|[0-9])$)";
            Regex regex = new Regex(pat, RegexOptions.IgnoreCase);       
            List<string> fileLines=FileReader.Read();
            

            foreach (var line in fileLines)
            {
                GetCategory(regex, line);

            }
            FileReader.Write(output);
        }

        private static void GetCategory(Regex regex, string line)
        {
            if (line.StartsWith("def"))
            {
                DefDetector(regex, line);
            }
            else if (line.StartsWith("calc"))
            {
                calcDetector(line);
            }
            else if (line.StartsWith("clear"))
            {
                MyDictionary.ClearTable();
            }
        }


        private static void calcDetector(string calcLine)
        {

            List<string> calcLineItems = calcLine.Split(' ').ToList();
            int? Total = CalculateTotal(calcLineItems);
            PrintTotal(calcLine, Total);
        }

        private static void PrintTotal(string calcLine, int? Total)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@$"{calcLine.Remove(0, 4)}");
            if (Total != null)
                if (MyDictionary.ContainsValue((int)Total))
                { sb.AppendLine(@$" {MyDictionary.GetKey((int)Total)}"); output.Add(sb.ToString()); return; }
            sb.AppendLine(@$" Unknown ");
            output.Add(sb.ToString());
        }

        private static int? CalculateTotal(List<string> values)
        {
            Dictionary<string, int> sta = MyDictionary.sta;
        int? numericEq;
            int i = 1;
            int? total = 0;
            int factor = 1;
            try
            {
                for (; i < values.Count;)
                {
                    string Operand = values[i];
                    string Operator = values[i + 1];
                    if (Operator == "+" || Operator == "-")
                    {
                        numericEq = MyDictionary.GetValue(Operand);
                        if (numericEq != null)
                        {
                            total = total + (factor) * numericEq;
                            if (Operator == "+") factor = 1; else factor = -1;
                        }
                        else { return null; }
                    }
                    else if (Operator == "=")
                    {
                        numericEq = MyDictionary.GetValue(Operand);
                        if (numericEq != null)
                        {
                            total = total + (factor) * numericEq;
                        }
                        else return null;
                        break;
                    }

                    i += 2;
                }
                return total;
            }
            catch(Exception ex)
            {
                return null;
            }
        }






    

        private static void DefDetector(Regex re, string item)
        {
            Match m = re.Match(item);

            if (!MyDictionary.ContainsKey(m.Groups[2].Value))
                MyDictionary.Add(m.Groups[2].Value, int.Parse(m.Groups[3].Value));
            else
            {
                MyDictionary.UpdateValue(m.Groups[2].Value , int.Parse(m.Groups[3].Value));

            }
        }
    }

    public static class MyDictionary
    {
        public static Dictionary<string, int> sta = new Dictionary<string, int>();
        public static void Add(string s, int i) => sta.Add(s, i);
        public static bool ContainsKey(string s) => sta.Any(_ => _.Key == s);
        public static bool ContainsValue(int i) => sta.Any(_ => _.Value == i);
        public static int? GetValue(string s) => (sta.Any(_ => _.Key == s)) ? sta.Single(_ => _.Key == s).Value : null;
        public static string GetKey(int i) => sta.Single(_ => _.Value == i).Key;
        public static void UpdateValue(string s,int i) => sta[s]=i;
        public static void ClearTable() => sta = new Dictionary<string, int>();
    }
    public static  class FileReader
    {
        static string Path = @"C:\Users\hamid\Downloads\sample.in";
        static string PathOut = @"C:\Users\hamid\Downloads\sample3.out";
        public static List<string> Read()
        {
            List<string> file = File.ReadAllLines(Path).ToList();
            return file;
    }
        public static void Write(List<string> output)
        {
            File.WriteAllLines(PathOut, output);
        }
    }
}
