﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatXml.FXml
{
	public class LLParser
	{

		IEnumerable<Token> tokens;
		int pos;
		List<Error> errors;

		public IEnumerable<FXmlElement> FXmlElements { get; private set; }
		public IEnumerable<Error> Errors { get { return errors.AsEnumerable(); } }

		private void SyntaxError(string expected, Token token)
		{
			int line = token != null ? token.Line : tokens.ElementAt(tokens.Count() - 1).Line;
			string err = string.Format("Syntax error at line {0}. Expected: {1}", line, expected);
			errors.Add(new Error { Text = err, Line = line });
		}

		private Token GetTokenAtPos(int p)
		{
			if (p >= tokens.Count())
				return null;
			return tokens.ElementAt(p);
		}

		private void ParseContext(FXmlElement element)
		{
			Token t = GetTokenAtPos(pos);
			if (t == null || t.Type != TokenType.OPEN_BRACES_SIGN)
			{
				SyntaxError("{", t);
				return;
			}

			pos = pos + 1;

			// Get children
			IEnumerable<FXmlElement> children = ParseStatements();
			foreach (FXmlElement e in children)
				element.Children.Add(e);

			t = GetTokenAtPos(pos);
			if (t == null || t.Type != TokenType.CLOSE_BRACES_SIGN)
			{
				SyntaxError("}", t);
				return;
			}

			pos = pos + 1;
		}

		private void ParseAttributes(FXmlElement element)
		{
			Token t1 = GetTokenAtPos(pos);
			Token t2 = GetTokenAtPos(pos + 1);
			Token t3 = GetTokenAtPos(pos + 2);

			if (t1 == null || t1.Type != TokenType.ID)
			{
				SyntaxError("attribute", t1);
				return;
			}
			if (t2 == null || t2.Type != TokenType.ASSIGN_SIGN)
			{
				SyntaxError("attribute", t2);
				return;
			}
			if (t3 == null || t3.Type != TokenType.LITERAL)
			{
				SyntaxError("attribute", t3);
				return;
			}

			// Add attributes
			element.FXmlAttributes.Add(t1.Value, t3.Value);

			pos = pos + 3;
			Token next = GetTokenAtPos(pos);
			Token next2 = GetTokenAtPos(pos + 1);

			if (next != null && next.Type == TokenType.OPEN_BRACES_SIGN)
				ParseContext(element);
			else if (next != null && next2 != null && next.Type == TokenType.ID && next2.Type == TokenType.ASSIGN_SIGN)
				ParseAttributes(element);
		}

		private FXmlElement ParseTask()
		{
			Token t = GetTokenAtPos(pos);
			if (t == null || t.Type != TokenType.ID)
			{
				SyntaxError("element", t);
				return null;
			}

			// Create element
			FXmlElement element = new FXmlElement { Name = t.Value };

			pos = pos + 1;
			Token next = GetTokenAtPos(pos);
			Token next2 = GetTokenAtPos(pos + 1);

			if (next != null && next.Type == TokenType.OPEN_BRACES_SIGN)
				ParseContext(element);
			else if (next != null && next2 != null && next.Type == TokenType.ID && next2.Type == TokenType.ASSIGN_SIGN)
				ParseAttributes(element);

			return element;
		}

		private IEnumerable<FXmlElement> ParseStatements()
		{
			List<FXmlElement> elements = new List<FXmlElement>();

			Token t = GetTokenAtPos(pos);
			while (t != null && t.Type == TokenType.ID)
			{
				FXmlElement e = ParseTask();
				t = GetTokenAtPos(pos);

				elements.Add(e);
			}

			return elements;
		}

		private IEnumerable<FXmlElement> ParseCode()
		{
			IEnumerable<FXmlElement> elements = ParseStatements();

			Token t = GetTokenAtPos(pos);
			if (t != null)
				SyntaxError("element", t);

			return elements;
		}

		public bool Parse(IEnumerable<Token> tokens)
		{
			this.tokens = tokens;
			this.pos = 0;
			this.errors = new List<Error>();

			FXmlElements = ParseCode();

			if (this.errors.Count > 0)
				return false;

			return true;
		}

	}
}