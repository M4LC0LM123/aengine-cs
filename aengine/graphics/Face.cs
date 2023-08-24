namespace aengine.graphics; 

public class Face {
    public List<int> indices = new List<int>();
    public int indexCount;

    public Face() {
        indexCount = indices.Count;
    }

    public Face(List<int> indices) {
        this.indices = indices;
        indexCount = indices.Count;
    }
    
}