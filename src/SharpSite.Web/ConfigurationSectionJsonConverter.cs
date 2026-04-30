using System.Text.Json;
using System.Text.Json.Serialization;
using SharpSite.Abstractions.Base;

namespace SharpSite.Web;

/// <summary>
/// Safe polymorphic JSON converter for <see cref="ISharpSiteConfigurationSection"/>.
/// Only resolves types that implement the interface — prevents arbitrary type instantiation (RCE).
/// </summary>
internal sealed class ConfigurationSectionJsonConverter : JsonConverter<ISharpSiteConfigurationSection>
{
	private const string TypeDiscriminatorPropertyName = "$type";

	public override ISharpSiteConfigurationSection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using var doc = JsonDocument.ParseValue(ref reader);
		var root = doc.RootElement;

		if (!root.TryGetProperty(TypeDiscriminatorPropertyName, out var typeElement))
			throw new JsonException("Missing type discriminator '$type' for configuration section.");

		var typeName = typeElement.GetString();
		if (string.IsNullOrEmpty(typeName))
			throw new JsonException("Empty type discriminator '$type' for configuration section.");

		var resolvedType = ResolveConfigurationType(typeName);
		if (resolvedType is null)
			throw new JsonException($"Unknown or disallowed configuration section type: {typeName}");

		return (ISharpSiteConfigurationSection?)JsonSerializer.Deserialize(root.GetRawText(), resolvedType, options);
	}

	public override void Write(Utf8JsonWriter writer, ISharpSiteConfigurationSection value, JsonSerializerOptions options)
	{
		var concreteType = value.GetType();

		writer.WriteStartObject();
		writer.WriteString(TypeDiscriminatorPropertyName, concreteType.FullName);

		using var doc = JsonDocument.Parse(JsonSerializer.Serialize(value, concreteType, options));
		foreach (var prop in doc.RootElement.EnumerateObject())
		{
			prop.WriteTo(writer);
		}

		writer.WriteEndObject();
	}

	private static Type? ResolveConfigurationType(string typeName)
	{
		// Strip assembly qualifier if present (backwards compat with legacy Newtonsoft format)
		var simpleTypeName = typeName.Contains(',') ? typeName.Split(',')[0].Trim() : typeName;

		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a =>
			{
				try { return a.GetTypes(); }
				catch { return Array.Empty<Type>(); }
			})
			.FirstOrDefault(t =>
				t.FullName == simpleTypeName &&
				typeof(ISharpSiteConfigurationSection).IsAssignableFrom(t) &&
				!t.IsInterface &&
				!t.IsAbstract);
	}
}
