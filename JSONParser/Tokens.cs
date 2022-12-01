namespace JSONParser
{
    internal enum Tokens
    {
        NotValidToken,
        OpenObjectBrace,
        CloseObjectBrace,
        OpenArrayBrace,
        CloseArrayBrace,
        Colon,
        Comma,
        Key,
        String,
        IntOrDouble,
        BoolOrNull
    }
}
