    package lab;

public class ThreadController {
    private final int threadCount;
    private final double[] array;
    private final double[] minInBoundElems;
    private final int[] minIndices;
    private double globalMin = Double.MAX_VALUE;
    private int globalMinIndex = -1;
    private int finishedThreadCount = 0;

    public ThreadController(int threadCount, double[] array) {
        this.threadCount = threadCount;
        this.array = array;
        this.minInBoundElems = new double[threadCount];
        this.minIndices = new int[threadCount];
        startThreads();
    }

    private void startThreads() {
        for (int i = 0; i < threadCount; i++) {
            int start = i * array.length / threadCount;
            int end = (i + 1) * array.length / threadCount;
            new Thread(new MyThread(i, start, end, this)).start();
        }
    }

    public static class MinResult {
        public final double value;
        public final int index;

        public MinResult(double value, int index) {
            this.value = value;
            this.index = index;
        }
    }
    
    public MinResult partMin(int firstBound, int secondBound) {
        double min = array[firstBound];
        int minIndex = firstBound;

        for (int i = firstBound; i < secondBound; i++) {
            if (array[i] < min) {
                min = array[i];
                minIndex = i;
            }
        }

        return new MinResult(min, minIndex);
    }

    public synchronized void collectMin(int threadId, double minElem, int minIndex) {
        minInBoundElems[threadId] = minElem;
        minIndices[threadId] = minIndex;

        if (minElem < globalMin) {
            globalMin = minElem;
            globalMinIndex = minIndex;
        }

        finishedThreadCount++;
        notifyAll();
    }

    private synchronized void waitUntilAllThreadsFinish() {
        while (finishedThreadCount < threadCount) {
            try {
                wait();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }

    public synchronized double getMin() {
        waitUntilAllThreadsFinish();
        return globalMin;
    }

    public synchronized int getMinIndex() {
        waitUntilAllThreadsFinish();
        return globalMinIndex;
    }

    
}
