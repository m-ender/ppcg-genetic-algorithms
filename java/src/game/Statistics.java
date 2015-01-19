package game;

public class Statistics {
    private static int totalFitness = 0;
    private static int maxFitness = 0;
    private static int allTimeMaxFitness = 0;
    public static void updateFitness(int fitness){
        if (fitness > maxFitness){
            maxFitness = fitness;
            if (fitness > allTimeMaxFitness){
                allTimeMaxFitness = fitness;
            }
        }
        totalFitness += fitness;
    }
    public static void restart(){
        totalFitness = 0;
    }

    public static int getTotalFitness(){
        return totalFitness;
    }

    public static int getMaxFitness(){
        return maxFitness;
    }

    public static int getAllTimeMaxFitness(){
        return allTimeMaxFitness;
    }
}
