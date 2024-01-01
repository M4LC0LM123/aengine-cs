namespace aengine_cs.aengine.parser; 

public class ParsedData {
    public Dictionary<string, ParsedObject> data;

    public ParsedData(Dictionary<string, ParsedObject> data) {
        this.data = data;
    }

    public ParsedData() {
        data = new Dictionary<string, ParsedObject>();
    }

    public Dictionary<string, ParsedObject>.KeyCollection objects => data.Keys;
    
    public Dictionary<string, object>.KeyCollection dataKeys(string objName) {
        return data[objName].data.Keys;
    }

    public ParsedObject getObject(string name) {
        return new ParsedObject(data[name].data, data[name].modifier);
    }
    
    public object getObjectValue(string name, string attribute) {
        return data[name].data[attribute];
    }

    public T getObjectValue<T>(string name, string attribute) {
        return (T) data[name].data[attribute];
    }
    
}