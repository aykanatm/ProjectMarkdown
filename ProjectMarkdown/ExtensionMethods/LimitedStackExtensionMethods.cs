using System;
using System.Collections.ObjectModel;
using FastColoredTextBoxNS;

namespace ProjectMarkdown.ExtensionMethods
{
    public static class LimitedStackExtensionMethods
    {
        public static ObservableCollection<T> ToOveObservableCollection<T>(this LimitedStack<T> input) where T : class
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
