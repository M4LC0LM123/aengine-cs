namespace aengine_cs.aengine.parser; 

public class ParsedObject {
    public Dictionary<string, object> data = new Dictionary<string, object>();
    public string modifier = String.Empty;

    public ParsedObject(Dictionary<string, object> data, string mod) {
        this.data = data;
        modifier = mod;
    }
    
    public T getValue<T>(string name) {
        return (T)data[name];
    }
}