namespace NetSchema.Syntax
{
    public enum YangTokenType
    {
        EndOfFile,
        Unknown,
        OpenBrace,
        CloseBrace,
        Semicolon,
        WhiteSpace,
        UnquotedString,
        DoubleQuotedString,
        SingleQuotedString,
        Comment
    }
}