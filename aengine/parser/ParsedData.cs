namespace aengine_cs.aengine.parser; 

public class ParsedData {
    public Dictionary<string, Dictionary<string, object>> data;

    public ParsedData(Dictionary<string, Dictionary<string, object>> data) {
        this.data = data;
    }

    public ParsedData() {
        data = new Dictionary<string, Dictionary<string, object>>();
    }

    public Dictionary<string, Dictionary<string, object>>.KeyCollection objects => data.Keys;
    
    public Dictionary<string, object>.KeyCollection dataKeys(string objName) {
        return data[objName].Keys;
    }

    public Dictionary<string, object> getObject(string name) {
        return data[name];
    }
    
    public object getObjectValue(string name, string attribute) {
        return data[name][attribute];
    }

    public T getObjectValue<T>(string name, string attribute) {
        return (T) data[name][attribute];
    }
    
}