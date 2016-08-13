using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatXml.FXml
{
	public enum TokenType
	{
		SPACE,
		COMMENT,
		ID,
		LITERAL,
		ASSIGN_SIGN,
		OPEN_BRACES_SIGN,
		CLOSE_BRACES_SIGN
	}

	public class Token
	{
		public int Line { get; set; }
		public TokenType Type { get; set; }
		public string Value { get; set; }
	}

	public enum LexicalState
	{
		ERROR,
		STOP,
		INITIAL,
		FINAL,
		COMMENT1,
		COMMENT2,
		ID1,
		ID2,
		OPEN_BRACES,
		CLOSE_BRACES,
		ASSIGN,
		STRINGLITERAL1,
		STRINGLITERAL2,
		STRINGLITERAL3,
		SPACE
	}

	public enum ParserAction
	{
		PREV,
		KEEP,
		NEXT
	}

	public class ParserResult
	{
		public LexicalState State { get; set; }
		public ParserAction Action { get; set; }
		public Token Token { get; set; }

		public static ParserResult ERROR = new ParserResult { State = LexicalState.ERROR, Action = ParserAction.KEEP };
		public static ParserResult STOP = new ParserResult { State = LexicalState.STOP, Action = ParserAction.KEEP };
		public static ParserResult INITIAL = new ParserResult { State = LexicalState.INITIAL, Action = ParserAction.KEEP };
		public static ParserResult FINAL = new ParserResult { State = LexicalState.FINAL, Action = ParserAction.KEEP };
		public static ParserResult FINAL_MOVE = new ParserResult { State = LexicalState.FINAL, Action = ParserAction.NEXT };
	}

	/*
	 *
	 * A very simple state machine
	 * 
	 */
	public class TokenParser
	{

		public const string STOP = "\0";
		public const string LETTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public const string NUMBERS = "0123456789";
		public const string OPENBRACES = "{";
		public const string CLOSEBRACES = "}";
		public const string EQUALS = "=";
		public const string QUOTATION = "\"";
		public const string HYPHEN = "-";
		public const string UNDERSCORE = "_";
		public const string DOT = ".";
		public const string COLON = ":";
		public const string NUMERAL = "#";
		public const string NEWLINE = "\n";
		public const string SPACE = " \n\t";

		ParserResult result;
		int currentLine;
		int pos;
		string chr;
		int initialPos;
		int finalPos;
		TokenType tokenType;

		public IEnumerable<Token> Tokens { get; private set; }
		public Error Error { get; private set; }

		private ParserResult InitialState(string line)
		{
			initialPos = pos;

			if (LETTERS.Contains(chr))
				return new ParserResult { State = LexicalState.ID1, Action = ParserAction.KEEP };

			if (OPENBRACES.Contains(chr))
				return new ParserResult { State = LexicalState.OPEN_BRACES, Action = ParserAction.KEEP };

			if (CLOSEBRACES.Contains(chr))
				return new ParserResult { State = LexicalState.CLOSE_BRACES, Action = ParserAction.KEEP };

			if (EQUALS.Contains(chr))
				return new ParserResult { State = LexicalState.ASSIGN, Action = ParserAction.KEEP };

			if (QUOTATION.Contains(chr))
				return new ParserResult { State = LexicalState.STRINGLITERAL1, Action = ParserAction.KEEP };

			if (SPACE.Contains(chr))
				return new ParserResult { State = LexicalState.SPACE, Action = ParserAction.KEEP };

			if (NUMERAL.Contains(chr))
				return new ParserResult { State = LexicalState.COMMENT1, Action = ParserAction.KEEP };

			return ParserResult.ERROR;
		}

		private ParserResult FinalState(string line)
		{
			string name = line.Substring(initialPos, finalPos - initialPos).Trim();
			Token token = new Token { Value = name, Type = tokenType };
			ParserResult result = ParserResult.INITIAL;
			if (chr.Equals(STOP))
				result = ParserResult.STOP;
			result.Token = token;
			return result;
		}

		private ParserResult Id1State(string line)
		{
			tokenType = TokenType.ID;

			if (LETTERS.Contains(chr))
				return new ParserResult { State = LexicalState.ID2, Action = ParserAction.NEXT };

			return ParserResult.ERROR;
		}

		private ParserResult Id2State(string line)
		{
			tokenType = TokenType.ID;

			if (LETTERS.Contains(chr) ||
			    NUMBERS.Contains(chr) ||
			    HYPHEN.Contains(chr) ||
			    UNDERSCORE.Contains(chr) ||
			    DOT.Contains(chr))
				return new ParserResult { State = LexicalState.ID2, Action = ParserAction.NEXT };

			finalPos = pos;
			return ParserResult.FINAL;
		}

		private ParserResult OpenBracesState(string line)
		{
			tokenType = TokenType.OPEN_BRACES_SIGN;

			if (OPENBRACES.Contains(chr))
				return new ParserResult { State = LexicalState.OPEN_BRACES, Action = ParserAction.NEXT };

			finalPos = pos;
			return ParserResult.FINAL;
		}

		private ParserResult CloseBracesState(string line)
		{
			tokenType = TokenType.CLOSE_BRACES_SIGN;

			if (CLOSEBRACES.Contains(chr))
				return new ParserResult { State = LexicalState.CLOSE_BRACES, Action = ParserAction.NEXT };
			
			finalPos = pos;
			return ParserResult.FINAL;
		}

		private ParserResult AssignState(string line)
		{
			tokenType = TokenType.ASSIGN_SIGN;

			if (EQUALS.Contains(chr))
				return new ParserResult { State = LexicalState.ASSIGN, Action = ParserAction.NEXT };
			
			finalPos = pos;
			return ParserResult.FINAL;
		}

		private ParserResult StringLiteral1State(string line)
		{
			initialPos = pos + 1;
			tokenType = TokenType.LITERAL;

			if (QUOTATION.Contains(chr))
				return new ParserResult { State = LexicalState.STRINGLITERAL2, Action = ParserAction.NEXT };

			return ParserResult.ERROR;
		}

		private ParserResult StringLiteral2State(string line)
		{
			if (!QUOTATION.Contains(chr))
				return new ParserResult { State = LexicalState.STRINGLITERAL2, Action = ParserAction.NEXT };

			return new ParserResult { State = LexicalState.STRINGLITERAL3, Action = ParserAction.NEXT };
		}

		private ParserResult StringLiteral3State(string line)
		{
			if (SPACE.Contains(chr))
			{
				finalPos = pos - 1;
				return ParserResult.FINAL;
			}

			return ParserResult.ERROR;
		}

		private ParserResult Comment1State(string line)
		{
			initialPos = pos + 1;
			tokenType = TokenType.COMMENT;

			if (NUMERAL.Contains(chr))
				return new ParserResult { State = LexicalState.COMMENT2, Action = ParserAction.NEXT };

			return ParserResult.ERROR;
		}

		private ParserResult Comment2State(string line)
		{
			if (!NEWLINE.Contains(chr))
				return new ParserResult { State = LexicalState.COMMENT2, Action = ParserAction.NEXT };

			finalPos = pos;
			return ParserResult.FINAL;
		}

		private ParserResult SpaceState(string line)
		{
			tokenType = TokenType.SPACE;
			if (SPACE.Contains(chr))
			{
				if (chr.Equals(NEWLINE))
					currentLine = currentLine + 1;

				return new ParserResult { State = LexicalState.SPACE, Action = ParserAction.NEXT };
			}

			finalPos = pos;
			return ParserResult.FINAL;
		}

		public bool Parse(string code)
		{
			List<Token> tokens = new List<Token>();

			result = ParserResult.INITIAL;
			tokenType = TokenType.SPACE;

			currentLine = 1;
			pos = 0;
			while (result.State != LexicalState.ERROR && result.State != LexicalState.STOP)
			{
				chr = string.Empty;
				if (pos < code.Length)
					chr = code[pos].ToString();

				switch (result.State)
				{
					case LexicalState.INITIAL:
						result = InitialState(code);
						break;

					case LexicalState.ID1:
						result = Id1State(code);
						break;

					case LexicalState.ID2:
						result = Id2State(code);
						break;

					case LexicalState.OPEN_BRACES:
						result = OpenBracesState(code);
						break;

					case LexicalState.CLOSE_BRACES:
						result = CloseBracesState(code);
						break;

					case LexicalState.ASSIGN:
						result = AssignState(code);
						break;

					case LexicalState.STRINGLITERAL1:
					result = StringLiteral1State(code);
						break;

					case LexicalState.STRINGLITERAL2:
						result = StringLiteral2State(code);
						break;

					case LexicalState.STRINGLITERAL3:
						result = StringLiteral3State(code);
						break;

					case LexicalState.COMMENT1:
						result = Comment1State(code);
						break;

					case LexicalState.COMMENT2:
						result = Comment2State(code);
						break;

					case LexicalState.SPACE:
						result = SpaceState(code);
						break;

					case LexicalState.FINAL:
						result = FinalState(code);
						TokenType type = result.Token.Type;
						if (type != TokenType.SPACE && type != TokenType.COMMENT)
						{
							// Console.WriteLine("Token detected - Type: {0}, Value: {1}", result.Token.Type.ToString(), result.Token.Value);
							result.Token.Line = currentLine;
							tokens.Add(result.Token);
						}
						break;
				}

				switch (result.Action)
				{
					case ParserAction.PREV:
						pos--;
						break;

					case ParserAction.NEXT:
						pos++;
						break;

					case ParserAction.KEEP:
						break;
				}
			}

			if (result.State == LexicalState.ERROR)
			{
				string err = string.Format("Syntax error at line {0}. Unrecognized symbol {1}", currentLine, chr);
				Error = new Error { Text = err, Line = currentLine };
				return false;
			}

			Tokens = tokens.AsEnumerable();
			return true;
		}

	}
}

