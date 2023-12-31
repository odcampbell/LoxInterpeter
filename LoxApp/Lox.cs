using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace LoxApp
{
	public class Lox {

		private static readonly Interpreter interpreter = new Interpreter();
		private static bool hadError = false;
		public static bool hadRuntimeError = false;

		public class Program{
			static void Main(string[]args){
				
				if(args.Length > 1){
					Console.WriteLine("Usage: jlox [script]");
					System.Environment.Exit(1);
				}
				else if(args.Length == 1){
					runFile(args[0]);
				}
				else{
					runPrompt();
				}
			}
		}

		private static void runFile(string path){
			byte[] bytes = File.ReadAllBytes(path);
			run(System.Text.Encoding.Default.GetString(bytes));
			if(hadError) System.Environment.Exit(1);
			if (hadRuntimeError) System.Environment.Exit(1);//70? //1?

		}

		private static void runPrompt(){

			StreamReader input = new StreamReader(Console.OpenStandardInput());
			for (;;)
			{
				Console.Write("> ");
				string? line = input.ReadLine(); //?
				if (line == null) break;
				run(line);
				hadError = false;
			}
		}

		private static void run(string source){

			Scanner scanner = new Scanner(source);
			List<Token> tokens = scanner.ScanTokens();
			
			Parser parser = new Parser(tokens);
			List<Stmt> statements = parser.parse();

			if (hadError) return;
			Resolver resolver = new(interpreter);
    		resolver.resolve(statements);
			if (hadError) return;

			interpreter.interpret(statements);
			// Console.WriteLine(new AstPrinter().Print(expression));

			if(hadError) System.Environment.Exit(1);
			if (hadRuntimeError) System.Environment.Exit(70);//70? //1?
		}

		public static void error(int line, string message){
			report(line, "", message);
		}

		public static void runtimeError(RuntimeError error)
		{
			Console.Error.WriteLine(error.Message +
				$"\n[line {error.token.line}]");
			hadRuntimeError = true;
		}

		public static void report(int line, string where, string message){
			Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
			hadError = true;
		}

		public static void error(Token token, string message){
			if (token.type == TokenType.EOF){
				report(token.line, " at end", message);
			}
			else
			{
				report(token.line, " at '" + token.lexeme + "'", message);
			}
		}

	}
}