using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzySortPlayground
{
    static class DataParser
    {
        
        static public List<ListItem> ParseListString(string textField)
        {
            textField = textField.Trim().Replace(',', ' ').Replace(';', ' ');

            string[] components = textField.Split(' ');
            List<ListItem> result = new List<ListItem>(components.Length);
            foreach (string component in components) {
                if (component != "") {
                    ListItem newItem = new ListItem();
                    newItem.TextValue = component;
                    result.Add(newItem);
                }
            }

            return result;
        }

        static public string ToString(List<ListItem> aList)
        {
            string result = "";
            int index = 0;
            foreach (ListItem item in aList) {
                result += item.TextValue;
                if (index < aList.Count)
                    result += " ";
                index++;
            }
            return result;
        }

        static public double ParseNumericInput(string str, double defaultValue)
        {
            double result = 0;
            if (!Double.TryParse(str, out result)) {
                result = defaultValue;
            }
            if (result < 0) {
                    int stop = 1;
            }
            return result;
        }

        static public double? ParseNullableNumericInput(string str)
        {
            double? result = 0;
            double testParse = 0;
            if (!Double.TryParse(str, out testParse)) {
                result = null;
            } else {
                result = testParse;
            }
            if (result < 0) {
                int stop = 1;
            }

            return result;
        }
    }
}
