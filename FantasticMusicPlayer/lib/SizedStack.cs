using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasticMusicPlayer.lib
{
    public class SizedStack<T>
    {

        private int stackSize = 30;
        private List<T> stackContent = new List<T>();

        public SizedStack(int stackSize)
        {
            this.stackSize = stackSize;
        }

        public void push(T obj) {
            stackContent.Add(obj);
            while (stackContent.Count > stackSize) { stackContent.RemoveAt(0); }
        }

        public bool pop(out T output) {
            if (stackContent.Count > 0) {
                output = stackContent.Last();
                stackContent.RemoveAt(stackContent.Count - 1);
                return true;
            }
            output = default(T);
            return false; 
        }

        internal void Clear()
        {
            stackContent.Clear();
        }
    }
}
