﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace JSONParser
{
    internal class Lexer
    {
        private readonly string _input;

        public List<Lexem> _lexems;
        private int _counter;

        public Lexer(string input)
        {
            _lexems = new List<Lexem>();
            _input = input;
        }

        public List<Lexem> MoveNext()
        {
            if (_input == string.Empty)
            {
                return null;
            }

            while (_counter < _input.Length)
            {

                if (_input[_counter] == '{' || _input[_counter] == '}' ||
                    _input[_counter] == '[' || _input[_counter] == ']' ||
                    _input[_counter] == ':' || _input[_counter] == ',')
                {
                    (int token, string symbol) = GetAnySymbol();
                    _lexems.Add(new Lexem(token, symbol));
                }
                else if (_input[_counter] == '"')
                {
                    (int token, string keyОrString) = GetKeyOrString();
                    _lexems.Add(new Lexem(token, keyОrString));
                }
                else if (Char.IsNumber(_input[_counter]))
                {
                    (int token, string intОrDouble) = GetIntOrDouble();
                    _lexems.Add(new Lexem(token, intОrDouble));
                }
                else if (Char.IsLetter(_input[_counter]))
                {
                    (int token, string intОrDouble) = GetBoolOrNull();
                    _lexems.Add(new Lexem(token, intОrDouble));
                }
                else
                {
                    return null;
                }
            }

            if (_lexems.Contains(null))
            {
                return null;
            }

            return _lexems;
        }

        private (int, string) GetAnySymbol()
        {
            char symbol = _input[_counter];
            List<char> interimList = new();
            interimList.Add(symbol);
            _counter++;
            if (_input[_counter] == ' ' || _input[_counter] == '\r')
            {
                interimList = AddWhiteSpacesOrNewNine(interimList);
            }

            if (symbol == '{')
            {
                return ((int)Tokens.OpenObjectBrace, new string(interimList.ToArray()));
            }
            else if (symbol == '}')
            {
                return ((int)Tokens.CloseObjectBrace, new string(interimList.ToArray()));
            }
            else if (symbol == '[')
            {
                return ((int)Tokens.OpenArrayBrace, new string(interimList.ToArray()));
            }
            else if (symbol == ']')
            {
                return ((int)Tokens.CloseArrayBrace, new string(interimList.ToArray()));
            }
            else if (symbol == ':')
            {
                return ((int)Tokens.Colon, new string(interimList.ToArray()));
            }
            else
            {
                return ((int)Tokens.Comma, new string(interimList.ToArray()));
            }
        }

        private (int, string) GetKeyOrString()
        {
            List<char> interimList = EnsureStringValid();
            if (interimList == null)
            {
                return ((int)Tokens.NotValidToken, null);
            }

            while (_counter < _input.Length)
            {
                if (_input[_counter] == ' ')
                {
                    interimList = AddWhiteSpacesOrNewNine(interimList);
                }
                else if (_input[_counter] == ':')
                {
                    return ((int)Tokens.Key, new string(interimList.ToArray()));
                }
                else if (_input[_counter] == ',' || _input[_counter] == '}' || _input[_counter] == ']')
                {
                    return ((int)Tokens.String, new string(interimList.ToArray()));
                }
                else
                {
                    return ((int)Tokens.NotValidToken, null);
                }
            }

            return ((int)Tokens.NotValidToken, null);
        }

        private List<char> EnsureStringValid()
        {
            List<char> interimList = new();
            interimList.Add(_input[_counter]);

            while (_counter < _input.Length)
            {
                _counter++;
                interimList.Add(_input[_counter]);
                if (_input[_counter] == '\\')
                {
                    if (_input[_counter + 1] == '\\' || _input[_counter + 1] == '"')
                    {
                        interimList.Add(_input[_counter + 1]);
                        _counter++;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (_input[_counter] == '"')
                {
                    _counter++;
                    interimList = AddWhiteSpacesOrNewNine(interimList);
                    return interimList;
                }
            }

            return null;
        }

        private (int, string) GetIntOrDouble()
        {
            List<char> interimList = GetAnyWordOrNumber();
            string interimString = new(interimList.ToArray());
            if (int.TryParse(interimString, out _) ||
                double.TryParse(interimString, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            {
                interimList = AddWhiteSpacesOrNewNine(interimList);
                interimString = new(interimList.ToArray());
                return ((int)Tokens.IntOrDouble, interimString);
            }
            else
            {
                return ((int)Tokens.NotValidToken, null);
            }
        }

        private (int, string) GetBoolOrNull()
        {
            List<char> interimList = GetAnyWordOrNumber();
            string interimString = new(interimList.ToArray());
            if (interimString == "true" || interimString == "false" || interimString == "null")
            {
                interimList = AddWhiteSpacesOrNewNine(interimList);
                interimString = new(interimList.ToArray());
                return ((int)Tokens.BoolOrNull, interimString);
            }
            else
            {
                return ((int)Tokens.NotValidToken, null);
            }
        }

        private List<char> GetAnyWordOrNumber()
        {
            List<char> interimList = new();
            while (_counter < _input.Length)
            {
                if (_input[_counter] == ' ' || _input[_counter] == ',' ||
                    _input[_counter] == ']' || _input[_counter] == '}')
                {
                    return interimList;
                }

                interimList.Add(_input[_counter]);
                _counter++;
            }

            return interimList;
        }

        private List<char> AddWhiteSpacesOrNewNine(List<char> token)
        {
            while (_counter < _input.Length)
            {
                if (_input[_counter] == ' ' || _input[_counter] == '\r' || _input[_counter] == '\n')
                {
                    token.Add(_input[_counter]);
                    _counter++;
                }
                else
                {
                    return token;
                }
            }

            return token;
        }
    }
}