using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ProjectMarkdown.ExtensionMethods
{
    public static class ListExtensionMethods
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this List<T> input)
        {
            var output = new ObservableCollection<T>();

            foreach (var inputElement in input)
            {
                output.Add(inputElement);
            }

            return output;
        }
    }
}
