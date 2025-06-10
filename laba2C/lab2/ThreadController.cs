using System;
using System.Threading;

namespace Lab
{
    public class ThreadController
    {
        private readonly int threadCount;
        private readonly double[] array;
        private readonly double[] minInBoundElems;
        private readonly int[] minIndices;
        private double globalMin;
        private int globalMinIndex;
        private int finishedThreadCount;
        private readonly object lockObject = new object();

        public ThreadController(int threadCount, double[] array)
        {
            this.threadCount = threadCount;
            this.array = array;
            this.minInBoundElems = new double[threadCount];
            this.minIndices = new int[threadCount];
            this.finishedThreadCount = 0;
            this.globalMin = double.MaxValue;
            this.globalMinIndex = -1;

            StartThreads(); 
        }

        private void StartThreads()
        {
            for (int i = 0; i < threadCount; i++)
            {
                int threadId = i;
                int firstBound = threadId * array.Length / threadCount;
                int secondBound = (threadId + 1) * array.Length / threadCount;

                Thread thread = new Thread(() =>
                {
                    MinResult result = PartMin(firstBound, secondBound);
                    CollectMin(threadId, result.Value, result.Index);
                });

                thread.Start(); 
            }
        }

        public void CollectMin(int threadId, double minElem, int minIndex)
        {
            lock (lockObject)
            {
                minInBoundElems[threadId] = minElem;
                minIndices[threadId] = minIndex;

                if (minElem < globalMin)
                {
                    globalMin = minElem;
                    globalMinIndex = minIndex;
                }

                finishedThreadCount++;
                if (finishedThreadCount >= threadCount)
                {
                    Monitor.PulseAll(lockObject); 
                }
            }
        }

        public MinResult GetMinWithIndex()
        {
            lock (lockObject)
            {
                while (finishedThreadCount < threadCount)
                {
                    Monitor.Wait(lockObject);
                }
                return new MinResult(globalMin, globalMinIndex);
            }
        }

        public MinResult PartMin(int firstBound, int secondBound)
        {
            double min = array[firstBound];
            int minIndex = firstBound;

            for (int i = firstBound; i < secondBound; i++)
            {
                if (array[i] < min)
                {
                    min = array[i];
                    minIndex = i;
                }
            }

            return new MinResult(min, minIndex);
        }
    }

    public class MinResult
    {
        public double Value { get; }
        public int Index { get; }

        public MinResult(double value, int index)
        {
            Value = value;
            Index = index;
        }
    }
}
