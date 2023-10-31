namespace aengine_cs.aengine.parser; 

public class ParsedObject {
    public Dictionary<string, object> data = new Dictionary<string, object>();

    public ParsedObject(Dictionary<string, object> data) {
        this.data = data;
    }
    
    public T getValue<T>(string name) {
        return (T)data[name];
    }
}