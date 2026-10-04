using System.Globalization;
using System.Text.Json;

namespace WebAppCosmosDBDEMO.Utils
{
    public static class ParameterFormattingExtensions
    {
        public static decimal ToDecimal(object? value)
        {
            return value switch
            {
                null => 0m,
                decimal d => d,
                double d => (decimal)d,
                float f => (decimal)f,
                int i => i,
                long l => l,

                // Handles System.Text.Json payloads
                JsonElement element => element.ValueKind switch
                {
                    JsonValueKind.Number => element.GetDecimal(),
                    JsonValueKind.String => decimal.TryParse(element.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed) ? parsed : 0m,
                    _ => 0m
                },

                // Handles string inputs like "1000.00" or "1000,00"
                string s => decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedString)
                    ? parsedString
                    : 0m,

                // Fallback for other numeric types (short, byte, etc.)
                IConvertible convertible => Convert.ToDecimal(convertible, CultureInfo.InvariantCulture),

                _ => 0m
            };
        }
    }
}
