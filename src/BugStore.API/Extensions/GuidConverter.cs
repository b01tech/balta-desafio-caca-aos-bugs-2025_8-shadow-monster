using System.Text.Json;
using System.Text.Json.Serialization;
using BugStore.Exception.ExceptionMessages;
using BugStore.Exception.ProjectException;

namespace BugStore.API.Extensions
{
    public class GuidConverter : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
                throw new FormatInvalidException(ResourceExceptionMessage.GUID_EMPTY);

            if (!Guid.TryParse(value, out var guid))
                throw new FormatInvalidException(ResourceExceptionMessage.GUID_INVALID);

            return guid;
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
