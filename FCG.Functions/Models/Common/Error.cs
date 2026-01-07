using FCG.Functions.Enums;

namespace FCG.Functions.Models.Common;

public record Error(
    ErrorType Type,
    string Code,
    string Message
);
