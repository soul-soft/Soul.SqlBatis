using System;
using System.Collections.Generic;
using System.Linq;

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
            return Build($"SELECT {columns}", _tokens);
        }

        public string Count()
        {
            var tokens = _tokens
                .Where(a => a.Type != TokenType.OrderBy)
                .Where(a => a.Type != TokenType.Limit);
            return Build($"SELECT COUNT(*)", tokens);
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

        private static string Build(string select, IEnumerable<Token> tokens)
        {
            var list = tokens.GroupBy(a => a.Type)
                .OrderBy(s => s.Key)
                .Select(s =>
                {
                    var connector = ", ";
                    if (s.Key == TokenType.Where || s.Key == TokenType.GroupBy)
                        connector = " AND ";
                    var expr = string.Join(connector, s.Select(a => a.Text));
                    switch (s.Key)
                    {
                        case TokenType.From:
                            return $"FROM {expr}";
                        case TokenType.Where:
                            return $"WHERE {expr}";
                        case TokenType.GroupBy:
                            return $"Group By {expr}";
                        case TokenType.Having:
                            return $"HAVING {expr}";
                        case TokenType.OrderBy:
                            return $"ORDER BY {expr}";
                        case TokenType.Limit:
                            return $"LIMIT {expr}";
                        default:
                            throw new NotImplementedException();
                    }
                });
            return $"{select} {string.Join(" ", list)}";
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
            Where,
            GroupBy,
            Having,
            OrderBy,
            Limit
        }
    }
}
