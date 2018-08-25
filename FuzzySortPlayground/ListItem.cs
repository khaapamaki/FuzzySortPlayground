﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzySortLib;

namespace FuzzySortPlayground
{
    public class ListItem : IFuzzySortable
    {
        private double? sortValue;

        public double? SortValue {
            get { return sortValue; }
            set { sortValue = value; }
        }

        private string textValue;
        public string TextValue {
            get {
                if (sortValue != null)
                    return sortValue.ToString();
                else
                    return textValue;
            }
            set {
                textValue = value;
                sortValue = ConvertToSortValue(textValue);
            }
        }

        public double? ConvertToSortValue(string textField)
        {
            textField = textField.Trim();
            bool negative = textField.StartsWith("-");
            if (negative)
                textField = textField.Substring(1);
            double doubleValue;
            if (double.TryParse(textField, out doubleValue))
                return doubleValue * (negative ? -1 : 1);
            else
                return null;
        }
    }
}
