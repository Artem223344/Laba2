package lab;

import java.util.Arrays;
import java.util.Random;
import java.util.Scanner;

public class Main { 
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        System.out.print("Введіть кількість потоків (натисніть Enter для значення за замовчуванням = 8): ");
        String input = scanner.nextLine();
        int threadCount = 8;
        if (!input.isBlank()) {
            try {
                threadCount = Integer.parseInt(input);
            } catch (NumberFormatException e) {
                System.out.println("Некоректне значення! Використовується значення за замовчуванням: 8");
            }
        }

        double[] array = getRandomArray(12_000_000);

        System.out.println("\nПошук мінімального значення (багатопотоковий режим):");
        long start = System.currentTimeMillis();
        ThreadController threadController = new ThreadController(threadCount, array);
        double minValue = threadController.getMin();
        int minIndex = threadController.getMinIndex();
        long finish = System.currentTimeMillis();
        long timeElapsed = finish - start;

        System.out.println("Мінімальне значення: " + minValue);
        System.out.println("Індекс мінімального значення: " + minIndex);
        System.out.println("Час виконання: " + timeElapsed + " мс");

        System.out.println("\nПеревірка результату (стандартний метод):");
        start = System.currentTimeMillis();
        double min = Arrays.stream(array).min().getAsDouble();
        finish = System.currentTimeMillis();
        timeElapsed = finish - start;

        System.out.println("Мінімальне значення: " + min);
        System.out.println("Час виконання: " + timeElapsed + " мс");

        boolean resultsMatch = Math.abs(minValue - min) < 1e-10;
        System.out.println("\nРезультати збігаються: " + (resultsMatch ? "yes" : "no"));
    }

    private static double[] getRandomArray(int count) {
        Random random = new Random();
        double[] array = new double[count];

        for (int i = 0; i < count; i++) {
            array[i] = random.nextDouble() * 5000; // позитивні значення
        }

        // Замінити випадковий елемент на від’ємне число
        int negativeIndex = random.nextInt(count);
        array[negativeIndex] = -(random.nextDouble() * 5000);

        return array;
    }
}
