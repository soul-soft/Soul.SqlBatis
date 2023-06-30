using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Soul.SqlBatis
{
    public class SqlBuilder
    {
        private List<Token> _tokens = new List<Token>();

        public SqlBuilder From(string sql)
        {
            _tokens.Add(new Token(TokenType.From, sql));
            return this;
        }
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
                _tokens.Add(new Token(TokenType.Where, sql));
            }
            return this;
        }

        public SqlBuilder Having(string sql, bool flag = true)
        {
            if (flag)
            {
                _tokens.Add(new Token(TokenType.Having, sql));
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

        public string Select(string columns = "*")
        {
            var tokens = _tokens;
            return string.Join(" ", $"SELECT {columns} FROM {View}", GetFilter(tokens));
        }

        public string Update()
        {
            var tokens = _tokens
               .Where(a => a.Type != TokenType.OrderBy)
               .Where(a => a.Type != TokenType.Having)
               .Where(a => a.Type != TokenType.GroupBy)
               .Where(a => a.Type != TokenType.Limit);
            return string.Join(" ", $"UPDATE {View}", GetFilter(tokens));
        }

        public string Delete()
        {
            var tokens = _tokens
               .Where(a => a.Type != TokenType.Set)
               .Where(a => a.Type != TokenType.OrderBy)
               .Where(a => a.Type != TokenType.Having)
               .Where(a => a.Type != TokenType.GroupBy)
               .Where(a => a.Type != TokenType.Limit);
            return string.Join(" ", $"DELETE FROM {View}", GetFilter(tokens));
        }

        public string Count()
        {
            var tokens = _tokens
                .Where(a => a.Type != TokenType.Set)
                .Where(a => a.Type != TokenType.OrderBy)
                .Where(a => a.Type != TokenType.Limit);
            return string.Join(" ", $"SELECT COUNT(*) FROM {View}", GetFilter(tokens));
        }

        public string Build(string format)
        {
            var tokens = GetTokens(_tokens);
            return Regex.Replace(format, @"/\*\*(?<token>\w+)\*\*/", match =>
            {
                var token = match.Groups["token"].Value.ToUpper();
                var value = match.Value;
                var key = GetTokenType(token);
                if (key.HasValue && tokens.ContainsKey(key.Value))
                {
                    return tokens[key.Value];
                }
                return value;
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

        private string View
        {
            get
            {
                return _tokens.Where(a => a.Type == TokenType.From).Select(s => s.Text).FirstOrDefault() ?? string.Empty;
            }
        }

        private static Dictionary<TokenType, string> GetTokens(IEnumerable<Token> tokens)
        {
            return tokens
                .Where(a => a.Type != TokenType.From && a.Type != TokenType.Limit)
                .GroupBy(a => a.Type)
                .OrderBy(s => s.Key)
                .Select(s =>
                {
                    var connector = ", ";
                    if (s.Key == TokenType.Where || s.Key == TokenType.GroupBy)
                        connector = " AND ";
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
        }

        private static string GetFilter(IEnumerable<Token> tokens)
        {
            return string.Join(" ", GetTokens(tokens).Select(s => s.Value));
        }

        private static TokenType? GetTokenType(string token)
        {
            switch (token)
            {
                case "SET":
                    return TokenType.Set;
                case "WHERE":
                    return TokenType.Where;
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
            GroupBy,
            Having,
            OrderBy,
            Limit
        }
    }
}
