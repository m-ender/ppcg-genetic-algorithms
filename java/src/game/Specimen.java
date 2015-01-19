package game;

public class Specimen {
    public final String genome;
    public int birthTurn;
    public int bonusFitness;
    public Specimen(String genome, int birthTurn){
        this.genome = genome;
        this.birthTurn = birthTurn;
        this.bonusFitness = 0;
    }

    public int bitAt(int position){
        return Integer.parseInt(genome.charAt(position)+"");
    }

}
