using System.Collections.Generic;

namespace StacksAndQueues
{
    public class MyQueue
    {
        private Stack<int> inStack;
        private Stack<int> outStack;

        public MyQueue()
        {
            inStack = new Stack<int>();
            outStack = new Stack<int>();
        }
        
        public void Push(int x)
        {
            inStack.Push(x);
        }
        
        public int Pop()
        {
            if (outStack.Count == 0)
            {
                while (inStack.Count > 0)
                {
                    outStack.Push(inStack.Pop());
                }
            }
            return outStack.Pop();
        }
        
        public int Peek()
        {
            if (outStack.Count == 0)
            {
                while (inStack.Count > 0)
                {
                    outStack.Push(inStack.Pop());
                }
            }
            return outStack.Peek();
        }
        
        public bool Empty()
        {
            return inStack.Count == 0 && outStack.Count == 0;
        }
    }
}