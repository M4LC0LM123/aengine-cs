using System.Globalization;
using System.Text.RegularExpressions;
using aengine_cs;
using aengine_cs.aengine.parser;

namespace aengine_cs;

public class Test {
    public static void main() {
        Directory.SetCurrentDirectory("../../../");
        
        ParsedData parsedData = Parser.parse(Parser.read("data.od"));
        
        foreach (string objectName in parsedData.objects) {
            Console.WriteLine($"Object: {objectName}");
            
            foreach (string attribute in parsedData.dataKeys(objectName)) {
                object value = parsedData.getObject(objectName).getValue<object>(attribute);
                Console.WriteLine($"  {attribute}: {value}, type: {value.GetType()}");
            }
        }

        while (true) {
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.R) {
                Console.Clear();
                Console.WriteLine("Reloading...");
                Thread.Sleep(1000);
                Console.WriteLine("Reloaded!");
                Thread.Sleep(300);
                Console.Clear();
                
                parsedData = Parser.parse(Parser.read("data.od"));
                
                foreach (string objectName in parsedData.objects) {
                    Console.WriteLine($"Object: {objectName}");
            
                    foreach (string attribute in parsedData.dataKeys(objectName)) {
                        object value = parsedData.getObjectValue<object>(objectName, attribute);
                        Console.WriteLine($"  {attribute}: {value}, type: {value.GetType()}");
                    }
                }   
            }
        }
    }
}