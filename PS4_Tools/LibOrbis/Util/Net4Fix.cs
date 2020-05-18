using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PS4_Tools.LibOrbis.Util
{
    /*****************************************************
     * 
     * This class will be used to recreate .net4+ functions
     * As we are downscaling to .net3.5 for Unity
     * (new Methods and functions will be added here)
     * Date         ||      Function        || Username
     * 2019-03-11           Tuple               XDPX 
     * 2019-03-14           Parallel            XDPX
     *****************************************************/

    public class Tuple<T1, T2>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        internal Tuple(T1 first, T2 second)
        {
            Item1 = first;
            Item2 = second;
        }
    }

    public static class Tuple
    {
        public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
        {
            var tuple = new Tuple<T1, T2>(first, second);
            return tuple;
        }
        public static Tuple<T1, T2> Create<T1, T2>(T1 first, T2 second)
        {
            var tuple = new Tuple<T1, T2>(first, second);
            return tuple;
        }
    }

    public class Parallel
    {
        public static int NumberOfParallelTasks;

        static Parallel()
        {
            NumberOfParallelTasks = Environment.ProcessorCount;
        }
        public static void ForEach<T>(IEnumerable<T> enumerable, Action<T> action)
        {
            var syncRoot = new object();

            if (enumerable == null) return;

            var enumerator = enumerable.GetEnumerator();

            InvokeAsync<T> del = InvokeAction;

            var seedItemArray = new T[NumberOfParallelTasks];
            var resultList = new List<IAsyncResult>(NumberOfParallelTasks);

            for (int i = 0; i < NumberOfParallelTasks; i++)
            {
                bool moveNext;

                lock (syncRoot)
                {
                    moveNext = enumerator.MoveNext();
                    seedItemArray[i] = enumerator.Current;
                }

                if (moveNext)
                {
                    var iAsyncResult = del.BeginInvoke
             (enumerator, action, seedItemArray[i], syncRoot, i, null, null);
                    resultList.Add(iAsyncResult);
                }
            }

            foreach (var iAsyncResult in resultList)
            {
                del.EndInvoke(iAsyncResult);
                iAsyncResult.AsyncWaitHandle.Close();
            }
        }

        delegate void InvokeAsync<T>(IEnumerator<T> enumerator,
        Action<T> achtion, T item, object syncRoot, int i);

        static void InvokeAction<T>(IEnumerator<T> enumerator, Action<T> action,
                T item, object syncRoot, int i)
        {
            if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                Thread.CurrentThread.Name =
            String.Format("Parallel.ForEach Worker Thread No:{0}", i);

            bool moveNext = true;

            while (moveNext)
            {
                action.Invoke(item);

                lock (syncRoot)
                {
                    moveNext = enumerator.MoveNext();
                    item = enumerator.Current;
                }
            }
        }
    }
}
