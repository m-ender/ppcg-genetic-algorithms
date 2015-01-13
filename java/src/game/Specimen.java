package game;

public class Specimen {
    public final long dna;
    public final int birthTurn;
    public Specimen(long dna, int birthTurn){
        this.dna = dna;
        this.birthTurn = birthTurn;
    }

    public int bitAt(int position){
        return (int)(this.dna >> position) & 1;
    }

    public long bitRange(int start, int stop){
        return (this.dna >> start) & ((1 << (stop-start))-1);
    }
}
