using System.Globalization;
using System.Text.RegularExpressions;

namespace aengine_cs.aengine.parser; 

public class Parser {
    public static Dictionary<string, Dictionary<string, object>> parse(string[] fileContent) {
        var objects = new Dictionary<string, Dictionary<string, object>>();
        string currentObjectName = null;
        Dictionary<string, object> currentObject = null;
        Dictionary<string, object> macros = new Dictionary<string, object>();
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

            if (trimmedLine.StartsWith("#macro ")) {
                Match match = Regex.Match(trimmedLine, @"#macro (\w+) (.+)");

                if (match.Success) {
                    string macroName = match.Groups[1].Value;
                    string macroValue = match.Groups[2].Value;
                    
                    if (int.TryParse(macroValue, out int intValue)) {
                        macros[macroName] = intValue;
                    } else if (float.TryParse(macroValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue)) { // float parsing bug
                        macros[macroName] = floatValue;
                    } else if (macroValue.Contains("true") || macroValue.Contains("false")) {
                        macros[macroName] = bool.Parse(macroValue);
                    } else {
                        // else its a string
                        macros[macroName] = macroValue.Trim('"');
                    }
                }
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
                    string dataType = match.Groups[1].Value;
                    string attribute = match.Groups[2].Value;
                    string value = match.Groups[3].Value;

                    if (macros.ContainsKey(value)) {
                        value = macros[value].ToString();
                    }
                    
                    if (dataType == "int") {
                        currentObject[attribute] = int.Parse(value);
                    } else if (dataType == "float") {
                        currentObject[attribute] = float.Parse(value, 
                            CultureInfo.InvariantCulture);
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
    
    public static string[] read(string filePath) {
        return File.ReadAllLines(filePath);
    }
    
}