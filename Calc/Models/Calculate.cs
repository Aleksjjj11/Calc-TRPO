﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.Interfaces;

namespace Calc.Models
{
    public class Calculate : ICalculate
    {
        public Expression Parse(string expression)
        {
            try
            {
                expression = expression.Replace(" ", "").Replace(".", ",");
                #region ParseBrackets
                //If count open bracket not equal count close bracket we wil throw exception
                if (expression.Count(x => x == '(') != expression.Count(x => x == ')'))
                {
                    throw new Exception("Count '(' not equal ')'.");
                }
                if (expression.Count(x => x == '(') > 0)
                {
                    int j = 0;
                    int indexOpenBracket = -1, indexCloseBracket = -1;
                    char? symbol = expression[j];

                    while (j < expression.Length)
                    {
                        if (symbol == '(')
                            indexOpenBracket = j;
                        if (symbol == ')')
                        {
                            indexCloseBracket = j;
                            break;
                        }

                        j++;
                        symbol = expression[j];
                    }
                    if (indexOpenBracket > indexCloseBracket)
                        throw new Exception("Close bracket before then open bracket");
                    var resInBracket = Parse(expression.Substring(indexOpenBracket + 1, indexCloseBracket - indexOpenBracket - 1));
                    expression = expression.Remove(indexOpenBracket, indexCloseBracket - indexOpenBracket + 1)
                        .Insert(indexOpenBracket, resInBracket.Result.ToString());
                    return Parse(expression);
                }
                #endregion
                string leftValue = "", rightValue = "";
                int i = 0, lessIndexOperation = -1;
                char? operation = null;
                if (expression.Contains("*") && expression.Contains("/"))
                {
                    lessIndexOperation = Math.Min(expression.IndexOf("*"), expression.IndexOf("/"));
                } 
                else if (expression.Contains("*"))
                {
                    lessIndexOperation = expression.IndexOf("*");
                }
                else if (expression.Contains("/"))
                {
                    lessIndexOperation = expression.IndexOf("/");
                } 
                else if (expression.Contains("+") && expression.Contains("-"))
                {
                    lessIndexOperation = expression.IndexOf("+");//Math.Min(expression.IndexOf("+"), expression.LastIndexOf("-"));
                }
                else if (expression.Contains("+"))
                {
                    lessIndexOperation = expression.IndexOf("+");
                }
                else if (expression.Contains("-"))
                {
                    lessIndexOperation = expression.LastIndexOf("-");
                    if (lessIndexOperation == 0)
                        return new Expression(Convert.ToDouble(expression));//Convert.ToDouble(expression);
                }
                
                if (lessIndexOperation == -1)
                    return new Expression(Convert.ToDouble(expression));

                leftValue = FindLeftValue(expression, lessIndexOperation);
                rightValue = FindRightValue(expression, lessIndexOperation);
                operation = expression[lessIndexOperation];
                    
                if (rightValue == "" || operation is null)
                    return new Expression(Convert.ToDouble(expression));

                int removeIndex = lessIndexOperation - leftValue.Length;
                expression = expression.Remove(removeIndex, leftValue.Length + rightValue.Length + 1);
                double result;
                
                switch (operation)
                {
                    case '-':
                        {
                            result = Convert.ToDouble(leftValue) - Convert.ToDouble(rightValue);
                            break;
                        }
                    case '+':
                        {
                            result = Convert.ToDouble(leftValue) + Convert.ToDouble(rightValue);
                            break;
                        }
                    case '*':
                        {
                            result = Convert.ToDouble(leftValue) * Convert.ToDouble(rightValue);
                            break;
                        }
                    case '/':
                        {
                            result = Convert.ToDouble(leftValue) / Convert.ToDouble(rightValue);
                            break;
                        }
                    default: throw new Exception("We have problem");
                }

                return Parse(expression.Insert(removeIndex, result.ToString()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Expression(0, e.Message);
            }
        }
        private static string FindLeftValue(string expression, int indexOperation)
        {
            string res = "";

            for (int i = indexOperation - 1; i >= 0; i--)
            {
                char symbol = expression[i];
                if (symbol >= '0' && symbol <= '9' || symbol == ',')
                    res += symbol;
                else if (symbol == '-')
                {
                    res += symbol;
                    break;
                }
                else break;
            }
            
            return new string(res.ToCharArray().Reverse().ToArray());
        }

        private static string FindRightValue(string expression, int indexOperation)
        {
            string res = "";

            for (int i = indexOperation + 1; i < expression.Length; i++)
            {
                char symbol = expression[i];
                if (symbol >= '0' && symbol <= '9' || symbol == ',')
                    res += symbol;
                else break;
            }
            
            return res;
        }
    }
}
