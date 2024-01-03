using System.Globalization;
using System.Text.RegularExpressions;

namespace aengine_cs.aengine.parser; 

public class Parser {
    private static string COMMENT_CHAR       = "//";
    private static string COMMENT_START_CHAR = "/*";
    private static string COMMENT_END_CHAR   = "*/";
    private static string FLOAT_CHAR         = "float";
    private static string F32_CHAR           = "f32";
    private static string INT_CHAR           = "int";
    private static string I32_CHAR           = "i32";
    private static string STRING_CHAR        = "string";
    private static string STR_CHAR           = "str";
    private static string BOOL_CHAR          = "bool";
    
    public static ParsedData parse(string[] fileContent) {
        var objects = new Dictionary<string, ParsedObject>();
        string currentObjectName = null;
        ParsedObject currentObject = null;
        Dictionary<string, object> macros = new Dictionary<string, object>();
        bool isInComment = false;
        bool openingBraceFound = false;
        
        for (int i = 0; i < fileContent.Length; i++) {
            string trimmedLine = fileContent[i].Trim();
            string lineBehind = String.Empty;
            if (i - 1 > -1) lineBehind = fileContent[(i - 1) % fileContent.Length].Trim();
            string lineInFront = fileContent[(i + 1) % fileContent.Length].Trim();

            if (string.IsNullOrEmpty(trimmedLine) || 
                trimmedLine.StartsWith(COMMENT_CHAR) ||
                trimmedLine.Contains(COMMENT_START_CHAR) && trimmedLine.Contains(COMMENT_END_CHAR)) {
                continue;
            }

            if (isInComment) {
                if (trimmedLine.Contains(COMMENT_END_CHAR)) {
                    isInComment = false;
                }
                
                continue;
            }

            if (trimmedLine.Contains(COMMENT_START_CHAR)) {
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
                    } else if (float.TryParse(macroValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue)) { // float parsing bug on OSX
                        macros[macroName] = floatValue;
                    } else if (macroValue.Contains("true") || macroValue.Contains("false")) {
                        macros[macroName] = bool.Parse(macroValue);
                    } else {
                        // else its a string
                        macros[macroName] = macroValue.Trim('"');
                    }
                }
            }

            if (trimmedLine.StartsWith("object ") && (trimmedLine.EndsWith("{") || lineInFront.StartsWith("{"))) {
                if (openingBraceFound) {
                    throw new Exception(
                        "Invalid format: Object definition must have both opening and closing curly braces: " + (i + 1)
                    );
                }
                
                if (currentObject != null) {
                    objects[currentObjectName] = currentObject;
                }

                Match match = Regex.Match(trimmedLine, @"object (\w+)");
                currentObjectName = match.Groups[1].Value;
                currentObject = new ParsedObject(new Dictionary<string, object>(), "default");
                openingBraceFound = true;
                
                if (lineBehind.StartsWith("#mod ")) {
                    Match match2 = Regex.Match(lineBehind, @"#mod (\w+)");
                
                    if (match2.Success) {
                        currentObject.modifier = match2.Groups[1].Value;
                    }
                }
            } else if (trimmedLine == "}") {
                if (!openingBraceFound) {
                    throw new Exception(
                        "Invalid format: Closing brace without a corresponding opening brace, line: " + (i + 1)
                    );
                }
                
                if (currentObject != null) {
                    objects[currentObjectName] = currentObject;
                }

                currentObject = null;
                currentObjectName = null;
                openingBraceFound = false;
            } else {
                Match match = Regex.Match(trimmedLine, @"(\w+) (\w+) = (.+);");
                
                if (match.Success) {
                    string dataType = match.Groups[1].Value;
                    string attribute = match.Groups[2].Value;
                    string value = match.Groups[3].Value;
                
                    if (macros.ContainsKey(value)) {
                        value = macros[value].ToString();
                    }
                    
                    if (dataType == INT_CHAR || dataType == I32_CHAR) {
                        currentObject.data[attribute] = int.Parse(value);
                    } else if (dataType == FLOAT_CHAR || dataType == F32_CHAR) {
                        currentObject.data[attribute] = float.Parse(value, 
                            CultureInfo.InvariantCulture);
                    } else if (dataType == BOOL_CHAR) {
                        currentObject.data[attribute] = bool.Parse(value);
                    } else if (dataType == STRING_CHAR || dataType == STR_CHAR) {
                        currentObject.data[attribute] = value.Trim('"');
                    }
                }
            }
        }

        if (openingBraceFound) {
            throw new Exception("Invalid format: Unclosed object definition");
        }

        if (currentObject != null) {
            objects[currentObjectName] = currentObject;
        }

        return new ParsedData(objects);
    }
    
    public static string[] read(string filePath) {
        return File.ReadAllLines(filePath);
    }
    
}