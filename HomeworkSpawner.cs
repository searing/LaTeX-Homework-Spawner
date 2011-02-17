using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace LaTeX_Homework_Spawner {
    static class HomeworkSpawner {
        public static string Spawn(string root, string template, IDictionary<string, string> strArgs, IDictionary<string, bool> boolArgs) {
            bool openEditor = boolArgs["OpenEditor"];
            bool stripComments = boolArgs["StripComments"];
            try {
                string latexString = "";
                using (StreamReader sr = new StreamReader(new FileStream(Path.Combine(root, template), FileMode.Open, FileAccess.Read, FileShare.Read))) {
                    string rawTemplate = sr.ReadToEnd();
                    latexString = createLatexString(rawTemplate, strArgs, boolArgs);
                    if (stripComments) {
                        latexString = stripLatexComments(latexString);
                    }
                }
                if (latexString != "") {
                    using (StreamWriter sw = openOutputFile(root, strArgs["ClassCode"], strArgs["HomeworkTitle"])) {
                        sw.Write(latexString);
                    }
                    if (openEditor) {
                        System.Diagnostics.Process.Start(Path.Combine(
                            root,
                            sanitizeFolderName(strArgs["ClassCode"]),
                            sanitizeFolderName(strArgs["HomeworkTitle"]),
                            sanitizeTexFileName(strArgs["ClassCode"] + "__" + strArgs["HomeworkTitle"]) + ".tex"));
                    }
                    return "";
                } else {
                    return "Empty LaTeX string.";
                }
            } catch (IOException e) {
                return "Error: " + e.Message;
            }

        }

        private static string createLatexString(string source, IDictionary<string, string> strArgs, IDictionary<string, bool> boolArgs) {
            string formatted = source;
            foreach (string key in boolArgs.Keys) {
                formatted = formatted.Replace("<<not." + key + ">>", boolArgs[key] ? "%%% " : "");
                formatted = formatted.Replace("<<" + key + ">>", boolArgs[key] ? "" : "%%% ");
            }
            foreach (string key in strArgs.Keys) {
                formatted = formatted.Replace("<<" + key + ">>", strArgs[key]);
            }
            return formatted;
        }

        private static string stripLatexComments(string source) {
            string strippedLatex = new Regex(@"%[^\n\r]*").Replace(source, "");
            strippedLatex = new Regex(@"[\n\r]+").Replace(strippedLatex, "\r\n");
            return strippedLatex;
        }

        private static char sanitizeFolderChar(char c) {
            if (@"\/:*?""<>|".Contains(c)) {
                return '_';
            } else {
                return c;
            }
        }

        private static char sanitizeTexFileChar(char c) {
            if (Char.IsLetterOrDigit(c)) {
                return c;
            } else {
                return '_';
            }
        }

        private static string sanitizeFolderName(string folder) {
            return new String(folder.ToCharArray().Select(c => sanitizeFolderChar(c)).ToArray());
        }

        private static string sanitizeTexFileName(string file) {
            return new String(file.ToCharArray().Select(c => sanitizeTexFileChar(c)).ToArray());
        }

        private static StreamWriter openOutputFile(string root, string classCode, string homeworkTitle) {
            // Store in <root>/<classCode>/<homeworkTitle>/<classCode>_<homeworkTitle>.tex
            if (!Directory.Exists(root)) {
                Directory.CreateDirectory(root);
            }
            if (!Directory.Exists(Path.Combine(root, sanitizeFolderName(classCode)))) {
                Directory.CreateDirectory(Path.Combine(root, sanitizeFolderName(classCode)));
            }
            if (!Directory.Exists(Path.Combine(root, sanitizeFolderName(classCode), sanitizeFolderName(homeworkTitle)))) {
                Directory.CreateDirectory(Path.Combine(root, sanitizeFolderName(classCode), sanitizeFolderName(homeworkTitle)));
            }
            return new StreamWriter(new FileStream(
                Path.Combine(root, sanitizeFolderName(classCode), sanitizeFolderName(homeworkTitle), sanitizeTexFileName(classCode + "__" + homeworkTitle) + ".tex"),
                FileMode.Create, FileAccess.Write, FileShare.None));
        }
    }
}
