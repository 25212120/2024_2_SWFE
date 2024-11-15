public class Chunk
{
    public int width;
    public int height;
    public Cell[,] cells;

    public Chunk(int width, int height)
    {
        this.width = width;
        this.height = height;
        cells = new Cell[width, height];
    }
}
