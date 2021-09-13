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
        
        static void Main(string[] args)
        {
            string pathIn = args[0]; //Full path to file >> C:\XXX\Sample.In
            string pathOut = args[1]; //Full path to file >> C:\XXX\Sample.Out

            List<string> output = new List<string>();
            List<string> input=FileReader.Read(pathIn);            

            foreach (var line in input)
            {
                var func = line.Split(' ').FirstOrDefault();
                switch (func)
                {                
                    case "def" : MyDictionary.DefHandler(line); break;                
                    case "calc": output.Add(MyCalculator.CalcHandler(line)); break;
                    case "clear": MyDictionary.ClearTable(); break;
                    default:break;
                } 
            }

            FileReader.Write(pathOut,output);
        } 

 
    }
    #region MyCalculator
    public static class MyCalculator
    {
        public static string CalcHandler(string calcLine)
        {
            List<string> calcLineItems = calcLine.Split(' ').ToList();
            int? Total = CalculateTotal(calcLineItems);
            return BuildOutputString(calcLine, Total);
        }

        private static string BuildOutputString(string calcLine, int? Total)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@$"{calcLine.Remove(0, 5)}");// Remove 'calc '
            if (Total != null)
                if (MyDictionary.ContainsValue((int)Total))
                { sb.Append(@$"{MyDictionary.GetKey((int)Total)}"); return sb.ToString(); }
            sb.Append(@$" unknown");
            return sb.ToString();
        }

        private static int? CalculateTotal(List<string> values)
        {
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
            catch (Exception ex)
            {
                return null;
            }
        }


    }
    #endregion
    #region MyDictionary
    public static class MyDictionary

    {
        public static Dictionary<string, int> VariableList = new Dictionary<string, int>();
        public static void Add(string s, int i) { if (!VariableList.Any(_ => _.Key == s)) VariableList.Add(s, i); else VariableList[s] = i; }
        public static bool ContainsValue(int i) => VariableList.Any(_ => _.Value == i);
        public static int? GetValue(string s) => (VariableList.Any(_ => _.Key == s)) ? VariableList.Single(_ => _.Key == s).Value : null;
        public static string GetKey(int i) => VariableList.Single(_ => _.Value == i).Key;
        public static void ClearTable() => VariableList = new Dictionary<string, int>();
        public static void DefHandler(string item)
        {
            string pattern = @"(def)\s(\w+)\s(-?(1000|[0-9][0-9][0-9]|[0-9][0-9]|[0-9])$)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = regex.Match(item);
            string alphabeticalValue = m.Groups[2].Value;
            int numericalValue = int.Parse(m.Groups[3].Value);

            VariableList.Add(alphabeticalValue, numericalValue);

        }
    }
    #endregion
    #region FileReader
    public static  class FileReader
    {
        public static List<string> Read(string PathIn) => File.ReadAllLines(PathIn).ToList(); 
        public static void Write(string PathOut,List<string> output) => File.WriteAllLines(PathOut, output);      
    }
    #endregion
}
