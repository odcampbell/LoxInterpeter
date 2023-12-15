using System;
using System.IO;
using System.Collections.Generic;

namespace Tool1
{
    public class GenerateAst
    {
        public static void Main(string[] args){
            // if (args.Length != 1){
            //     Console.Error.WriteLine("Usage: generate_ast <output directory>");
            //     // Environment.Exit(1);
            // }
            string outputDir = @"C:\cygwin64\home\campb\cs403\LoxInterpreter\LoxApp";
            // string outputDir = args[0];

            List<string> Arrays = new List<string> { 
                "Binary   : Expr left, Token @operator, Expr right",
                "Grouping : Expr expression",
                "Literal  : Object value",
                "Unary    : Token @operator, Expr right" };

            DefineAst(outputDir, "Expr", Arrays);
        }

        private static void DefineAst(string outputDir, string baseName, List<string> types){
            string path = outputDir + @"\" + baseName + ".cs";
            using (StreamWriter writer = new StreamWriter(path)){
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine();
                writer.WriteLine("namespace LoxApp");
                writer.WriteLine("{");
                writer.WriteLine("    abstract class " + baseName);
                writer.WriteLine("    {");
                DefineVisitor(writer, baseName, types);

                foreach (string type in types)
                {
                    string[] typeParts = type.Split(':');
                    string className = typeParts[0].Trim();
                    string fields = typeParts[1].Trim();
                    defineType(writer, baseName, className, fields);
                }

                writer.WriteLine();
                writer.WriteLine("  public abstract R Accept<R>(Visitor<R> visitor);");

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
        }

        private static void DefineVisitor(
            StreamWriter writer, string baseName, List<string> types) {
            writer.WriteLine("  public interface Visitor<R> {");
            foreach (string type in types) {
            string typeName = type.Split(":")[0].Trim();
            writer.WriteLine("    R Visit" + typeName + baseName + "(" +
                typeName + " " + baseName.ToLower() + ");");
            }
            writer.WriteLine("  }");
        }

        private static void defineType(
            StreamWriter writer, string baseName,
            string className, string fieldList) {
            writer.WriteLine("  public class " + className + " : " +
                baseName + " {");
            
            writer.WriteLine("    public " + className + "(" + fieldList + ") {");
            
            string[] fields = fieldList.Split(", ");
            foreach (string field in fields) {
                string name = field.Split(" ")[1];
                writer.WriteLine("      this." + name + " = " + name + ";");
            }
            writer.WriteLine("    }");
            writer.WriteLine();
            writer.WriteLine("    public override R Accept<R>(Visitor<R> visitor) {");
            writer.WriteLine("        return visitor.Visit" +
                className + baseName + "(this);");
            writer.WriteLine("    }");
            writer.WriteLine();
            foreach (string field in fields) {
                writer.WriteLine("    readonly " + field + ";");
            }
            writer.WriteLine("  }");
        }
    }
}


