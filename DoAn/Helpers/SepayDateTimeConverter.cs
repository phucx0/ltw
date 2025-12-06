using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DoAn.Areas.Booking.Services
{
    public class SepayDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string[] DateTimeFormats = new[]
        {
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
            "yyyy-MM-dd'T'HH:mm:ss'Z'",
            "dd/MM/yyyy HH:mm:ss",
            "dd-MM-yyyy HH:mm:ss",
            "yyyy/MM/dd HH:mm:ss",
            "yyyy-MM-dd",
            "dd/MM/yyyy",
            "MM/dd/yyyy HH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "yyyy-MM-ddTHH:mm:ssZ"
        };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();

            if (string.IsNullOrWhiteSpace(dateString))
            {
                return DateTime.MinValue;
            }

            if (DateTime.TryParseExact(dateString, DateTimeFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime result))
            {
                return result;
            }

            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            if (long.TryParse(dateString, out long unixTime))
            {
                return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
            }
            Console.WriteLine($"Failed to parse DateTime: '{dateString}'");

            return DateTime.MinValue;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}