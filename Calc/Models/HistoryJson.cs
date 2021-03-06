﻿using System.Collections.ObjectModel;
using System.IO;
using Calc.Interfaces;
using Newtonsoft.Json;

namespace Calc.Models
{
    public class HistoryJson : IHistory
    {
        private string _fileName;
        public HistoryJson(string fileName)
        {
            _fileName = fileName;
            if (File.Exists(fileName) == false)
            {
                HistoryCollection = new ObservableCollection<Expression>();
                return;
            }

            var fileText = File.ReadAllText(_fileName);
            HistoryCollection = JsonConvert.DeserializeObject<ObservableCollection<Expression>>(fileText);
        }

        public ObservableCollection<Expression> HistoryCollection { get; }
        public bool TryAdd()
        {
            return HistoryCollection is null == false;
        }

        public bool TryDelete(int index)
        {
            return index > -1 && index < HistoryCollection?.Count;
        }

        public bool TryClear()
        {
            return HistoryCollection?.Count > 0;
        }

        public void Add(Expression expression)
        {
            HistoryCollection.Insert(0, expression);
            Save();
        }

        public void Delete(int index)
        {
            HistoryCollection.RemoveAt(index);
            Save();
        }

        public void Clear()
        {
            HistoryCollection.Clear();
            Save();
        }

        public void Save()
        {
            var textToOutput = JsonConvert.SerializeObject(HistoryCollection);
            File.WriteAllText(_fileName, textToOutput);
        }
    }
}
