using System.Globalization;
using System.Text.RegularExpressions;
using aengine_cs.aengine.parser;

namespace aengine_cs;

public class Test {
    public static void main() {
        Directory.SetCurrentDirectory("../../../");
        
        var parsedData = Parser.parse(Parser.read("data.od"));
        
        foreach (var objectName in parsedData.Keys) {
            Console.WriteLine($"Object: {objectName}");

            foreach (var attribute in parsedData[objectName].Keys) {
                var value = parsedData[objectName][attribute];
                Console.WriteLine($"  {attribute}: {value}, type: {value.GetType()}");
            }
        }
    }
}