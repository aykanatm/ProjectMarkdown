using System.Collections.Generic;
using System.Collections.ObjectModel;
using FastColoredTextBoxNS;

namespace ProjectMarkdown.ExtensionMethods
{
    public static class ObservableCollectionExtensionMethods
    {
        public static LimitedStack<T> ToLimitedStack<T>(this ObservableCollection<T> input, int limit) where T: class
        {
            var limitedStack = new LimitedStack<T>(limit);

            if (input.Count > 0)
            {
                for (var i = input.Count - 1; i >= 0; i--)
                {
                    limitedStack.Push(input[i]);
                }
            }

            return limitedStack;
        }

        public static Stack<T> ToStack<T>(this ObservableCollection<T> input) where T : class
        {
            var stack = new Stack<T>();

            if (input.Count > 0)
            {
                for (var i = input.Count - 1; i >= 0; i--)
                {
                    stack.Push(input[i]);
                }
            }

            return stack;
        }
    }
}
