using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ProjectMarkdown.ExtensionMethods
{
    public static class StackExtensionMethods
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this Stack<T> input)
        {
            try
            {
                var observableCollection = new ObservableCollection<T>();

                while (input.Count > 0)
                {
                    observableCollection.Add(input.Pop());
                }

                return observableCollection;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
