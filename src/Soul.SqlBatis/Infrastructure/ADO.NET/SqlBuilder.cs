﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Soul.SqlBatis
{
	public class SqlBuilder
	{
		private readonly List<Token> _tokens = new List<Token>();

		public SqlBuilder Set(string sql, bool flag = true)
		{
			if (flag)
			{
				_tokens.Add(new Token(TokenType.Set, sql));
			}
			return this;
		}

		public SqlBuilder Where(string sql, bool flag = true)
		{
			if (flag)
			{
				_tokens.Add(new Token(TokenType.Where, $"({sql})"));
			}
			return this;
		}

		public SqlBuilder Join(string sql, bool flag = true)
		{
			if (flag)
			{
				_tokens.Add(new Token(TokenType.Join, sql));
			}
			return this;
		}

		public SqlBuilder Having(string sql, bool flag = true)
		{
			if (flag)
			{
				_tokens.Add(new Token(TokenType.Having, $"({sql})"));
			}
			return this;
		}

		public SqlBuilder OrderBy(string sql, bool flag = true)
		{
			if (flag)
			{
				_tokens.Add(new Token(TokenType.OrderBy, sql));
			}
			return this;
		}

		public SqlBuilder GroupBy(string sql, bool flag = true)
		{
			if (flag)
			{
				_tokens.Add(new Token(TokenType.GroupBy, sql));
			}
			return this;
		}

		public SqlBuilder Limit(int row, int size)
		{
			_tokens.Add(new Token(TokenType.Limit, $"{row}, {size}"));
			return this;
		}

		public SqlBuilder Page(int row, int size)
		{
			return Limit((row - 1) * size, size);
		}

		public string WhereSql => Build("/**where**/");

        public string WhereText 
		{
			get
			{
				return string.Join(" AND ", _tokens.Where(a => a.Type == TokenType.Where).Select(s => s.Text));
			}
		}

        public string Build(string format)
		{
			var tokens = _tokens
				.Where(a => a.Type != TokenType.From)
				.GroupBy(a => a.Type)
				.OrderBy(s => s.Key)
				.Select(s =>
				{
					var connector = ", ";
					if (s.Key == TokenType.Where || s.Key == TokenType.GroupBy)
						connector = " AND ";
					if (s.Key == TokenType.Join)
						connector = " ";
					var expr = string.Join(connector, s.Select(a => a.Text));
					var values = string.Empty;
					switch (s.Key)
					{
						case TokenType.Set:
							values = $"SET {expr}";
							break;
						case TokenType.Where:
							values = $"WHERE {expr}";
							break;
						case TokenType.Join:
							values = $"{expr}";
							break;
						case TokenType.GroupBy:
							values = $"Group By {expr}";
							break;
						case TokenType.Having:
							values = $"HAVING {expr}";
							break;
						case TokenType.OrderBy:
							values = $"ORDER BY {expr}";
							break;
						case TokenType.Limit:
							values = $"LIMIT {expr}";
							break;
					}
					return new KeyValuePair<TokenType, string>(s.Key, values);
				})
				.ToDictionary(s => s.Key, s => s.Value);
			return Regex.Replace(format, @"/\*\*(?<token>\w+)\*\*/", match =>
			{
				var token = match.Groups["token"].Value.ToUpper();
				var value = match.Value;
				var key = GetTokenType(token);
				if (key.HasValue && tokens.ContainsKey(key.Value))
				{
					return tokens[key.Value];
				}
				return string.Empty;
			}, RegexOptions.IgnoreCase);
		}

		public SqlBuilder Clone()
		{
			var sb = new SqlBuilder();
			foreach (var item in _tokens)
			{
				sb._tokens.Add(item);
			}
			return sb;
		}

		private static TokenType? GetTokenType(string token)
		{
			switch (token)
			{
				case "SET":
					return TokenType.Set;
				case "WHERE":
					return TokenType.Where;
				case "JOIN":
					return TokenType.Join;
				case "GROUPBY":
					return TokenType.GroupBy;
				case "HAVING":
					return TokenType.Having;
				case "ORDERBY":
					return TokenType.OrderBy;
				case "LIMIT":
					return TokenType.Limit;
			}
			return null;
		}

		class Token
		{
			public TokenType Type { get; }

			public string Text { get; }

			public Token(TokenType type, string text)
			{
				Type = type;
				Text = text;
			}
		}

		enum TokenType
		{
			From,
			Set,
			Where,
			Join,
			GroupBy,
			Having,
			OrderBy,
			Limit
		}
	}
}
