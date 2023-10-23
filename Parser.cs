using System.Text.RegularExpressions;

namespace aengine_cs;

public class Parser {
    public static Dictionary<string, Dictionary<string, object>> parseCustomFile(string[] fileContent) {
        var objects = new Dictionary<string, Dictionary<string, object>>();
        string currentObjectName = null;
        Dictionary<string, object> currentObject = null;
        bool isInComment = false;

        foreach (var line in fileContent) {
            var trimmedLine = line.Trim();

            if (string.IsNullOrEmpty(trimmedLine) || 
                trimmedLine.StartsWith("//") ||
                trimmedLine.Contains("/*") && trimmedLine.Contains("*/")) {
                continue;
            }

            if (isInComment) {
                if (trimmedLine.Contains("*/")) {
                    isInComment = false;
                }
                
                continue;
            }

            if (trimmedLine.Contains("/*")) {
                isInComment = true;
                
                continue;
            }

            if (trimmedLine.StartsWith("object ")) {
                if (currentObject != null) {
                    objects[currentObjectName] = currentObject;
                }

                var match = Regex.Match(trimmedLine, @"object (\w+)");
                currentObjectName = match.Groups[1].Value;
                currentObject = new Dictionary<string, object>();
            } else {
                var match = Regex.Match(trimmedLine, @"(\w+) (\w+) = (.+);");

                if (match.Success) {
                    var dataType = match.Groups[1].Value;
                    var attribute = match.Groups[2].Value;
                    var value = match.Groups[3].Value;

                    if (dataType == "int") {
                        currentObject[attribute] = int.Parse(value);
                    } else if (dataType == "float") {
                        currentObject[attribute] = float.Parse(value);
                    } else if (dataType == "bool") {
                        currentObject[attribute] = bool.Parse(value);
                    } else if (dataType == "string") {
                        currentObject[attribute] = value.Trim('"');
                    }
                }
            }
        }

        if (currentObject != null) {
            objects[currentObjectName] = currentObject;
        }

        return objects;
    }

    public static string[] readCustomFile(string filePath) {
        return File.ReadAllLines(filePath);
    }

    public static void main() {
        Directory.SetCurrentDirectory("../../../");

        var fileLines = readCustomFile("data.od");
        var parsedData = parseCustomFile(fileLines);

        foreach (var objectName in parsedData.Keys) {
            Console.WriteLine($"Object: {objectName}");

            foreach (var attribute in parsedData[objectName].Keys) {
                var value = parsedData[objectName][attribute];
                Console.WriteLine($"  {attribute}: {value}");
            }
        }
    }
}